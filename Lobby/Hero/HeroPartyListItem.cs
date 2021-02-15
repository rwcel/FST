using UnityEngine;
using UnityEngine.UI;

public class HeroPartyListItem : ListItemData
{
    [SerializeField] GameObject     m_InPartyObject;        // 어두워짐

    [Header("UI")]
    [SerializeField] Image          m_SDImage;
    [SerializeField] Image          m_Class;
    [SerializeField] Image          m_Race;
    [SerializeField] Text           m_Level;
    [SerializeField] Transform      m_StarParent;

    private int                     m_HeroNum;              // 영웅 번호
    private bool                    m_IsInParty;            // 파티에 있는지


    protected override void SetScreen() {
        base.SetScreen();

        m_HeroNum = HeroPartyMgr.Instance._HeroList[m_Num];

        m_SDImage.sprite = ResourceMgr.Instance.GetSprite("HeroSD", m_HeroNum % 5);    // #
        m_SDImage.SetNativeSize();
        m_Class.sprite = ResourceMgr.Instance.GetSprite("Class", CSVMgr.Instance.GetType(m_HeroNum));
        m_Race.sprite = ResourceMgr.Instance.GetSprite("Race", CSVMgr.Instance.GetRace(m_HeroNum));

        // **처음에 1부터 시작하게 코드를 선언해줘야 할 듯 합니다.
        m_Level.text = string.Format("LV.{0}", (PlayerMgr.Instance.GetHeroLv(m_HeroNum) + 1));

        int starNum = PlayerMgr.Instance.GetHeroAwaken(m_HeroNum) + 1;
        for (int i = 0, length = m_StarParent.childCount; i < length; i++) {
            m_StarParent.GetChild(i).gameObject.SetActive(i < starNum);
        }

        // 알람 표시 : 각성, 새로얻음
        m_NewObject.SetActive(PlayerMgr.Instance.GetHeroUP(m_HeroNum) || PlayerMgr.Instance.GetHeroNew(m_HeroNum));
        //         _Obj[(int)Objects.Level].SetActive(HeroPartyMgr.Instance._PartyType == HeroPartyType.InGame);


        m_IsInParty = HeroPartyMgr.Instance.HasHeroInParty(m_HeroNum);
        m_InPartyObject.SetActive(m_IsInParty);
    }

    public override int GetKey() {
        return m_HeroNum;
    }


    #region interaction

    public override void Click_Item()
    {
        base.Click_Item();

        HeroPartyMgr.Instance.SelectListItem(m_Num, true);
    }

    public void Click_Detail() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        HeroInfoMgr.Instance.Show(m_HeroNum);
    }

    #endregion interaction
}
