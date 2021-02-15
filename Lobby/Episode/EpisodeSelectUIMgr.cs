using UnityEngine;
using UnityEngine.UI;

public class EpisodeSelectUIMgr : VisibleUIMgr<EpisodeSelectUIMgr>
{
    [Header("Common UI")]
    [SerializeField] Text               m_Title;
    [SerializeField] Text               m_EpisodeTitle;
    [SerializeField] Image              m_CharImage;
    [SerializeField] Text               m_PaymentValueText;
    [SerializeField] Text               m_EpisodeCostText;
    [SerializeField] Text               m_ChoiceText;

    [Header("Limit UI")]
    [SerializeField] GameObject         m_ClearObj;             // 에피소드 완료          : PlayerPrefs
    [SerializeField] GameObject         m_LockObj;              // 에피소드 활성화
    [SerializeField] Text               m_LockText;             // 활성화 조건

    [Header("Other Scripts")]
    [SerializeField] ListItems          m_ListItems;

    // private EpisodeData[]               m_EpisodeDatas;

    private double                      m_PaymentValue;
    private double                      m_EpisodeCost;

    private int                         m_SelectNum;


    public override void Show() {
        int count = 0;
        for (int i = 0, length = CSVMgr.Instance.GetHeroCnt(); i < length; i++) {
            if (PlayerMgr.Instance.GetHeroLv(i) >= 0) {
                count++;
            }
        }
        m_ListItems.SetItem(count);

        base.Show();
    }

    public override void SetScreen() {
        m_Title.text = "EPISODE";               // # Language
        m_PaymentValue = PlayerMgr.Instance.GetResource(PaymentType.RebirthStone);
        m_PaymentValueText.text = string.Format("{0:n0}", m_PaymentValue);
        m_ChoiceText.text = "CHOICE";           // #

        m_SelectNum = -1;

        SelectHeroListItem(0);       // 0번 자동선택
    }

    /// <summary>
    /// # csv통해서 바꾸고 환생 횟수 가져오기
    /// </summary>
    public void UpdateEpisode(int heroNum) {
        if (PlayerMgr.Instance.GetResurrection() >= heroNum) {                               // #
            m_LockObj.SetActive(false);
        }
        else {
            m_LockObj.SetActive(true);
            m_LockText.text = string.Format("Unlock to {0} Rebirth", heroNum);
        }
        m_ClearObj.SetActive(PrefsMgr.Instance.GetEpisodeClear(heroNum));

        m_EpisodeTitle.text = string.Format("{0}'S\nEPISODE", CSVMgr.Instance.GetHeroName(heroNum).ToUpper());
        m_CharImage.sprite = ResourceMgr.Instance.GetSprite("HeroFull", heroNum % 6);       // #
        m_EpisodeCost = heroNum % 2 == 0 ? 100 : 200;                                       // # Table
        m_EpisodeCostText.text = string.Format("{0:#,###}", m_EpisodeCost);
    }

    /// <summary>
    /// 캐릭터 선택에 따른 반응
    /// </summary>
    public void SelectHeroListItem(int listNum) {
        EpisodeCharItem item = m_ListItems.GetSelectType() as EpisodeCharItem;
        if (item != null) {
            item.SetSelectObj(false);
            if (item.GetArrayNum() == listNum) {       // 같은 번호라면 선택 진행 안함
                listNum = -1;
            }
        }
        m_ListItems.SetSelectItem(listNum);
        if (listNum != -1) {
            item = m_ListItems.GetSelectType() as EpisodeCharItem;
            item.SetSelectObj(true);
            //m_IsChange = !m_IsChange;
        }

        m_SelectNum = listNum;

        UpdateEpisode(item.GetKey());
    }

    /// <summary>
    /// 들어갈 수 있는지 확인
    /// </summary>
    public void Click_Choice() {
        if(m_LockObj.activeSelf) {
            return;
        }

        if (m_ClearObj.activeSelf ||
            PlayerMgr.Instance.UseResource(PaymentType.RebirthStone, m_EpisodeCost, false)) {
            TransitionUIMgr.Instance.FadeOut(MoveEpisode, 0.5f, true);
            PrefsMgr.Instance.SetEpisodeClear(m_SelectNum);
        }
        else {
            NoneTouchMsg.Instance.ShowText("Don't have enough Rebirth coins to enter.");
        }
    }

    private void MoveEpisode() {
        EpisodeSelectMgr.Instance.SetEpisodeStory(m_SelectNum);

        SystemMgr.Instance.MoveScene("Episode");
    }


}
