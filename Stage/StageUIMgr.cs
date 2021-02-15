using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class StageUIMgr : SingleTon<StageUIMgr>
{
    #region type def

    private enum Texts
    {
        GoldAmount,
        StageSkipTicket,
        Item_SpeedUp,
        Item_SpeedUp_Time,
        Item_Auto_Time,
    }

    private enum Objects
    {
        GoldIcon,
        Rebirth,
        StageSkipTicket,
    }

    private enum LevelUps
    {
        Max, Ten, One
    }

    private class Artifacts
    {
        public Transform _Tr;

        public Image _Image;
        public string _Name;
        public string _Desc;

        public Artifacts(Transform tr) {
            _Tr = tr;
            _Image = _Tr.GetChild(0).GetComponent<Image>();
            _Name = null;
            _Desc = null;
        }
    }

    #endregion // type def



    #region serialized fields

    [SerializeField] Image                  m_BackgroundImage = null;
    [SerializeField] Text[]                 m_Texts = null;
    [SerializeField] GameObject[]           m_Objects = null;

    [SerializeField] Image                  m_ItemSpeedUpImage = null;
    [SerializeField] Image                  m_ItemAutoImage = null;

    [SerializeField] Transform              m_ArtifactParent = null;
    [SerializeField] Artifacts[]            m_Artifacts;

    [Header("OtherScripts")]
    [SerializeField] Transform              m_HeroBtnParent;
    [SerializeField] GoldLevel              m_GoldLevel;
    [SerializeField] SlideTab               m_SlideTab;

    #endregion // serialized fields
    

    #region private varialbes

    private HeroBtn[]                       m_HeroBtns;
    private double                          m_GoldAmount;
    private PlayerMgr                       m_PlayerMgr = null;
    private SystemMgr                       m_SystemMgr = null;

    private static readonly Color _Color_Off_Item = new Color(0.7f, 0.7f, 0.7f);

    #endregion private varialbes


    #region properties

    public HeroBtn GetHeroBtn(int num) { return m_HeroBtns[num]; }

    #endregion properties



    #region default functions

    protected override void Awake() {
        base.Awake();

        m_PlayerMgr = PlayerMgr.Instance;
        m_SystemMgr = SystemMgr.Instance;

        m_HeroBtns = new HeroBtn[m_HeroBtnParent.childCount];
        for (int i = 0, length = m_HeroBtns.Length; i < length; i++) {
            m_HeroBtns[i] = m_HeroBtnParent.GetChild(i).GetComponent<HeroBtn>();
        }
    }

    private void Start() {
        m_GoldAmount = 0;

        m_Artifacts = new Artifacts[m_ArtifactParent.childCount];
        for (int i = 0, length = m_ArtifactParent.childCount; i < length; i++) {
            m_Artifacts[i] = new Artifacts(m_ArtifactParent.GetChild(i));
        }

        m_SlideTab.SetButton(0, "MAX", Click_SetLevelMax);
        m_SlideTab.SetButton(1, "X10", Click_SetLevel10);
        m_SlideTab.SetButton(2, "X1", Click_SetLevel1);

        if (PrefsMgr.Instance._IsLvUpMax) {
            m_SlideTab.SetButton(0, "MAX", Click_SetLevelMax, true);
        }
        else if (PrefsMgr.Instance.GetUp() == 10) {
            m_SlideTab.SetButton(1, "X10", Click_SetLevel10, true);
        }
        else {
            m_SlideTab.SetButton(2, "X1", Click_SetLevel1,true);
        }

        SetScreen();
    }

    #endregion // default functions



    #region public functions

    /// <summary>
    /// 스테이지 내에서 값이 변할 수 있는 항목
    /// </summary>
    public void SetScreen() {
        ItemSetting();

        UpdateGoldValue();

        ArtifactSetting();
    }

    // $
    //public void SetNewbeeIcon(bool isActive) {
    //    objects[(int)Objects.Newbee].SetActive(isActive);
    //}

    /// <summary>
    /// 골드 업데이트
    /// * 먼저 실행될 가능성있기에 m_PlayerMgr 등 사용안함
    /// </summary>
    public void UpdateGoldValue() {
        m_GoldAmount = PlayerMgr.Instance.GetResource(PaymentType.Gold);
        m_Texts[(int)Texts.GoldAmount].text = SystemMgr.Instance.GetDoubleString(m_GoldAmount);

        for (int i = 0, length = m_HeroBtns.Length; i < length; i++) {
            m_HeroBtns[i].SetLevelUpCost();
        }
        m_GoldLevel.SetLevelUpCost();
    }

    #endregion // public functions

    #region interaction

    public void Click_Lobby() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        TransitionUIMgr.Instance.TransitionBox(MoveLobbyScene, Vector2.left, 1f, true);
    }

    private void MoveLobbyScene() {
        m_SystemMgr.MoveStageToLobby();
    }

    public void Click_GoldInfo() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        ToolTipMgr.Instance.Show(m_Objects[(int)Objects.GoldIcon].transform, 0, (int)PaymentType.Gold);
    }

    // 툴팁 보여주기
    public void Click_Artifact(int num) {
        ToolTipMgr.Instance.ShowString(m_Artifacts[num]._Tr, m_Artifacts[num]._Name, m_Artifacts[num]._Desc);
    }

    public void Click_Rebirth() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        TwoButtonMgr.Instance.Show(null, RebirthMgr.Instance.Show, 3, 2, 772, 774, TwoButtonPopupType.Rebirth);
    }

    /// <summary>
    /// ** 아이템 10개를 소모해 천계(N*100스테이지)로 가는 작업 아직 없습니다.
    /// </summary>
    public void Click_StorySkip() {
        // 사용 개수 체크 필요
        int count = 1;
        if (!PlayerMgr.Instance.UseResource(PaymentType.StorySkipTicket, count)) {
            return;
        }

        int _Stage = Mathf.CeilToInt((PlayerMgr.Instance.GetStageFull() + (count-1)*10 +1) * 0.1f) *10 - 1;

        PlayerMgr.Instance.SetStage(_Stage, true);

        TransitionUIMgr.Instance.FadeOut(() => SystemMgr.Instance.MoveScene("Boss"), 0.5f, true);
    }

    public void Click_Quest() {
        QuestMgr.Instance.Show();
    }

    public void Click_NewBee() {
        //ToolTipMgr.Instance.ShowLanguage(GetObjectPosition(Objects.Newbee),2538,2539);
    }

    public void Click_SetLevelMax() {
        PrefsMgr.Instance.SetUp(1, true);

        SetUpLevel((int)LevelUps.Max);
    }

    public void Click_SetLevel10() {
        PrefsMgr.Instance.SetUp(10);

        SetUpLevel((int)LevelUps.Ten);
    }

    public void Click_SetLevel1() {
        PrefsMgr.Instance.SetUp(1);

        SetUpLevel((int)LevelUps.One);
    }

    #endregion // interaction

    #region private functions

    private void ItemSetting() {
        double storySkipTicket = m_PlayerMgr.GetResource(PaymentType.StorySkipTicket);
        m_Texts[(int)Texts.StageSkipTicket].text = string.Format("{0:n0}", storySkipTicket);
        m_Objects[(int)Objects.StageSkipTicket].SetActive(storySkipTicket > 0 &&              // Animation?
                                                         SystemMgr.Instance.GetSceneName() == "Stage");
        m_Objects[(int)Objects.Rebirth].SetActive((m_PlayerMgr.GetStageFull() + 1) >= 51);

        // item
        if (m_PlayerMgr.GetPlaySpeed() > 1f || m_PlayerMgr.GetAutoSkill() > m_SystemMgr.GetTimestamp()) {
            StopCoroutine("UpdateContents");
            StartCoroutine("UpdateContents");
        }
        else {  // 기본 텍스트
            m_Texts[(int)Texts.Item_SpeedUp].text = "▶1x";
            m_Texts[(int)Texts.Item_SpeedUp_Time].text = "";
            m_Texts[(int)Texts.Item_Auto_Time].text = "";
        }
        m_ItemSpeedUpImage.color = m_PlayerMgr.GetPlaySpeed() > 1f ? Color.white : _Color_Off_Item;
        m_ItemAutoImage.color = m_PlayerMgr.GetAutoSkill() > m_SystemMgr.GetTimestamp() ? Color.white : _Color_Off_Item;
    }

    private void SetUpLevel(int selectNum) {
        ResourceMgr.Instance.PlaySound("FX", 0);

        for (int i = 0, length = m_HeroBtns.Length; i < length; i++) {
            m_HeroBtns[i].SetLevelUpCost();
        }
        m_GoldLevel.SetLevelUpCost();
    }

    /// <summary>
    /// **Don't Destroy로 EquipList를 가지고 있어야 합니다.
    /// </summary>
    private void ArtifactSetting() {
        if (EquipMgr.Instance != null) {        // **이쪽으로 들어오지 않아 많은 연산 강요
            int artifactIdx = -1;
            for (int i = 0, length = EquipMgr.Instance._EquipIDXList.Count; i < length; i++) {
                artifactIdx = EquipMgr.Instance._EquipIDXList[i];
                m_Artifacts[i]._Image.sprite = ResourceMgr.Instance.GetSprite("Artifact", m_PlayerMgr.GetArtifactType(artifactIdx));
                m_Artifacts[i]._Name = m_PlayerMgr.GetArtifactName(artifactIdx);
                m_Artifacts[i]._Desc = m_PlayerMgr.GetArtifactDescription(artifactIdx);
            }
        }
        else {
            int cnt = 0;
            for (int i = 0, length = PlayerMgr.Instance.GetArtifactCnt(); i < length; i++) {
                int _A_IDX = PlayerMgr.Instance.GetArtifactIDX(i);
                if (PlayerMgr.Instance.GetArtifactEquip(_A_IDX) > 0) {
                    int type = m_PlayerMgr.GetArtifactType(_A_IDX);
                    int enchant = m_PlayerMgr.GetArtifactEnchant(_A_IDX);
                    int awaken = m_PlayerMgr.GetArtifactAwaken(_A_IDX);

                    m_Artifacts[cnt]._Image.sprite = ResourceMgr.Instance.GetSprite("Artifact", type);
                    m_Artifacts[cnt]._Name = CSVMgr.Instance.GetArtifactName(type);
                    m_Artifacts[cnt]._Desc = CSVMgr.Instance.GetArtifactDescription(type, enchant, awaken);
                    cnt++;
                }
            }
        }

        for (int i = m_PlayerMgr.GetArtifactEquipCnt(), length = m_ArtifactParent.childCount; i < length; i++) {
            m_ArtifactParent.GetChild(i).gameObject.SetActive(false);
        }
    }


    // $ 퀘스트
    //private bool HasQuestReward() {
    //    int questCount = PlayerMgr.Instance.GetQuest();
    //    int targetAchieveValue = CSVMgr.Instance.GetQuestAchieveValue(questCount);
    //    int achieveType = CSVMgr.Instance.GetQuestAchieveType(questCount);
    //    double achieveValue = PlayerMgr.Instance.GetAchievementValue(2, achieveType);

    //    if (7 == achieveType || 11 == achieveType || 14 == achieveType) {
    //        achieveValue++;
    //    }

    //    return (achieveValue >= targetAchieveValue);
    //}

    /// <summary>
    /// 1분간격 아이템 확인
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateContents() {
        var waitTerm = new WaitForSeconds(60f);
        m_Texts[(int)Texts.Item_SpeedUp].text = string.Format("▶{0}x", m_PlayerMgr.GetPlaySpeed().ToString());
        bool flag = false;
        while (!flag) {
            m_Texts[(int)Texts.Item_SpeedUp_Time].text = m_SystemMgr.GetDateTime(m_PlayerMgr.GetPlaySpeedTime() - m_SystemMgr.GetTimestamp(), 1);
            m_Texts[(int)Texts.Item_Auto_Time].text = m_SystemMgr.GetDateTime(m_PlayerMgr.GetAutoSkill() - m_SystemMgr.GetTimestamp(), 1);

            // Debug.Log(m_PlayerMgr.GetPlaySpeedTime() + ","+ m_PlayerMgr.GetAutoSkill() + ","+ m_SystemMgr.GetTimestamp());

            //objects[(int)Objects.Alarm_Quest].SetActive(HasQuestReward());

            if (m_PlayerMgr.GetPlaySpeedTime() <= 1f && m_PlayerMgr.GetAutoSkill() <= 0f) {
                flag = true;
            }

            yield return waitTerm;
        }

        m_Texts[(int)Texts.Item_SpeedUp].text = "▶1x";
        m_Texts[(int)Texts.Item_SpeedUp_Time].text = "";
        m_Texts[(int)Texts.Item_Auto_Time].text = "";
        m_ItemSpeedUpImage.color = _Color_Off_Item;
        m_ItemAutoImage.color = _Color_Off_Item;
    }

    #endregion // private functions
}