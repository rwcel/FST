using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 번호 순서대로 부여해서 번호 매기기
/// </summary>
public class ListItems : MonoBehaviour
{
    [SerializeField] Transform      m_ListParent;
    [SerializeField] GameObject     m_Prefab;
    [SerializeField] Component      m_ItemScript;       // T

    private string                  m_TypeName;
    private List<GameObject>        m_ItemList;         // T
    private GameObject              m_SelectItem;
    private Component               m_SelectType;

    /// <summary> * Awake보다 먼저 시작 </summary>
    public void SetItem(int itemCount) {
        m_ListParent.gameObject.SetActive(false);
        if (m_ItemList == null) {
            FirstSetting();
        }
        m_ItemList.Clear();

        int childCount = m_ListParent.childCount;
        // 아이템이 자식개수보다 적을 때 : 나머지 setactive false
        if (itemCount <= childCount) {
            for (int i = 0; i < itemCount; i++) {
                m_ItemList.Add(m_ListParent.GetChild(i).gameObject);
            }
            for (int i = itemCount; i < childCount; i++) {
                m_ListParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        else {
            for (int i = 0; i < childCount; i++) {
                m_ItemList.Add(m_ListParent.GetChild(i).gameObject);
            }
            for (int i = childCount; i < itemCount; i++) {
                GameObject newObj = Instantiate(m_Prefab, m_ListParent);
                newObj.name = m_Prefab.name + "_" + i;
                m_ItemList.Add(newObj);
            }
        }

        m_ListParent.gameObject.SetActive(true);
        for (int i = 0, length = m_ItemList.Count; i < length; i++) {
            m_ItemList[i].SetActive(true);
        }
    }

    /// <summary> 초기 설정 </summary>
    private void FirstSetting() {
        m_ItemList = new List<GameObject>();
        m_TypeName = m_ItemScript.GetType().Name;

        GameObject child;
        for (int i = 0, length = m_ListParent.childCount; i < length; i++) {
            child = m_ListParent.GetChild(i).gameObject;
            child.name = m_Prefab.name + "_" + i;
            child.SetActive(false);
        }
    }

    public List<GameObject> GetList() {
        return m_ItemList;
    }

    /// <summary>
    /// 기존 클릭 : 비활성화
    /// 현재 클릭 : 활성화
    /// </summary>
    public void SetSelectItem(int selectNum) {
        if(selectNum == -1) {
            m_SelectItem = null;
        }
        else {
            m_SelectItem = m_ItemList[selectNum];
        }
    }

    public GameObject GetSelectItem() {
        return m_SelectItem;
    }

    public void SetSelectType(int selectNum) {
        if (selectNum == -1) {
            m_SelectType = null;
        }
        else {
            m_SelectType = m_ItemList[selectNum].GetComponent(m_TypeName);
        }
    }

    public Component GetSelectType() {
        if(m_SelectItem == null) {
            return null;
        }
        return m_SelectItem.GetComponent(m_TypeName);
    }
}
