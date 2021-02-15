using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class HeroBookMgr : ScrollPopup<HeroBookMgr>
{
    [HideInInspector] public List<int> _HeroList;

    private RaceType _Race;
    private ClassType _Class;   // 필터로 적용
    private bool isClear;        // 한번 열렸다면 닫기전까지 All로 초기화하지않음
   

    private List<GameObject> _HeroViewItem = new List<GameObject>();
    private GameObject _Obj;

    private WaitForEndOfFrame waitTerm;

    public void Show() {
        base.Open();

        _HeroList = new List<int>();

        waitTerm = new WaitForEndOfFrame();

        // PartyUIMgr.Instance.VisibleAction += () => { ListClear(); };

        SetScreen();
    }

    public void Close() {
        _HeroList.Clear();

        base.Close();
    }


    public void SetScreen()
    {   
        if(!isClear) {
            isClear = true;

            _Race = RaceType.All;
            _Class = ClassType.All;

            //CreatePrefab
        }

        GetList();

    }
    
    void GetList()
    {
        _HeroList.Clear();

        for (int i = 0, length = CSVMgr.Instance.GetHeroCnt(); i < length; i++)
        {
            if (CSVMgr.Instance.GetAwakenItem(i) < 0)   // 존재안함
                continue;
            if ((_Class == ClassType.All || CSVMgr.Instance.GetType(i) == (int)_Class)  // 해당항목 넣기
             && (_Race == RaceType.All || CSVMgr.Instance.GetRace(i) == (int)_Race))
            {
                _HeroList.Add(i);
            }
        }
    }

    public int GetListCnt()
    {
        return _HeroList.Count;
    }

    public int GetListHero(int _Cnt)
    {
        return _HeroList[_Cnt];
    }

    void ListClear() {
        // active - false
        //for (int i = 0; i < _HeroViewItem.Count; i++) {
        //    Destroy(_HeroViewItem[i]);
        //}

        // **
        // PlayerMgr.Instance.CheckHeroClear();

        _Race = RaceType.All;
        _Class = ClassType.All;
        isClear = false;

        _HeroViewItem.Clear();
    }

    public class HeroCompare : IComparer<int>
    {
        public HeroSortingType _Sorting = 0;

        private int _Result;

        // HeroListMgr.Instance.HasHeroInParty(_HeroNum) : 파티에 있는거 먼저?

        public int Compare(int x, int y) {
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
            else // if (_Sorting == HeroSorting.Level) {
                _Result = PlayerMgr.Instance.GetHeroLv(y).CompareTo(PlayerMgr.Instance.GetHeroLv(x));

            if (_Result == 0) {
                _Result = x.CompareTo(y);
            }

            return _Result;
        }
    }

    public void Sort(HeroSortingType sort) {
        HeroCompare _HC = new HeroCompare();
        _HC._Sorting = sort;
        _HeroList.Sort(_HC);

        SetList();
    }

    public void SetList(bool isReset = true) {
        _DynamicScroll[0].SetCnt(_HeroList.Count);
        _DynamicScroll[0].SetList();
        if(isReset) {
            _DynamicScroll[0].PositionReset();
        }
    }

    /// <summary>
    /// 필터 적용
    /// </summary>
    /// ** All이 있기때문에 하나씩 밀려야함
    /// <param name="race">종족</param>
    /// <param name="type">클래스</param>
    public void AllowFilter(int race, int type) {
        if (race < (int)RaceType.Kemono) {
            _Race = (RaceType)(race - 1);
        }
        else {
            _Race = (RaceType)(race);
        }
        _Class = (ClassType)(type-1);

        GetList();
        SetList();
    }
}
