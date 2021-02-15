using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroPartyMgr : EnableUIMgr<HeroPartyMgr>
{
    public HeroPartyType            _PartyType { get; private set; }

    [SerializeField] Button         m_Confirm;
    [SerializeField] Text           m_ConfirmText;
    [SerializeField] Text           m_HeroListText;

    [SerializeField] Transform      m_PartySlotParent;

    private int[]                   m_HeroSlotNums;
    private int[]                   m_TmpHeroSlot;              // **m_PartyItem에 적용을 안해도 변경이 되기때문에 임시 슬롯을 만들어줘야합니다.
    private int                     m_SelectListItem = -1;

    [Header("Other Scripts")]
    [SerializeField] ListItems      m_ListItems;

    private PartyItem[]             m_PartyItems;
    // private List<ListItems>         m_HeroListItems;

    private bool                    m_Allow = true;

    private bool m_IsChange;
    public bool _IsChange {
        get { return m_IsChange; }
        set {
            if (value == m_IsChange) {
                return;
            }

            m_IsChange = value;
            m_Confirm.interactable = !m_IsChange;
        }
    }

    protected override void SetData() {
        m_PartyItems = new PartyItem[m_PartySlotParent.childCount];
        for (int i = 0, length = m_PartyItems.Length; i < length; i++) {
            m_PartyItems[i] = m_PartySlotParent.GetChild(i).GetComponent<PartyItem>();
        }
    }

    protected override void Show() {
        Show();
    }


    public void Show(HeroPartyType partyType = HeroPartyType.InGame) {
        _IsChange = false;
        _PartyType = partyType;
        _Race = RaceType.All;
        _Class = ClassType.All;

        m_ConfirmText.text = CSVMgr.Instance.GetLanguage(534);
        m_HeroListText.text = "Heroes List";

        PartyUIMgr.Instance._OnClose += () => { PlayerMgr.Instance.CheckHeroClear(); m_Allow = false; };

        SetScreen();
        Refresh();
    }

    private void SetScreen() { 
        if(m_Allow) {
            if (_PartyType == HeroPartyType.InGame) {
                m_HeroSlotNums = PlayerMgr.Instance.GetHeroSlots();
                m_Confirm.onClick.AddListener(() => { AllowParty(); m_Allow = true; });
            }
            else if (_PartyType == HeroPartyType.Tower) {
                m_HeroSlotNums = PlayerMgr.Instance.GetTowerHeroSlots();
                m_Confirm.onClick.AddListener(() => { AllowTowerParty(); m_Allow = true; });
            }
            else if (_PartyType == HeroPartyType.Colosseum) {
                m_HeroSlotNums = PlayerMgr.Instance.GetColosseumHeroSlots();
                m_Confirm.onClick.AddListener(() => { AllowColosseumParty(); m_Allow = true; });
            }
        }

        m_TmpHeroSlot = new int[m_HeroSlotNums.Length];
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            m_TmpHeroSlot[i] = m_HeroSlotNums[i];
        }

        for (int i = 0, length = m_HeroSlotNums.Length; i < length; i++) {
            m_PartyItems[i].SetItem(i, m_HeroSlotNums[i]);
        }

        // 인게임파티에서만 레벨 보여주기
        //for (int i = 0, length = m_PartyItem.Length; i < length; i++) {
        //    m_PartyItem[i].SetItem(i, _HeroSlot[i]);
        //    m_PartyItem[i].LevelOnOff(_PartyType == HeroPartyType.InGame);
        //}
    }

    #region interact

    public void Click_Confirm() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        PartyUIMgr.Instance.Click_Close();
    }

    #endregion


    #region Party

    /// <summary> 현재 상태에 맞춰 오픈하기 / 리스트 변경 : 클릭했다면 </summary>
    public void SelectSlotItem(int slotNum = -1, bool isClick = false) {
        _IsChange = !_IsChange;

        bool isinParty;
        if(slotNum == -1) {
            isinParty = false;
        }
        else {
            isinParty = HasHeroInParty(_HeroList[slotNum]);
        }

        for (int i = 0, length = m_PartyItems.Length; i < length; i++) {
            m_PartyItems[i].SetSelectObj(_IsChange, slotNum, isClick, isinParty);
        }

        if(isClick) {
            if(!_IsChange) {
                SelectListItem(-1);
                return;
            }

            List<GameObject> heroitems = m_ListItems.GetList();
            HeroPartyListItem item;
            for (int i = 0, length = heroitems.Count; i < length; i++) {
                item = heroitems[i].GetComponent<HeroPartyListItem>();
                if (item.GetKey() == m_PartyItems[slotNum]._HeroNum) {
                    SelectListItem(i);
                    break;
                }
            }
        }
    }

    /// <summary> 전에 선택했던거 변환, 현재 선택 표시 / 클릭했다면 파티 찾아서 바꾸기 </summary>
    public void SelectListItem(int listNum, bool isClick = false) {
        HeroPartyListItem item = m_ListItems.GetSelectType() as HeroPartyListItem;
        if (item != null) {
            item.SetSelectObj(false);
            if(item.GetArrayNum() == listNum) {       // 같은 번호라면 선택 진행 안함
                listNum = -1;
            }
        }
        m_ListItems.SetSelectItem(listNum);
        if (listNum != -1) {
            item = m_ListItems.GetSelectType() as HeroPartyListItem;
            item.SetSelectObj(true);
        }
        m_SelectListItem = item.GetKey();

        if (isClick) {                              // Party 변경
            if(_IsChange && listNum != -1) {        // 버튼만 변경
                int slotNum = -1;
                for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
                    if (item.GetKey() == m_PartyItems[i]._HeroNum) {
                        slotNum = i;
                        break;
                    }
                }
                for (int i = 0, length = m_PartyItems.Length; i < length; i++) {
                    m_PartyItems[i].SetSelectObj(_IsChange, slotNum, false, slotNum != -1);
                }
            }
            else {
                for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
                    if (item.GetKey() == m_PartyItems[i]._HeroNum) {
                        SelectSlotItem(i);
                        return;
                    }
                }
                SelectSlotItem();
            }
        }
    }

    /// <summary>
    /// 영웅 파티에 있는지 확인r
    /// </summary>
    public bool HasHeroInParty(int _Num) {
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            if (m_TmpHeroSlot[i] == _Num) {
                return true;
            }
        }
        return false;
    }

    public void AddHero(int slotNum) {
        m_TmpHeroSlot[slotNum] = m_SelectListItem;

        PartySort();
        Refresh();
    }

    public void RemoveHero(int slotNum) {
        int count = m_TmpHeroSlot.Length;
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            if (m_TmpHeroSlot[i] == -1) {
                count--;
            }
        }
        if(count > 1) {
            m_TmpHeroSlot[slotNum] = -1;
        }

        PartySort();
        Refresh();
    }

    /// <summary>
    /// 영웅변경 -> List재배열, Sprite 끄기, 교체버튼 비활성화
    /// </summary>
    public void ChangeHero(int slotNum)
    {
        bool flag = false;
        // 겹치면 위치 변경
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            if (m_TmpHeroSlot[i] == m_SelectListItem) {
                int tmp = m_TmpHeroSlot[slotNum];
                m_TmpHeroSlot[slotNum] = m_TmpHeroSlot[i];
                m_TmpHeroSlot[i] = tmp;
                flag = true;
                break;
            }
        }

        if(!flag) {
            m_TmpHeroSlot[slotNum] = m_SelectListItem;
        }

        PartySort();
        Refresh();
    }

    /// <summary>
    /// 파티 확정 : 닫기
    /// </summary>
    private void AllowParty()
    {
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            PlayerMgr.Instance.SetHeroSlot(i, m_TmpHeroSlot[i]);
        }

        PlayerMgr.Instance.CheckTeamColor();
        NetworkMgr.Instance.UpdatePlayerHeroSlot();

        MyInfo.Instance.UpdateProfile();

        // 로비 캐릭터 변경하기
        TransitionUIMgr.Instance.FadeOut(() => SystemMgr.Instance.MoveScene("Lobby"), 0.5f, true);
    }

    // **미변경
    public void AllowTowerParty()
    {
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            if(m_TmpHeroSlot[i] < 0) {
                return;
            }
        }

        for (int i = 0, length = UnitMgr.Instance.GetHeroMgrCnt(); i < length; i++) {
            PlayerMgr.Instance.SetTowerHeroSlot(i, m_TmpHeroSlot[i]);
        }

        PlayerMgr.Instance.CheckTowerTeamColor();
        NetworkMgr.Instance.UpdatePlayerTowerHeroSlot();
        TowerInfoMgr.Instance.SetScreen();
    }

    // **미변경
    public void AllowColosseumParty() {
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            if (m_TmpHeroSlot[i] < 0) {
                return;
            }
        }
        //SelectNumAndEffect();

        for (int i = 0, length = UnitMgr.Instance.GetHeroMgrCnt(); i < length; i++) {
            PlayerMgr.Instance.SetColosseumHeroSlot(i, m_TmpHeroSlot[i]);
        }

        PlayerMgr.Instance.CheckColosseumTeamColor();
        NetworkMgr.Instance.UpdateColosseum();
        ColosseumInfoMgr.Instance.SetScreen();
    }

    /// <summary>
    /// 영웅 내에서 진화했을 때 PartyItem에 적용시키게 하기
    /// </summary>
    public void CheckInParty(int heroNum) {
        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            if (m_TmpHeroSlot[i] == heroNum) {
                m_PartyItems[i].SetItem(i, heroNum);
                return;
            }
        }
    }

    #endregion Party

    #region HeroList

    public UIPopupList _SortPopup;

    [HideInInspector] public List<int> _HeroList = new List<int>();

    private ClassType _Class;
    private RaceType _Race;
    private HeroSortingType _Sort;

    // **필터 관련 함수 수정 X
    void SetSortPopup()
    {
        _SortPopup.Clear();
        _SortPopup.AddItem(CSVMgr.Instance.GetLanguage(523));
        _SortPopup.AddItem(CSVMgr.Instance.GetLanguage(524));
        _SortPopup.AddItem(CSVMgr.Instance.GetLanguage(525));

        _SortPopup.value = CSVMgr.Instance.GetLanguage(523);        // 자동 1번 선택

		Refresh();
    }

    private void PartySort() {
        for (int i = 0, length = m_TmpHeroSlot.Length - 1; i < length; i++) {
            for (int j = 0, length2 = m_TmpHeroSlot.Length - 1 - i; j < length2; j++) {
                if (m_TmpHeroSlot[j] < 0) {
                    m_TmpHeroSlot[j] = m_TmpHeroSlot[j + 1];
                    m_TmpHeroSlot[j + 1] = -1;
                }
            }
        }

        for (int i = 0, length = m_TmpHeroSlot.Length; i < length; i++) {
            m_PartyItems[i].SetItem(i, m_TmpHeroSlot[i]);
        }

        _IsChange = false;
        m_ListItems.SetSelectItem(-1);
    }

    /// <summary>
    /// On -> Off
    /// 리스트 재배열
    /// </summary>
    public void Refresh()
	{
        _IsChange = false;

        GetList();
        Sorting();
    }

    /// <summary>
    /// 리스트 새로 넣기
    /// **필터 부분 주석 처리
    /// </summary>
    void GetList()
    {
        _HeroList.Clear();

        for (int i = 0, length = CSVMgr.Instance.GetHeroCnt(); i < length; i++)
        {
            if(PlayerMgr.Instance.GetHeroLv(i) >= 0) {
                _HeroList.Add(i);
            }
            //if (PlayerMgr.Instance.GetHeroLv(i) < 0) {
            //    continue;
            //}
            //if ((_Class == ClassType.All || CSVMgr.Instance.GetType(i) == (int)_Class)
            // && (_Race == RaceType.All || CSVMgr.Instance.GetRace(i) == (int)_Race))
            //{
            //    _HeroList.Add(i);
            //}
        }
    }

    public class HeroCompare : IComparer<int>
    {
        public HeroSortingType _Sorting = 0;

        private int _Result;

        /// <summary>
        /// 0순위 파티 / 1순위 선택한 필터
        /// </summary>
        public int Compare(int x, int y) {
            _Result = Instance.HasHeroInParty(y).CompareTo(Instance.HasHeroInParty(x));

            if (_Sorting == HeroSortingType.Awaken) {
                _Result = PlayerMgr.Instance.GetHeroAwaken(y).CompareTo(PlayerMgr.Instance.GetHeroAwaken(x));

                if (_Result == 0) {
                    _Result = PlayerMgr.Instance.GetHeroLv(y).CompareTo(PlayerMgr.Instance.GetHeroLv(x));
                }

                if (_Result == 0) {
                    _Result = CSVMgr.Instance.GetGrade(y).CompareTo(CSVMgr.Instance.GetGrade(x));
                }

                if (_Result == 0) {
                    _Result = x.CompareTo(y);
                }

                return _Result;
            }
            else if (_Sorting == HeroSortingType.Grade) {
                _Result = PlayerMgr.Instance.GetHeroGrade(y).CompareTo(PlayerMgr.Instance.GetHeroGrade(x));

                if (_Result == 0) {
                    _Result = PlayerMgr.Instance.GetHeroLv(y).CompareTo(PlayerMgr.Instance.GetHeroLv(x));
                }

                if (_Result == 0) {
                    _Result = CSVMgr.Instance.GetAwaken(y).CompareTo(CSVMgr.Instance.GetAwaken(x));
                }

                if (_Result == 0) {
                    _Result = x.CompareTo(y);
                }

                return _Result;
            }
            else {  // if (_Sorting == HeroSorting.Level) 
                _Result = PlayerMgr.Instance.GetHeroLv(y).CompareTo(PlayerMgr.Instance.GetHeroLv(x));
            }

            if (_Result == 0) {
                _Result = x.CompareTo(y);
            }

            return _Result;
        }
    }

    void Sorting()
    {
        HeroCompare _HC = new HeroCompare();
        _HC._Sorting = (HeroSortingType)_Sort;
        _HeroList.Sort(_HC);

        SetList();
    }

    public void SetList()
    {
        m_ListItems.SetItem(_HeroList.Count);
    }

    /// <summary>
    /// 팝업 밸류 변경
    /// </summary>
    public void Change_Sort() { 
        if (_SortPopup.value == CSVMgr.Instance.GetLanguage(523))
            _Sort = HeroSortingType.Level;
        else if (_SortPopup.value == CSVMgr.Instance.GetLanguage(524))
            _Sort = HeroSortingType.Grade;
        else if (_SortPopup.value == CSVMgr.Instance.GetLanguage(525))
            _Sort = HeroSortingType.Awaken;

        GetList();
        Sorting();

        HeroBookMgr.Instance.Sort(_Sort);

        // 선택된게 있었다면 다시 선택할 수 있게 바꿔줘야함
        //if (m_SelectHeroNum != -1) {       
        //    ChangeHeroListData();
        //}

    }

    // OnOff

    /// <summary>
    /// 영웅 리스트 필터
    /// 0번이 ALL
    /// </summary>
    /// <param name="race"></param>
    /// <param name="type"></param>
    public void AllowFilter(int race, int type) {
        if (race < (int)RaceType.Kemono) {
            _Race = (RaceType)(race - 1);
        }
        else {
            _Race = (RaceType)(race);
        }
        _Class = (ClassType)(type - 1);

        Debug.Log(_Race+","+_Class);

        GetList();
        Sorting();

        // 선택된게 있었다면 다시 선택할 수 있게 바꿔줘야함
        //if(m_SelectHeroNum != -1) {
        //    ChangeHeroListData();
        //}
    }


    #endregion HeroList
}
