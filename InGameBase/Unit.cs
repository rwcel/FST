using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Flags]
public enum GetBuffAbilityFlag
{
	Nothing = 0,
	PassiveSkill = 1 << 0,
	ActiveSkill = 1 << 1,
	PartyEffect = 1 << 2, 
    PlayerSkill = 1 << 3,
	All = PassiveSkill | ActiveSkill | PartyEffect | PlayerSkill
}

public enum AbilityType : int
{
    Unknown = -1,
    DamageRate = 0,						// 피해량 %증가              :: 데미지
    HealRate = 1,						// 공격력의 %만큼 체력회복    :: 힐
    AddAttackRate = 2,					// 공격력 %증가
    AddDefenseRate = 3,					// 방어력 %증가
    AddHpRate = 4,						// 체력 %증가
    AddCriticalRate = 5,				// 크리티컬 확률 %증가
    AddCriticalAttackRate = 6,			// 크리티컬 데미지 %증가
    AddHitRate = 7,						// 명중률 %증가
    AddAvoidRate = 8,					// 회피율 %증가
    AddPenerationRate = 9,				// 방어관통 %증가
    AddAttackValue = 10,				// 공격력 절대값 증가
    AddDefenseValue = 11,				// 방어력 절대값 증가
    AddHpValue = 12,					// 체력 절대값 증가
    AddResStoneRate = 13,				// 환생석 획득 %증가
    ReduceFairyLvUpCostRate = 14,		// 정령 레벨업 비용 %감소
    ReduceFairyTimeRate = 15,			// 퀘스트 소요시간 %감소
    AddGetCombatGoldRate = 16,			// 적처치시 추가 골드%
    ReduceHeroLvUpCostRate = 17,		// 영웅 레벨업 비용 %감소
    ReduceSkillDelayRate = 18,			// 스킬 딜레이 %감소
    AddGetFairyGoldRate = 19,			// 퀘스트 완료 획득골드 %증가
    ReduceColosseumWaitTimeRate = 20,	// 콜로세움 대기시간 %감소
    Provocation = 21,					// 도발
    RebirthFairyLV = 22,                // 환생 시 정령 레벨 초기값 증가
    RebirthStage = 23,                  // 환생 시 스테이지 초기값 %증가
    JumpingBoss = 24,                   // 스테이지 클리어시 보스 스테이지로 넘어갈확률 %증가
    Count,
}

public class Unit : MonoBehaviour 
{      
	protected int _LV;              // 레벨
	protected int _Grade;           // 승급 
	protected int _Awaken;          // 각성 (별) - [0 : 1성 캐릭터]
	protected int _Race;            // 종족 : 인간, 엘프, 천사, 마족, 수인족, 용족
	protected int _Type;            // 타입 : 공격, 방어, 지원

	protected double _MaxHP;		// 최대 체력
	protected double _RemainHP;		// 남은 체력
	protected double _ATK;			// 공격력
	protected double _DEF;			// 방어력
	protected float _Range;			// 사정거리
	protected int _Critical;		// 크리티컬 확률
	protected int _CriticalAtk;		// 크리티컬 데미지
	protected int _HitRate;			// 명중률
	protected int _Avoid;			// 회피확률
	protected double _Peneration;   // 방어관통


    [HideInInspector] public int _AttackEffectID;         // 공격 ID
    protected double _AttackCool;       // 

    [HideInInspector] public int _SkillEffectID;          // 스킬 번호
    [HideInInspector] public double _SkillCool;           // 스킬쿨타임


    // ** Hero만 사용하는것은 HeroMgr로 내리기
	public int[,] _PassiveBuff = new int[4, (int)AbilityType.Count]; // 캐릭터 패시브 스킬만 적용
	protected Dictionary<int, float> _ActiveBuffTime = new Dictionary<int, float>();
	protected int[] _ActiveBuff = new int[(int)AbilityType.Count];

    [HideInInspector] public int _SlotNum;  // 슬롯에 영웅의 번호  0~3  **Enemy도 0번인데 충돌 문제 없나?
    [HideInInspector] public int _TableNum; // 테이블에 등록된 영웅의 번호 (Hero.csv 내 IDX번호)

    [HideInInspector] public BattleCharacter _BattleCharacter;

    protected GameObject _Obj;

    protected CSVMgr        m_CSVMgr;
    protected UnitMgr       m_UnitMgr;
    protected PlayerMgr     m_PlayerMgr;


