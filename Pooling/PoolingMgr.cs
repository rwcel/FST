using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingMgr : MonoBehaviour
{
    [System.Serializable]
    protected struct Pools
    {
        public string           Name;       // [Key]
        public GameObject       Obj;
        public int              Max;
        [HideInInspector]
        public Transform        Parent;
    }

    [SerializeField] protected Pools[]      m_Pools;

    protected Dictionary<string, Queue<GameObject>> m_PoolDictionary;

    protected void Start() {
        // base.Awake(); 

        m_PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        SetPool();
    }

    protected virtual void SetPool() {
        for (int i = 0, length = m_Pools.Length; i < length; i++) {
            Queue<GameObject> queue = new Queue<GameObject>();
            GameObject parent = new GameObject(m_Pools[i] + "_Pool");
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

    public GameObject DeQueue(string name) {
        try {
            GameObject obj = m_PoolDictionary[name].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        catch { return null; }
    }

    public virtual void EnQueue(GameObject obj) {
        obj.SetActive(false);

        string[] split = obj.name.Split('_');

        for (int i = 0, length = m_Pools.Length; i < length; i++) {
            if( split[0] == m_Pools[i].Name) {
                obj.transform.SetParent(m_Pools[i].Parent);
                break;
            }
        }
        m_PoolDictionary[split[0]].Enqueue(obj);
    }
}
