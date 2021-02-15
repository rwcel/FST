using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMgr : SingleTon<UnitMgr>
{
    #region NPCMgr
    // **선언 타이밍 느려서 Awake에서 사용 불가
    public List<HeroMgr> _HeroMgr = new List<HeroMgr>();          // 영웅 캐릭 오브젝트풀 - 4개
    public List<EnemyMgr> _EnemyMgr = new List<EnemyMgr>();       // 적 캐릭 오브젝트풀 - 8개

    public int GetHeroMgrCnt()
    {
        return _HeroMgr.Count;
    }

    public int GetEnemyMgrCnt()
    {
        return _EnemyMgr.Count;
    }

    public HeroMgr GetHeroMgr(int _Num)
    {
        return _HeroMgr[_Num];
    }

    public EnemyMgr GetEnemyMgr(int _Num)
    {
        return _EnemyMgr[_Num];
    }

    #endregion NPCMgr
    
    #region NPCList
    
    private List<HeroMgr> _HeroList = new List<HeroMgr>();   // 활성화중인 영웅 캐릭 리스트
    [HideInInspector] public List<EnemyMgr> _EnemyList = new List<EnemyMgr>();// 활성화중인 적 캐릭 리스트
    
    public int GetHeroListCnt()
    {
        return _HeroList.Count;
    }

    public void AddHeroList(HeroMgr _HeroMgr)
    {
        if (!_HeroList.Contains(_HeroMgr))
        {
            _HeroList.Add(_HeroMgr);
        }
    }

    public void RemoveHeroList(HeroMgr _HeroMgr)
    {
        _HeroList.Remove(_HeroMgr);
    }

    public HeroMgr GetHeroList(int _Num)
    {
        return _HeroList[_Num];
    }

    public int GetEnemyListCnt()
    {
        return _EnemyList.Count;
    }

    public void AddEnemyList(EnemyMgr _EnemyMgr)
    {
        if (!_EnemyList.Contains(_EnemyMgr))
        {
            _EnemyList.Add(_EnemyMgr);
        }
    }

    public void RemoveEnemyList(EnemyMgr _EnemyMgr)
    {
        _EnemyList.Remove(_EnemyMgr);
    }

    public EnemyMgr GetEnemyList(int _Num)
    {
        return _EnemyList[_Num];
    }

    public void SetStatus(int _HeroNum)
    {
        HeroMgr _Mgr = _HeroList.Find(Hero => Hero._TableNum == _HeroNum);

        if (_Mgr != null)
        {
            _Mgr.SetStatus();
            _Mgr.InitHP(false);
        }
    }
    
    //커스텀정렬 알고리즘 : 가까운 영웅 찾기
    public class HeroListCompare : IComparer<HeroMgr>
    {
        public EnemyMgr _Enemy;

        // sort시 정렬하는 곳  **변수명 등 신경쓰기
        public int Compare(HeroMgr hero1, HeroMgr hero2)
        {
            bool _ApplyProvocationX = hero1.GetApplyBuff(AbilityType.Provocation);
            bool _ApplyProvocationY = hero2.GetApplyBuff(AbilityType.Provocation);
            if (_ApplyProvocationX && !_ApplyProvocationY)
                return -1;
            else if (!_ApplyProvocationX && _ApplyProvocationY)
                return 1;
            // 모두 도발을 시전하거나 아무도 도발을 시전하지 않았다면 가까운 상대(=영웅) 공격
            return Vector3.Distance(hero1.transform.position, _Enemy.transform.position).CompareTo(Vector3.Distance(hero2.transform.position, _Enemy.transform.position));
        }
    }

    public HeroMgr FindHero(int _PartyNum)
    {
        return _HeroList.Find(_Item => _Item._SlotNum == _PartyNum);
    }

    public HeroMgr FindHero(EnemyMgr _Enemy)
    {
        HeroListCompare hc = new HeroListCompare();
        hc._Enemy = _Enemy;

        _HeroList.Sort(hc);

        if (_HeroList.Count > 0)    // 정렬순위 가장 높은 값 반환
        {
            return _HeroList[0];
        }
        else
        {
            return null;
        }
    }
    
    public class EnemyListCompare : IComparer<EnemyMgr>
    {
        public HeroMgr _Hero;

        public int Compare(EnemyMgr x, EnemyMgr y)
        {
            bool _ApplyProvocationX = x.GetApplyBuff(AbilityType.Provocation);
            bool _ApplyProvocationY = y.GetApplyBuff(AbilityType.Provocation);
            if (_ApplyProvocationX && !_ApplyProvocationY)
                return -1;
            else if (!_ApplyProvocationX && _ApplyProvocationY)
                return 1;
            // 모두 도발을 시전하거나 아무도 도발을 시전하지 않았다면 가까운 상대 공격
            return Vector3.Distance(x.transform.position, _Hero.transform.position).CompareTo(Vector3.Distance(y.transform.position, _Hero.transform.position));
        }
    }

    public EnemyMgr FindEnemy(HeroMgr _Hero)
    {
        EnemyListCompare ec = new EnemyListCompare();
        ec._Hero = _Hero;

        _EnemyList.Sort(ec);

        if (_EnemyList.Count > 0)
        {
            return _EnemyList[0];
        }
        else
        {
            return null;
        }
    }

    #endregion NPCList

    #region ActiveSkill

    private Unit _ActiveSkill;
    
    public void SetActiveSkill(Unit _Unit)
    {
        _ActiveSkill = _Unit;
    }

    public Unit GetActiveSkill()
    {
        return _ActiveSkill;
    }

    #endregion ActiveSkill

    #region TempleSkill

    private bool _TempleSkillOn;

    public void SetTempleSkill(bool _OnOff)
    {
        _TempleSkillOn = _OnOff;
    }

    public bool GetTempleSkill()
    {
        return _TempleSkillOn;
    }

    #endregion TempleSkill
}