    #region default functions

    private void Start() {
        m_CSVMgr  = CSVMgr.Instance;
        m_UnitMgr = UnitMgr.Instance;
        m_PlayerMgr = PlayerMgr.Instance;

        _BattleCharacter = GetComponent<BattleCharacter>();


        StopCoroutine("CheckBuff");
        StartCoroutine("CheckBuff");
    }

    private void OnDisable() {
        StopCoroutine("CheckBuff");
    }

    IEnumerable CheckBuff() {
        WaitForSeconds waitTerm = new WaitForSeconds(1.0f);

        while(true) {
            CheckActiveBuff();
            yield return waitTerm;
        }
    }

    #endregion


    #region property functions

    public int GetTableID() {
        return _TableNum;
    }

    public int GetLV() {
        if (_SlotNum < 0 || _TableNum < 0)
            return -1;
        return _LV;
    }

    public int GetGrade() {
        return _Grade;
    }

    public float GetGradeRate() {
        if (_Grade < 0)
            return 0;
        return (_Grade * (_Grade + 1) * 50) / 1000.0f;
    }

    public float GetAwaken() {
        return _Awaken;
    }

    public float GetAttackRange()     // **Range대신 새로 테이블에 들어오는 값 넣어야함
    {
        return _Range;
    }

    public virtual double GetMaxHP() {
        return System.Math.Round((_MaxHP * (1 + GetGradeRate())) + GetAddedHP());
    }

    public double GetRemainHP() {
        // 체력 증가버프 때문에 항상 Added와 함께 계산한다.
        return _RemainHP + GetAddedHP();
    }

