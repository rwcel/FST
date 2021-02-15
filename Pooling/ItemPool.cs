using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemPool : PoolingMgr
{
    private static ItemPool _Instance;
    public static ItemPool Instance {
        get {
            return _Instance;
        }
    }

    protected virtual void Awake() {
        _Instance = this.GetComponent<ItemPool>();
    }

    public int _MaxItem;

    protected override void SetPool() {
        // **계속 변화 필요
        m_Pools = new Pools[(int)PaymentType.Medal+1];
        for (int i = 0, length = m_Pools.Length; i < length; i++) {
            m_Pools[i].Max = _MaxItem;
            m_Pools[i].Name = ((PaymentType)i).ToString();
            m_Pools[i].Obj = ResourceMgr.Instance.GetPrefab("Item",i);

            Queue<GameObject> queue = new Queue<GameObject>();
            GameObject parent = new GameObject(m_Pools[i].Name + "_Pool");
            m_Pools[i].Parent = parent.transform;
            parent.transform.SetParent(transform);

            for (int j = 0, length2 = m_Pools[i].Max; j < length2; j++) {
                GameObject obj = Instantiate(m_Pools[i].Obj);
                obj.name = m_Pools[i].Name + "_" + j.ToString();
                obj.transform.SetParent(parent.transform);
                obj.SetActive(false);

                queue.Enqueue(obj);
            }
            m_PoolDictionary.Add(m_Pools[i].Name, queue);
        }
    }

    // 자동으로 해주는 함수
    public void DropItems(PaymentType itemType, Transform parent, Vector3 dropPos, Vector3 targetPos, 
                          int totalValue, int size, float randomPosX, float radomPosY, 
                          float minAnimDuration = 0.6f, float maxAnimDuration = 1.5f, Ease easeType = Ease.InBack) {

        int parValue, lastValue = 0;
        parValue = totalValue / size;

        // 안되는 경우 : lastValue 생김
        if (parValue * size != totalValue) {
            parValue = totalValue / (size - 1);
            lastValue = totalValue % (size - 1);
        }

        for (int i = 0; i < size - 1; i++) {
            GameObject item = DeQueue(itemType.ToString());
            // null 처리
            item.transform.position = dropPos +
                            new Vector3(Random.Range(-randomPosX, randomPosX), Random.Range(-radomPosY, radomPosY));
            item.transform.SetParent(parent);

            item.transform.DOMove(targetPos, Random.Range(minAnimDuration, maxAnimDuration)).SetEase(easeType).OnComplete(() => {
                ItemPool.Instance.EnQueue(item);
                PlayerMgr.Instance.AddResource(itemType, parValue);
            });
        }

        GameObject lastItem = DeQueue(itemType.ToString());
        lastItem.transform.position = dropPos +
                        new Vector3(Random.Range(-randomPosX, randomPosX), Random.Range(-radomPosY, radomPosY));
        lastItem.transform.SetParent(parent);

        lastItem.transform.DOMove(targetPos, Random.Range(minAnimDuration, maxAnimDuration)).SetEase(easeType).OnComplete(() => {
            ItemPool.Instance.EnQueue(lastItem);
            PlayerMgr.Instance.AddResource(itemType, lastValue == 0 ? parValue : lastValue);
        });
    }
}
