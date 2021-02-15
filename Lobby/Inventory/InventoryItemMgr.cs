using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryItemMgr : EnableUIMgr<InventoryItemMgr>
{
    [SerializeField] Transform      m_ItemListParent;

    [SerializeField] ListItems      m_ListItems;

    private int                     m_SelectListItem;

    public List<int>                _ItemType;
    public List<int>                _ItemIDX;
    public List<bool>               _ItemAlarm;

    private CSVMgr                  m_CSVMgr;
    private PlayerMgr               m_PlayerMgr;


    protected override void SetData() {
        m_CSVMgr = CSVMgr.Instance;
        m_PlayerMgr = PlayerMgr.Instance;

        _ItemType = new List<int>();
        _ItemIDX = new List<int>();
        _ItemAlarm = new List<bool>();

        m_SelectListItem = -1;
    }

    protected override void Show() {
        SetInventoryItem();
        Refresh();
    }

    private void SetInventoryItem() {
        int itemType;

        _ItemType.Clear();
        _ItemIDX.Clear();
        _ItemAlarm.Clear();

        if (InventoryMgr.Instance._SelectItemType == InventoryItemType.Piece) {
            for (int i = 0, length = m_CSVMgr.GetItemCnt(); i < length; i++) {
                itemType = m_CSVMgr.GetItemType(i);

                if (itemType == (int)InventoryItemType.Piece && m_PlayerMgr.GetResource(i) > 0) {
                    _ItemType.Add((int)InventoryItemType.Piece);
                    _ItemIDX.Add(i);
                    _ItemAlarm.Add(m_PlayerMgr.GetResourceAlarm(i));
                }
            }
        }
        else {
            for (int i = 0, length = m_CSVMgr.GetItemCnt(); i < length; i++) {
                itemType = m_CSVMgr.GetItemType(i);

                if (PlayerMgr.Instance.GetResource(i) > 0 && (itemType == (int)InventoryItemType.Normal ||
                                                  itemType == (int)InventoryItemType.Ticket)) {

                    _ItemType.Add(itemType);
                    _ItemIDX.Add(i);
                    _ItemAlarm.Add(m_PlayerMgr.GetResourceAlarm(i));
                }
            }
        }
    }

    public void SelectListItem(int listNum) {
        InventoryItem item = m_ListItems.GetSelectType() as InventoryItem;
        if (item != null) {
            item.SetSelectObj(false);
            if (item.GetArrayNum() == listNum) {       // 같은 번호라면 선택 진행 안함
                listNum = -1;
            }
        }
        m_ListItems.SetSelectItem(listNum);
        if (listNum != -1) {
            item = m_ListItems.GetSelectType() as InventoryItem;
            item.SetSelectObj(true);
            m_SelectListItem = item.GetKey();
        }
    }

    private void Refresh() {
        m_SelectListItem = -1;
        SelectListItem(-1);

        m_ListItems.SetItem(_ItemType.Count);
    }
}