    public double GetAddedHP() {
        // 액티브 버프효과로 추가된 HP
        double _SkillValue = GetTotalBuffAbilityValue(AbilityType.AddHpValue, GetBuffAbilityFlag.ActiveSkill);  // 절대값
        double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddHpRate, GetBuffAbilityFlag.ActiveSkill);     // 천분율
        return _SkillValue + System.Math.Max(_MaxHP * (1 + _SkillRate) - _MaxHP, 0);
    }

    #endregion property functions


    #region virtual functions 

    public virtual void SetStatus() {
        if(m_CSVMgr == null) {
            m_CSVMgr = CSVMgr.Instance;
        }
        if (m_PlayerMgr == null) {
            m_PlayerMgr = PlayerMgr.Instance;
        }
    }


    public virtual int GetSkillCoolSec() {
        return (int)(_SkillCool - SystemMgr.Instance.GetTimestamp());
    }

    public virtual float GetSkillCoolPercent() {
        double _Delay = CSVMgr.Instance.GetSkillCoolDown(_SkillEffectID);
        return (float)((_SkillCool - SystemMgr.Instance.GetTimestamp()) / _Delay);
    }

    public virtual void ApplySkillCool(float _Percent = 1.0f) {
        double _Delay = CSVMgr.Instance.GetSkillCoolDown(_SkillEffectID);
        _SkillCool = SystemMgr.Instance.GetTimestamp() + (_Delay * _Percent);
    }

    public virtual void ApplyAutoSkillCool(float _Percent = 1.0f) {
        double _Delay = CSVMgr.Instance.GetSkillCoolDown(_AttackEffectID);
        _AttackCool = SystemMgr.Instance.GetTimestamp() + (_Delay * _Percent);
    }

    #endregion virtual functions 



    #region Buff

    /// <summary> 해당버프가 적용되어 있는지 확인 </summary>
    public bool GetApplyBuff(AbilityType _AbilityType)
	{
		if (_ActiveBuff[(int)_AbilityType] != 0)
			return true;
		if (_PassiveBuff[_SlotNum, (int)_AbilityType] != 0)
			return true;
		return false;
	}

	protected void ResetActiveBuff()
	{
		_ActiveBuffTime.Clear();
		for (int i = 0; i < _ActiveBuff.Length; ++i)
			_ActiveBuff[i] = 0;
	}

	public void SetActiveBuff(int _SkillID, float _DurationTime)
	{
		if (_SkillID < 0)
			return;

		if (_ActiveBuffTime.ContainsKey(_SkillID) == true)
			_ActiveBuffTime[_SkillID] += _DurationTime;
		else
			_ActiveBuffTime.Add(_SkillID, _DurationTime);

		ApplyActiveBuff();
	}

	protected void CheckActiveBuff()
	{
        if (_ActiveBuffTime.Count <= 0 || GetRemainHP() <= 0) {
            return;
        }

		bool _Reapply = false;
		List<int> _Keys = new List<int>(_ActiveBuffTime.Keys);
		for (int i = 0; i < _Keys.Count; ++i)
		{
			if (_ActiveBuffTime[_Keys[i]] > 0.0f)
				_ActiveBuffTime[_Keys[i]] -= Time.deltaTime;
			else
			{
				_ActiveBuffTime[_Keys[i]] = 0.0f;
				_Reapply = true;
			}
		}
		if (_Reapply == true)
			ApplyActiveBuff();
	}

    // 모든 버프 초기화하고 버프 남은거 활성화
	protected void ApplyActiveBuff()
	{
		double _HPBeforeBuff = GetRemainHP();

		for (int i = 0; i < _ActiveBuff.Length; ++i)
			_ActiveBuff[i] = 0;
		if (_ActiveBuffTime.Count > 0)
		{
			foreach (var _KVP in _ActiveBuffTime)
			{
				if (_KVP.Value <= 0.0f)
					continue;
				for (int i = 0; i < CSVMgr.Instance.GetSkillEffectTypeCount(_KVP.Key); ++i)
				{
					AbilityType _Type = CSVMgr.Instance.GetSkillEffectType(_KVP.Key, i);
					int _Value = CSVMgr.Instance.GetSkillEffectValue(_KVP.Key, i);
					if (_Type != AbilityType.Unknown)
						_ActiveBuff[(int)_Type] = _Value;
				}
			}
		}

		// 체력증가 버프 풀도 남은 체력이 줄어들지 않게 한다.
		// 체력 감소 디버프 나오면 어떻게 처리할 때 다시 고민필요
		if (_HPBeforeBuff > GetRemainHP())
			_RemainHP = _HPBeforeBuff;

        // 체력증가 버프 풀려서 체력이 0이면 사망
        if (GetRemainHP() <= 0)
            DeadStart();
    }

    /// <summary>
    /// 적용된 모든 버프효과 값을 절대값으로 가져온다
    /// 비트연산으로 진행
    /// </summary>
    public virtual float GetTotalBuffAbilityValue(AbilityType _AbilityType, GetBuffAbilityFlag _Flags)
	{
		float _Total = 0.0f;
		if ((_Flags & GetBuffAbilityFlag.PassiveSkill) != 0)
		{
			// 캐릭터 패시브 스킬 추가
			for (int i = 0; i < _PassiveBuff.GetLength(0); ++i)
				_Total += _PassiveBuff[i, (int)_AbilityType];
		}
		if ((_Flags & GetBuffAbilityFlag.ActiveSkill) != 0)
		{
			// 캐릭터 액티브 스킬 추가
			_Total += _ActiveBuff[(int)_AbilityType];
		}
		if ((_Flags & GetBuffAbilityFlag.PartyEffect) != 0)
		{
			// 파티 스킬 추가
            for (int i = 0; i < PlayerMgr.Instance._PartyBuff.Count; ++i)
            {
                _Total += PlayerMgr.Instance._PartyBuff[i][(int)_AbilityType];
            }
		}
		if ((_Flags & GetBuffAbilityFlag.PlayerSkill) != 0)
		{
			// 플레이어 스킬 추가
			_Total += PlayerMgr.Instance.GetPlayerSkillBuff(_AbilityType); 
		}
		return _Total;
	}

	/// <summary> 적용된 모든 버프효과 값을 천분율로 가져온다 </summary>
	public float GetTotalBuffAbilityRate(AbilityType _AbilityType, GetBuffAbilityFlag _Flags)
	{
		return GetTotalBuffAbilityValue(_AbilityType, _Flags) / 1000.0f;
	}

    #endregion Buff

    #region Damage

    public virtual double GetNormalAttackDamage() { return 0.0f; }

    public virtual double GetDefense() { return 0.0f; }

    public virtual bool GetIsCritical() { return false;}

    public virtual float GetCriticalAtkPercent() { return 0f; }

    #endregion Damage

    #region Dead

    public void DeadStart() {
        if (UnitMgr.Instance.GetActiveSkill() == this) {
            UnitMgr.Instance.SetActiveSkill(null);

            // SystemMgr.Instance.SetLayer(gameObject, "Default", true); // 디폴트 레이어로 이동
        }

        Dead();
    }

    protected virtual void Dead() { }

    #endregion Dead

}
