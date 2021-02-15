using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;

public class EquipMgr : EnableUIMgr<EquipMgr>
{
    [SerializeField] Transform          m_EquipSlotParent;

    [SerializeField] ListItems          m_ListItems;

    [HideInInspector] public List<int>  _EquipIDXList;

    private List<EquipItem>             m_EquipSlotList;

    private PlayerMgr                   m_PlayerMgr;

    private int                         m_SelectSlot;
    private int                         m_SelectListItem;

    private bool                        m_IsChange;

    private static readonly int _MAX_Artifact_Cnt = 5;

    protected override void SetData()
    {
        m_PlayerMgr = PlayerMgr.Instance;

        m_EquipSlotList = new List<EquipItem>();
        for (int i = 0, length = m_EquipSlotParent.childCount; i < length; i++) {
            m_EquipSlotList.Add(m_EquipSlotParent.GetChild(i).GetComponent<EquipItem>());
        }
        _EquipIDXList = new List<int>();

        m_SelectListItem = -1;
        m_SelectSlot = -1;
    }

    protected override void Show() {
        SetEquipItem();
        Refresh();          // 포함 : 리스트 추가
    }

    public int GetArtifactIDXCnt()
    {
        return _EquipIDXList.Count;
    }

    public void SetEquipItem()
    {
        _EquipIDXList.Clear();
        int _A_IDX;
        for (int i = 0, length = m_PlayerMgr.GetArtifactCnt(); i < length; i++) {
            _A_IDX = m_PlayerMgr.GetArtifactIDX(i);

            if (m_PlayerMgr.GetArtifactEquip(_A_IDX) > 0) {
                m_EquipSlotList[_EquipIDXList.Count].SetItem(_EquipIDXList.Count, _A_IDX);
                _EquipIDXList.Add(_A_IDX);
            }
        }

        for (int i = _EquipIDXList.Count; i < _MAX_Artifact_Cnt; i++) {
            m_EquipSlotList[i].SetItem(i, -1);
        }
    }

    public int GetArtifactIDX(int _Num)
    {
        return _EquipIDXList[_Num];
    }

    public bool IsEquipItem(int num) {
        return _EquipIDXList.Contains(num);
    }

    public int GetSlotCnt()
    {
        return Mathf.Clamp(PlayerMgr.Instance.GetResurrection() / _MAX_Artifact_Cnt, 0, 4);
    }

    #region Condition
    public void SelectSlotItem(int slotNum) {
        if (m_SelectSlot != -1) {
            m_EquipSlotList[m_SelectSlot].SetSelectObj(m_IsChange, true);
        }
        m_IsChange = !m_IsChange;
        m_SelectSlot = slotNum;
        m_EquipSlotList[m_SelectSlot].SetSelectObj(m_IsChange, true);

    }
 
    public void SelectListItem(int listNum) {
        ArtifactItem item = m_ListItems.GetSelectType() as ArtifactItem;
        if (item != null) {
            item.SetSelectObj(false);
            if (item.GetArrayNum() == listNum) {       // 같은 번호라면 선택 진행 안함
                listNum = -1;
            }
        }
        m_ListItems.SetSelectItem(listNum);
        if (listNum != -1) {
            item = m_ListItems.GetSelectType() as ArtifactItem;
            item.SetSelectObj(true);
        }

        m_SelectListItem = item.GetKey();


        if (!m_IsChange || listNum == -1) {
            m_IsChange = !m_IsChange;
        }

        for (int i = 0; i < m_EquipSlotList.Count; i++) {
            m_EquipSlotList[i].SetSelectObj(m_IsChange, false);
        }
    }


    public void AddArtifact(int slotNum) {
        m_EquipSlotList[slotNum].SetItem(slotNum, m_SelectListItem);
        _EquipIDXList.Add(m_SelectListItem);

        m_PlayerMgr.SetArtifactEquip(m_SelectListItem, true);

        SetEquipItem();
        Refresh();
    }

    public void RemoveArtifact(int slotNum, int idx) {
        _EquipIDXList.RemoveAt(slotNum);
        m_EquipSlotList[slotNum].SetItem(slotNum, -1);

        m_PlayerMgr.SetArtifactEquip(idx, false);

        SetEquipItem();
        Refresh();
    }

    public void ChangeArtifact(int slotNum) {
        m_EquipSlotList[slotNum].SetItem(slotNum, m_SelectListItem);

        m_PlayerMgr.SetArtifactEquip(_EquipIDXList[slotNum], false);
        m_PlayerMgr.SetArtifactEquip(m_SelectListItem, true);
        _EquipIDXList[slotNum] = m_SelectListItem;

        SetEquipItem();
        Refresh();
    }

    private void Refresh() {
        m_IsChange = false;

        m_SelectListItem = -1;
        m_SelectSlot = -1;
        m_ListItems.SetItem(m_PlayerMgr.GetArtifactCnt());
        m_ListItems.SetSelectItem(-1);

        for (int i = 0; i < m_EquipSlotList.Count; i++) {
            m_EquipSlotList[i].SetSelectObj(m_IsChange, false);
        }
    }

    #endregion Condition
}
