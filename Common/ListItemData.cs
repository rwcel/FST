using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItemData : MonoBehaviour
{
    [SerializeField]
    protected GameObject    m_SelectObject;         // 바깥 마젠타 색, Detail 표시
    [SerializeField]
    protected GameObject    m_NewObject;            // 알람

    protected int           m_Num;                  // 배열 번호
    protected bool          m_IsSelect;

    // 번호 매기기
    protected void OnEnable() {
        string[] split = gameObject.name.Split('_');
        m_Num = int.Parse(split[1]);
        SetScreen();                            
    }

    protected virtual void SetScreen() {
        m_SelectObject.SetActive(false);
    }

    public int GetArrayNum() {
        return m_Num;
    }

    public virtual int GetKey() {
        return -1;
    }

    /// <summary>
    /// 선택 변경
    /// </summary>
    public void SetSelectObj(bool active) {
        m_SelectObject.SetActive(active);
    }


    public virtual void Click_Item() {
        ResourceMgr.Instance.PlaySound("FX", 0);
    }
}
