using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : ListItemData
{
    [SerializeField] Image              m_ItemImage;
    [SerializeField] Text               m_ItemCount;

    private int                         m_IDX;
    private InventoryItemType           m_Type;
    private string                      m_Name;
    private string                      m_Info;

    protected override void SetScreen()
    {
        m_Type = (InventoryItemType)InventoryItemMgr.Instance._ItemType[m_Num];
        m_IDX = InventoryItemMgr.Instance._ItemIDX[m_Num];
        m_NewObject.gameObject.SetActive(InventoryItemMgr.Instance._ItemAlarm[m_Num]);

        m_Name = CSVMgr.Instance.GetItemName(m_IDX);
        m_Info = CSVMgr.Instance.GetItemDescription(m_IDX);
        if (m_Type == InventoryItemType.Piece) {
            m_ItemImage.sprite = ResourceMgr.Instance.GetSprite("HeroFace", (m_IDX % 4));           // #
            m_ItemCount.text = PlayerMgr.Instance.GetResource(m_IDX).ToString();
        }
        else {
            m_ItemImage.sprite = ResourceMgr.Instance.GetSprite("Item", m_IDX);
            m_ItemCount.text = PlayerMgr.Instance.GetResourceDivision(m_IDX).ToString();
        }
    }

    public override int GetKey() {
        return m_IDX;
    }

    public override void Click_Item() {
        base.Click_Item();

        InventoryItemMgr.Instance.SelectListItem(m_Num);
        InventoryMgr.Instance.SelectItem(m_Type, m_IDX, m_ItemImage.sprite, m_Name, m_ItemCount.text, m_Info);
    }
}
