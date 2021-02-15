using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroMgr : Unit
{
    #region etc
    public bool                             _CanStartStat { get; private set; }

    //[SerializeField] protected HeroBtn      m_HeroBtn;
    protected HeroBtn m_HeroBtn;

    private const string _AddStatScene = "Stage Boss";

	public float GetPlayerLvRate()
	{        
		if (m_PlayerMgr == null)
			return 0.0f;
		return m_PlayerMgr.GetLv() * 0.05f; // 플레이어 레벨의 5%
	}

    public override double GetMaxHP() // 힐, 버프효과 사라짐 처리 때문에 지속적으로 계산
    {
        double _GradeBuff = m_PlayerMgr.GetHeroGradeBuff() * 500;
        double _CollectionBuff = m_PlayerMgr.GetHeroCollectionBuff() / 1000.0;
		double _ArtifactValue = GetTotalEquipAbility(AbilityType.AddHpValue);			// 절대값
		double _ArtifactRate = GetTotalEquipAbility(AbilityType.AddHpRate) / 1000.0;	// 천분율
		double _SkillValue = GetTotalBuffAbilityValue(AbilityType.AddHpValue, GetBuffAbilityFlag.All ^ GetBuffAbilityFlag.ActiveSkill);	// 절대값(액티브 스킬은 _AddedHP에서 적용)
		double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddHpRate, GetBuffAbilityFlag.All ^ GetBuffAbilityFlag.ActiveSkill);	// 천분율(액티브 스킬은 _AddedHP에서 적용)
		return System.Math.Round(
            (_MaxHP * (1 + _SkillRate + _ArtifactRate + _CollectionBuff + GetGradeRate() + GetPlayerLvRate())) + _GradeBuff + _ArtifactValue + _SkillValue + GetAddedHP()
			);
    }

    /// <summary> 
    /// 장착한 아티펙트의 장착효과 값을 가져온다. = 유물 
    /// </summary>
    public float GetTotalEquipAbility(AbilityType _AbilityType)
	{
        float _Total = 0;
        PlayerMgr playerMgr = PlayerMgr.Instance;

        int _A_IDX = 0;
        ArtifactTargetType targetType = 0;
        bool flag = false;

        for (int i = 0, length = playerMgr.GetArtifactCnt(); i < length; ++i)
		{
            _A_IDX = playerMgr.GetArtifactIDX(i);

            if (playerMgr.GetArtifactEquip(_A_IDX) > 0)
            {
                for (int j = 0; j < 3; ++j) {
                    targetType = (ArtifactTargetType)playerMgr.GetArtifactTargetType(_A_IDX);
                    flag = false;

                    if (targetType == ArtifactTargetType.Race) {
                        flag = (_Race == playerMgr.GetArtifactTargetValue(_A_IDX));
                    }
                    else if (targetType == ArtifactTargetType.Type) {
                        flag = (_Type == playerMgr.GetArtifactTargetValue(_A_IDX));
                    }
                    else if (targetType == ArtifactTargetType.TableNum) {
                        flag = (_TableNum == playerMgr.GetArtifactTargetValue(_A_IDX));
                    }

                    // All 한번에 처리
                    if (flag && playerMgr.GetArtifactAbilityType(_A_IDX, j) == _AbilityType) {
                        _Total += playerMgr.GetArtifactAbilityTotalValue(_A_IDX, j);
                    }
                }
            }
		}
        return _Total;
	}

	/// <summary> 기본 공격력 반환 </summary>
	public override double GetNormalAttackDamage()
	{
        double _GradeBuff = PlayerMgr.Instance.GetHeroGradeBuff() * 50;
        double _CollectionBuff = PlayerMgr.Instance.GetHeroCollectionBuff() / 1000.0;
		double _ArtifactValue = GetTotalEquipAbility(AbilityType.AddAttackValue);
		double _ArtifactRate = GetTotalEquipAbility(AbilityType.AddAttackRate) / 1000.0;
		double _SkillValue = GetTotalBuffAbilityValue(AbilityType.AddAttackValue, GetBuffAbilityFlag.All);
		double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddAttackRate, GetBuffAbilityFlag.All);
        double _Damage = _ATK * (1 + _SkillRate + _ArtifactRate + _CollectionBuff + GetGradeRate() + GetPlayerLvRate()) + _GradeBuff + _ArtifactValue + _SkillValue; // 기본데미지

		return System.Math.Round(_Damage);
	}

	public override double GetDefense()
	{
        double _GradeBuff = PlayerMgr.Instance.GetHeroGradeBuff() * 50;
        double _CollectionBuff = PlayerMgr.Instance.GetHeroCollectionBuff() / 1000.0;
		double _ArtifactValue = GetTotalEquipAbility(AbilityType.AddDefenseValue);
		double _ArtifactRate = GetTotalEquipAbility(AbilityType.AddDefenseRate) / 1000.0;
		double _SkillValue = GetTotalBuffAbilityValue(AbilityType.AddDefenseValue, GetBuffAbilityFlag.All);
		double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddDefenseRate, GetBuffAbilityFlag.All);

		return System.Math.Round(
            _DEF * (1 + _SkillRate + _ArtifactRate + _CollectionBuff + GetGradeRate() + GetPlayerLvRate()) + _GradeBuff + _ArtifactValue + _SkillValue
			);
	}

	public override bool GetIsCritical()
	{
		float _ArtifactRate = GetTotalEquipAbility(AbilityType.AddCriticalRate);
		float _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddCriticalRate, GetBuffAbilityFlag.All);
		float _Percent = Mathf.Round(_Critical + _SkillRate + _ArtifactRate);
		// 천분율
		return Random.Range(0, 1000) <= _Percent;
	}

	public override float GetCriticalAtkPercent()
	{
		float _ArtifactRate = GetTotalEquipAbility(AbilityType.AddCriticalAttackRate);
		float _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddCriticalAttackRate, GetBuffAbilityFlag.All);
		return ((float)(1000 + _CriticalAtk + _SkillRate + _ArtifactRate)) / 1000.0f;
	}

	public virtual double GetPeneration()
	{
        float _ArtifactRate = GetTotalEquipAbility(AbilityType.AddPenerationRate);
		float _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddPenerationRate, GetBuffAbilityFlag.All);
		return _Peneration + _SkillRate + _ArtifactRate;
	}

	public virtual double GetHitRate()
	{
		double _GradeRate = _Grade * 100; // 천분율 값을 더하기 때문에 100을 곱함
		double _ArtifactRate = GetTotalEquipAbility(AbilityType.AddHitRate);
		double _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddHitRate, GetBuffAbilityFlag.All);
		return _HitRate + _SkillRate + _ArtifactRate + _GradeRate;
	}

	public virtual double GetAvoid()
	{
		float _ArtifactRate = GetTotalEquipAbility(AbilityType.AddAvoidRate) / 1000.0f;
		float _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddAvoidRate, GetBuffAbilityFlag.All);
		return System.Math.Round(_Avoid * (1 + _SkillRate + _ArtifactRate));
	}

	public override void ApplySkillCool(float _Percent = 1.0f)
	{
        if(m_CSVMgr == null) {
            m_CSVMgr = CSVMgr.Instance;
        }
		int _BaseValue = m_CSVMgr.GetSkillCoolDown(_SkillEffectID);
		float _SkillRate = GetTotalBuffAbilityRate(AbilityType.ReduceSkillDelayRate, GetBuffAbilityFlag.All);
		double _Delay = System.Math.Max(1, _BaseValue * (1 - _SkillRate)); // 1초 이상
		_SkillCool = SystemMgr.Instance.GetTimestamp() + (_Delay * _Percent);
	}

	public override void ApplyAutoSkillCool(float _Percent = 1.0f)
	{
		int _BaseValue = m_CSVMgr.GetSkillCoolDown(_AttackEffectID); // 밀리초로 변환
		float _SkillRate = GetTotalBuffAbilityRate(AbilityType.ReduceSkillDelayRate, GetBuffAbilityFlag.All);
		double _Delay = System.Math.Max(1, _BaseValue * (1 - _SkillRate)); // 1초 이상
		_AttackCool = SystemMgr.Instance.GetTimestamp() + (_Delay * _Percent);
	}

    /// <summary>
    /// 영웅 세팅하기 : Instantiate - prefab 해당위치에 넣기
    /// </summary>
    /// <param name="_SlotNum"> 리스트에서 선택된 영웅의 순서 </param>
    public virtual void SetHero(int _SlotNum)
    {
		this._SlotNum = _SlotNum;
        _TableNum = PlayerMgr.Instance.GetHeroSlot(_SlotNum);

        m_HeroBtn = StageUIMgr.Instance.GetHeroBtn(int.Parse(gameObject.name.Substring(name.Length - 2, 1)));
        m_HeroBtn.SetData(this);       // **타이밍 문제?

        if (_TableNum == -1)
        {
            gameObject.SetActive(false);
            return;
        }

        SetList(); 
        SetStatus();

        // 처음 시작시 스킬딜레이의 10%만 적용
        // 스킬 딜레이가 10초일 경우 9초뒤에 스킬을 쓸 수 있도록 설정
        ApplySkillCool(0.15f);
        ApplyAutoSkillCool(0.15f);

        if(CSVMgr.Instance.IsLoadStartStat()) {
            if (_AddStatScene.Contains(SystemMgr.Instance.GetSceneName())) { 
                AddStageStatus();
            }
        }
    }

    /// <summary>
    /// 오브젝트 활성화, 리스트 넣기
    /// </summary>
    protected void SetList()
    {   
        gameObject.SetActive(true);
	    UnitMgr.Instance.AddHeroList(this);
    }

    /// <summary>
    /// 버프 초기화
    /// </summary>
    public override void SetStatus()
    {
        base.SetStatus();

        m_CSVMgr.SetPassiveSkill(_SlotNum, _TableNum);
        
		_LV		= m_PlayerMgr.GetHeroLv(_TableNum);
		_Grade	= m_PlayerMgr.GetHeroGrade(_TableNum);
		_Awaken = m_PlayerMgr.GetHeroAwaken(_TableNum);
		_Race	= m_CSVMgr.GetRace(_TableNum);
		_Type	= m_CSVMgr.GetType(_TableNum);


        _SkillEffectID = _Awaken < 4 ? m_CSVMgr.GetSkillID(_TableNum, 0) : m_CSVMgr.GetSkillID(_TableNum, 4);
		_AttackEffectID = m_CSVMgr.GetSkillID(_TableNum, 2);

		_ATK		= m_CSVMgr.GetTotalATK(_TableNum, _LV);
		_DEF		= m_CSVMgr.GetTotalDEF(_TableNum, _LV);
		_Range		= m_CSVMgr.GetRange(_TableNum);
        _Critical	= m_CSVMgr.GetCritical(_TableNum);
		_CriticalAtk= m_CSVMgr.GetCriticalATK(_TableNum);
		_Peneration	= m_CSVMgr.GetPenetration(_TableNum);
		_HitRate	= m_CSVMgr.GetHitRate(_TableNum);
		_Avoid		= m_CSVMgr.GetAvoid(_TableNum);

		ResetActiveBuff();
    }

    void AddStageStatus() {

        int _StageValue = PlayerMgr.Instance.GetStageFull() + 1;      // **Start_Stat Table은 1스테이지부터 시작
        int _RebirthValue = PlayerMgr.Instance.GetResurrection();

        _CanStartStat = m_CSVMgr.CanApplyStartStat(_StageValue, _RebirthValue, _LV);
        if (_CanStartStat) {
            _ATK += m_CSVMgr.GetStartStatAddAttack(_StageValue);
            _DEF += m_CSVMgr.GetStartStatAddDefense(_StageValue);
        }
    }

    /// <summary>
    /// 체력을초기화한다 -> 체력증가 버프 때문에 따로 분리
    /// </summary>
    public void InitHP(bool _FillHP)
	{
        if(m_CSVMgr == null) {
            m_CSVMgr = CSVMgr.Instance;
        }
        _MaxHP = m_CSVMgr.GetTotalHP(_TableNum, _LV);

        if (_CanStartStat ) {
            _MaxHP += m_CSVMgr.GetStartStatAddHP(PlayerMgr.Instance.GetStageFull() + 1);
        }

        if (_FillHP == true) {
            _RemainHP = GetMaxHP();
        }
	}

    public virtual void SetDamage(DamageParam _DP) {
        //if (UnitMgr.Instance.GetActiveSkill() == null || _DP._DamageType == "Active") // 액티브 스킬 발동중일땐 액티브 스킬에만 데미지를 받음
        //{
        //    DamageType _DmgType = _DP.GetDamageType();
        //    double _Damage = _DP.CalcDamage(GetDefense());
        //    string _DmgText = SystemMgr.Instance.GetDoubleString(_Damage);

        //    if (GetRemainHP() > 0) {
        //        if (_DmgType == DamageType.Heal) // 힐
        //        {
        //            _RemainHP += _Damage;
        //            if (_RemainHP > GetMaxHP())
        //                _RemainHP = GetMaxHP();
        //            EffectMgr.Instance.CreateDamageEffect(transform, 2, string.Format("+{0}", SystemMgr.Instance.GetDoubleString(_Damage))); // 데미지 텍스트
        //        }
        //        else if (_DmgType == DamageType.Attack) {
        //            if (_DP.CalcIsHit(GetAvoid()) == true) {
        //                _RemainHP -= _Damage; // 체력 감소

        //                // 데미지 텍스트
        //                int criticalPoint = _DP._IsCritical ? 4 : 3;
        //                EffectMgr.Instance.CreateDamageEffect(transform, criticalPoint, _DmgText);
        //            }
        //            else // 영웅 적 회피
        //            {
        //                switch (ConfigMgr.Instance.GetCountry()) {
        //                    case "Taiwan":
        //                    case "Japan":
        //                        EffectMgr.Instance.CreateDamageEffect(transform, 5, "闪避"); // 데미지 텍스트
        //                        break;
        //                    default:
        //                        EffectMgr.Instance.CreateDamageEffect(transform, 5, "AVOID"); // 데미지 텍스트
        //                        break;
        //                }
        //            }
        //        }
        //        else if (_DmgType == DamageType.Buff) // 버프 적용
        //        {
        //            SetActiveBuff(_DP._SkillID, _DP.GetBuffDurationTime());
        //        }
        //        else    // 데미지 없음
        //        {
        //            return;
        //        }

        //        EffectMgr.Instance.CreateHitEffect(transform, _DP._HitEffect);  // 히트 이펙트
        //        ResourceMgr.Instance.PlaySound("FX", _DP._HitSound);            // 히트 효과음

        //        if (GetRemainHP() <= 0) {
        //            DeadStart();
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 스킬 사용
    /// </summary>
    /// <param name="_Target">타겟 번호</param>
    /// <param name="_DP"></param>
    /// <param name="_UseSkillEffect"></param>
    protected virtual void SendSkillDamage(SkillTargetType _Target, DamageParam _DP, bool _UseSkillEffect)
    {
        //if (_Target == SkillTargetType.None)
        //    return;

        //switch (_Target)
        //{
        //    case SkillTargetType.Myself:
        //        SetDamage(_DP);
        //        break;
        //    case SkillTargetType.Team:
        //        UnitMgr.Instance.GetHeroList(Random.Range(0, UnitMgr.Instance.GetHeroListCnt())).SetDamage(_DP);
        //        break;
        //    case SkillTargetType.TeamAll:
        //        for (int i = 0; i < UnitMgr.Instance.GetHeroListCnt(); i++)
        //            UnitMgr.Instance.GetHeroList(i).SetDamage(_DP);
        //        break;
        //    case SkillTargetType.Enemy:
        //        if (_UseSkillEffect == true)
        //            _Enemy.SetSkillEffect(_DP);
        //        else
        //            _Enemy.SetDamage(_DP);
        //        break;
        //    case SkillTargetType.EnemyAll:
        //        for (int i = 0; i < UnitMgr.Instance.GetEnemyListCnt(); i++)
        //        {
        //            if (_UseSkillEffect == true)
        //                UnitMgr.Instance.GetEnemyList(i).SetSkillEffect(_DP);
        //            else
        //                UnitMgr.Instance.GetEnemyList(i).SetDamage(_DP);
        //        }
        //        break;
        //}
    }

    /// <summary>
    /// 강제적으로 죽이기
    /// </summary>
    [ContextMenu("Force Die")]
    public void ForceDie()
    {
        _RemainHP = 0.0f;
        if (GetAddedHP() > 0.0f)
            _RemainHP -= GetRemainHP();
        DeadStart();
    }

    #endregion etc


    #region Dead

    protected override void Dead()
    {
        m_CSVMgr.ResetPassiveSkill(_SlotNum);  // 패시브 꺼짐

        // **오브젝트 제거
        // Destroy(_Obj);  
        _Obj = null;

        ResetActiveBuff();

        UnitMgr.Instance.RemoveHeroList(this);   // 활성화된 영웅 리스트에서 제거
    }

    #endregion Dead
}