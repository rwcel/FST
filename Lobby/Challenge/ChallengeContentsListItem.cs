using UnityEngine;
using UnityEngine.UI;

public class ChallengeContentsListItem : ListItemData
{
    [SerializeField] Text               m_NameText;
    [SerializeField] Image              m_RewardImage;
    [SerializeField] Text               m_CountText;

    private ChallengeType               m_Type;


    protected override void SetScreen() {
        //m_NameText.text = ;
        //m_RewardImage.sprite = ;
        //m_CountText.text = ;

        //m_Type = (ChallengeType)ChallengeMgr.Instance._ItemType[m_Num];
        //m_IDX = InventoryItemMgr.Instance._ItemIDX[m_Num];

        //m_Name = CSVMgr.Instance.GetItemName(m_IDX);
        //m_Info = CSVMgr.Instance.GetItemDescription(m_IDX);
        //if (m_Type == InventoryItemType.Piece) {
        //    m_ItemImage.sprite = ResourceMgr.Instance.GetSprite("HeroFace", (m_IDX % 4));           // #
        //    m_ItemCount.text = PlayerMgr.Instance.GetResource(m_IDX).ToString();
        //}
        //else {
        //    m_ItemImage.sprite = ResourceMgr.Instance.GetSprite("Item", (m_IDX > 13) ? 11 : m_IDX);
        //    m_ItemCount.text = PlayerMgr.Instance.GetResourceDivision(m_IDX).ToString();
        //}
    }

    public override int GetKey() {
        return m_Num;
    }

    public override void Click_Item() {
        base.Click_Item();

        ChallengeMgr.Instance.ShowInfo(m_Num);
    }
}
