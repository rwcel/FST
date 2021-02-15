using UnityEngine;
using UnityEngine.UI;

public class ChallengeItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text               m_SubTitle;
    [SerializeField] Text               m_Count;

    [Header("Condition")]
    [SerializeField]
    protected GameObject                m_ListType;
    [SerializeField]
    protected GameObject                m_PartyType;
    [SerializeField] GameObject         m_LockObj;

    private ChallengeType               m_Type;
    private GameObject                  m_SelectObj;
    private ChallengeSubItem            m_SubItem;

    private int                         m_RemainCount;
    private int                         m_MaxCount;

    private int[] _Rebirth_Condition = { 2, 3, 4, 5 };

    public void SetData(ChallengeType type) {
        m_Type = type;

        if (m_Type == ChallengeType.Daily) {
            m_SelectObj = m_ListType;
            m_SubTitle.text = CSVMgr.Instance.GetLanguage(885);
            m_RemainCount = PlayerMgr.Instance.GetDungeon();
            m_MaxCount = PlayerMgr.Instance.GetMaxDungeon();
        }
        else if(m_Type == ChallengeType.Boss) {
            m_SelectObj = m_ListType;
            m_SubTitle.text = CSVMgr.Instance.GetLanguage(912);
            m_RemainCount = PlayerMgr.Instance.GetChargedItem((int)ChargedItemType.RaidTicket);
            m_MaxCount = CSVMgr.Instance.GetChargedItemMax((int)ChargedItemType.RaidTicket);
        }
        else if(m_Type == ChallengeType.Tower){
            m_SelectObj = m_PartyType;
            m_SubTitle.text = CSVMgr.Instance.GetLanguage(521);
            m_RemainCount = PlayerMgr.Instance.GetTower();
            m_MaxCount = 1;
        }
        else if (m_Type == ChallengeType.Arena) {
            m_SelectObj = m_PartyType;
            m_SubTitle.text = CSVMgr.Instance.GetLanguage(502);
            m_RemainCount = PlayerMgr.Instance.GetChargedItem((int)ChargedItemType.ColosseumTicket);
            m_MaxCount = CSVMgr.Instance.GetChargedItemMax((int)ChargedItemType.ColosseumTicket);
        }
        else {
            Debug.Log("없는 타입 : 탐험");
        }

        m_Count.text = string.Format("Entry {0}/{1}", m_RemainCount, m_MaxCount);
        m_LockObj.SetActive(PlayerMgr.Instance.GetResurrection() < _Rebirth_Condition[(int)m_Type]);
        m_SubItem = m_SelectObj.GetComponent<ChallengeSubItem>();
        m_SelectObj.SetActive(false);
    }

    #region interaction

    public virtual void Click_Select() {
        m_SubItem.SetScreen(m_Type, m_SubTitle.text);
        m_SelectObj.SetActive(true);
    }

    public virtual void Click_CloseSelect() {
        m_SelectObj.SetActive(false);
    }

    /// <summary>
    /// **지우기
    /// </summary>
    public void Click_Info() {
        ChallengeMgr.Instance.ShowInfo((int)m_Type);
        m_SelectObj.SetActive(false);
    }

    public void Click_SetHero() {
        PartyUIMgr.Instance.Show();
        HeroPartyMgr.Instance.Show(m_Type == ChallengeType.Tower ? HeroPartyType.Tower : HeroPartyType.Colosseum);
    }

    public void Click_Enter() {
        NoneTouchMsg.Instance.ShowDeveloping();
    }

    #endregion
}
