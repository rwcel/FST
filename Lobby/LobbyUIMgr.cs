using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class LobbyUIMgr : SingleTon<LobbyUIMgr>
{
    #region type def

    public enum LobbySideMenus
    {
        Hep, Option, Shop, Rebirth, Rank
    }
    
    [System.Serializable]
    private struct SideMenu
    {
        public GameObject   Obj;
        //public Text         NameText;
        public GameObject   Alarm;
    }

    #endregion // type def

    public System.Action                    _OnMoveInGame;
    public float                            _WindowCharMinX = -450;             
    //public float                            _WindowCharMaxX = 400;             
    public float                            _WindowCharLimitY = 20;             // -20 ~ 20
    public float                            _CharRandomValue = 100;             // 캐릭터 랜덤하게 둘 간격
    public float                            _BetweenCharDistance = 250;         // 캐릭터당 떨어져 있는 위치


    #region serialized fields

    [SerializeField] private SideMenu[]     m_SideMenus;

    [SerializeField] private Animator       m_SideMenuAnim;
    [SerializeField] private GameObject     m_InGame;

    #endregion // serialized fields


    #region private fields

    private bool                            m_ShowSideMinus;

    private const string _Animation_SideOnOff = "ShowOnOff";

    #endregion


    #region properties
    public GameObject ButtonQuest { get { return m_SideMenus[(int)LobbySideMenus.Shop].Obj; } }    //**수정필요

    public void SetSideMenuAlarm(LobbySideMenus menuType, bool isActive) {
        m_SideMenus[(int)menuType].Alarm.SetActive(isActive);
    }

    #endregion // properties


    #region default functions

    private void Start() {
        SetScreen();
    }

    #endregion // default functions


    #region public functions

    public void SetScreen() {
        m_ShowSideMinus = false;

        // 에피소드에서 왔으면 화면 열기
        if (PrefsMgr.Instance.GetPrevScene() == "Episode") {
            EpisodeSelectUIMgr.Instance.Show();
        }
    }

    #endregion // public functions

     
    #region interaction

    #region BottomMenu

    public void Click_Stage() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        m_InGame.SetActive(false);
        StartCoroutine("WaitMoveScene");
        if (_OnMoveInGame != null) {
            _OnMoveInGame();
        }
    }

    private IEnumerator WaitMoveScene()
    {
        yield return new WaitForSeconds(2.0f);
        TransitionUIMgr.Instance.TransitionBox(
            SystemMgr.Instance.MoveInGame, Vector2.right, 2f, true);
    }

    public void Click_Episode() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        EpisodeSelectUIMgr.Instance.Show();
    }

    public void Click_Temple() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        // 
    }

    public void Click_Heroes() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        PartyUIMgr.Instance.Show();
    }

    public void Click_Inventory() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        InventoryMgr.Instance.Show();
    }

    public void Click_Challenge() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        ChallengeMgr.Instance.Show();
    }

    #endregion

    #region SideMenu

    /// <summary>
    /// 애니메이션 + 그 동안 터치 못하게
    /// </summary>
    public void Click_SideMenu() {
        m_ShowSideMinus = !m_ShowSideMinus;

        m_SideMenuAnim.SetBool(_Animation_SideOnOff, m_ShowSideMinus);
    }

    public void Click_Help() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //HeroInfoHelpMgr.Instance.Show();
        Click_SideMenu();
    }

    public void Click_Option() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //OptionMgr.Instance.Show();
        Click_SideMenu();
    }

    public void Click_Shop() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //StoreMgr.Instance.Show(StoreType.CashShop, (int)CashTabs.Dia);
        Click_SideMenu();
    }

    public void Click_Resurrection() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //ResurrectionMgr.Instance.Show();
        Click_SideMenu();
    }

    public void Click_Rank() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //RankingMgr.Instance.Show(RankingType.Stage);
        Click_SideMenu();
    }

    #endregion

    #region SubMenu

    public void Click_Mail() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //MailMgr.Instance.Show();
    }

    // 미션
    public void Click_Achievement() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //AchieveMgr.Instance.Show();
    }

    // 캘린더
    public void Click_Attendance() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();
        //AttendanceMgr.Instance.Show();
    }

    //public void Click_Quest() {
    //    ResourceMgr.Instance.PlaySound("FX", 0);

    //    QuestMgr.Instance.Show();
    //    Click_SideMenu();
    //}

    #endregion

    #endregion // interaction


    #region private functions

    private bool HasQuestReward() {
        int questCount = PlayerMgr.Instance.GetQuest();
        int targetAchieveValue = CSVMgr.Instance.GetQuestAchieveValue(questCount);
        int achieveType = CSVMgr.Instance.GetQuestAchieveType(questCount);
        double achieveValue = PlayerMgr.Instance.GetAchievementValue(2, achieveType);

        if (7 == achieveType || 11 == achieveType || 14 == achieveType) {
            achieveValue++;
        }

        return (achieveValue >= targetAchieveValue);
    }

    #endregion // private functions
}