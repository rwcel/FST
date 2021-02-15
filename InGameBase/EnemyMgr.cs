using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class EnemyMgr : Unit
{
    #region etc
    protected HeroMgr _Hero;        // 선택된 적


    public void SetEnemy(int _EnemyNum)
    {
		this._TableNum = _EnemyNum;
        if (_EnemyNum == -1) {
            gameObject.SetActive(false);
            return;
        }

        SetStatus();
    }

    public override void SetStatus()
    {
        base.SetStatus();

		UnitMgr.Instance.AddEnemyList(this);

        _LV         = m_PlayerMgr.GetStageFull() + 1;
		_Grade      = m_CSVMgr.GetGrade(_TableNum);
		_Awaken     = m_CSVMgr.GetAwaken(_TableNum);
		_Race       = m_CSVMgr.GetRace(_TableNum);
		_Type       = m_CSVMgr.GetType(_TableNum);

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

		// 처음 시작시 스킬딜레이의 10%만 적용
		// 스킬 딜레이가 10초일 경우 9초뒤에 스킬을 쓸 수 있도록 설정
		ApplySkillCool(0.1f);
		ApplyAutoSkillCool(0.1f);
    }

	/// <summary>
	/// 체력을초기화한다 -> 체력증가 버프 때문에 따로 분리
	/// </summary>
	public virtual void InitHP()
	{
        _RemainHP = GetMaxHP();

        gameObject.SetActive(true);
    }


    public virtual void SetDamage(DamageParam _DP)
    {
        //if (UnitMgr.Instance.GetActiveSkill() == null || _DP._DamageType == "Active") // 액티브 스킬 발동중일땐 액티브 스킬에만 데미지를 받음
        //{
        //    DamageType _DmgType = _DP.GetDamageType();
        //    double _Damage = _DP.CalcDamage(GetDefense());
        //    string _DmgText = SystemMgr.Instance.GetDoubleString(_Damage);

        //    if (GetRemainHP() > 0)
        //    {
        //        if (_DmgType == DamageType.Heal) // 힐
        //        {
        //            _RemainHP += _Damage;
        //            if (GetRemainHP() > GetMaxHP())
        //                _RemainHP = GetMaxHP();
        //            EffectMgr.Instance.CreateDamageEffect(transform, 2, string.Format("+{0}", SystemMgr.Instance.GetDoubleString(_Damage))); // 데미지 텍스트
        //        }
        //        else if (_DmgType == DamageType.Attack)
        //        {
        //            if (_DP.CalcIsHit(GetAvoid()) == true)
        //            {
        //                _RemainHP -= _Damage; // 체력 감소

        //                // 데미지 텍스트
        //                int criticalPoint = _DP._IsCritical ? 1: 0;
        //                EffectMgr.Instance.CreateDamageEffect(transform, criticalPoint, _DmgText);
        //            }
        //            else // 영웅 빛맞춤
        //            {
        //                switch(ConfigMgr.Instance.GetCountry())
        //                {
        //                    default:
        //                        EffectMgr.Instance.CreateDamageEffect(transform, 6, "MISS"); // 데미지 텍스트
        //                        break;

        //                    case "Japan":
        //                    case "Taiwan":
        //                        EffectMgr.Instance.CreateDamageEffect(transform, 6, "未命中"); // 데미지 텍스트
        //                        break;
        //                }
        //            }
        //        }
        //        else if (_DmgType == DamageType.Buff) // 버프 적용
        //        {
        //            SetActiveBuff(_DP._SkillID, _DP.GetBuffDurationTime());
        //        }
        //        else
        //        {
        //            return;
        //        }

        //        EffectMgr.Instance.CreateHitEffect(transform, _DP._HitEffect); // 히트 이펙트
        //        ResourceMgr.Instance.PlaySound("FX", _DP._HitSound);

        //        if (GetRemainHP() <= 0)
        //        {
        //            DeadStart();
        //        }
        //    }
        //}
    }

    public void SetSkillEffect(DamageParam _DP)
    {
        StartCoroutine(EffectDelay(_DP));
    }

    IEnumerator EffectDelay(DamageParam _DP)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f)); // 정지 모션이 각각 따로 풀리는 듯한 느낌을 주기 위해 렌덤사용

        SetDamage(_DP);
    }

    // 스킬 사용
    protected virtual void SendSkillDamage(SkillTargetType _Target, DamageParam _DP, bool _UseSkillEffect)
    {
        if (_Target == SkillTargetType.None)
            return;

        switch (_Target)
        {
            case SkillTargetType.Myself:
                SetDamage(_DP);
                break;
            case SkillTargetType.Team:
                UnitMgr.Instance.GetEnemyList(Random.Range(0, UnitMgr.Instance.GetEnemyListCnt())).SetDamage(_DP);
                break;
            case SkillTargetType.TeamAll:
                for (int i = 0; i < UnitMgr.Instance.GetEnemyListCnt(); i++)
                    UnitMgr.Instance.GetEnemyList(i).SetDamage(_DP);
                break;
            case SkillTargetType.Enemy:
                this._Hero.SetDamage(_DP);
                break;
            case SkillTargetType.EnemyAll:
                for (int i = 0; i < UnitMgr.Instance.GetHeroListCnt(); i++)
                    UnitMgr.Instance.GetHeroList(i).SetDamage(_DP);
                break;
        }
    }

    // 중복
    public override double GetNormalAttackDamage()
    {
        double _SkillValue = GetTotalBuffAbilityValue(AbilityType.AddAttackRate, GetBuffAbilityFlag.ActiveSkill);
        double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddAttackRate, GetBuffAbilityFlag.ActiveSkill);
        double _Damage = _ATK * (1 + _SkillRate + GetGradeRate()) + _SkillValue;

        return System.Math.Round(_Damage);
    }

    public override double GetDefense()
    {
        double _SkillValue = GetTotalBuffAbilityValue(AbilityType.AddDefenseValue, GetBuffAbilityFlag.ActiveSkill);
        double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddDefenseRate, GetBuffAbilityFlag.ActiveSkill);

        return System.Math.Round(_DEF * (1 + _SkillRate + GetGradeRate()) + _SkillValue);
    }

    public override bool GetIsCritical()
    {
        float _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddCriticalRate, GetBuffAbilityFlag.ActiveSkill);
        float _Percent = Mathf.Round(_Critical + _SkillRate);
        // 천분율
        return Random.Range(0, 1000) <= _Percent;
    }

    public override float GetCriticalAtkPercent()
    {
        float _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddCriticalAttackRate, GetBuffAbilityFlag.ActiveSkill);
        return (1000.0f + (float)_CriticalAtk + _SkillRate) / 1000.0f;
    }

    public virtual double GetPeneration()
    {
        float _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddPenerationRate, GetBuffAbilityFlag.All);
        return _Peneration + _SkillRate;
    }

    public virtual double GetHitRate()
    {
        int _GradeRate = _Grade * 100; // 천분율 값을 더하기 때문에 100을 곱함
        double _SkillRate = GetTotalBuffAbilityValue(AbilityType.AddHitRate, GetBuffAbilityFlag.All);
        return _HitRate + _SkillRate + _GradeRate;
    }

    public virtual double GetAvoid()
    {
        double _SkillRate = GetTotalBuffAbilityRate(AbilityType.AddAvoidRate, GetBuffAbilityFlag.All);
        return System.Math.Round(_Avoid * (1 + _SkillRate));
    }

    private void OnDrawGizmos()
    {
        if (_Hero != null)      // 타겟 선긋기
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _Hero.transform.position);
        }
    }

    #endregion etc
    

    #region Dead

    /// <summary>
    /// 죽음처리
    /// </summary>
    /// **이벤트 Bool변수 하나 만들어서 진행 - enum으로 다른 event들 따로 처리
    /// **SetData int형 enum형으로 바꾸기
    /// **Switch ~ case 한번만 사용하기
    /// **Destroy 제거 -> Resource Hierarchy 등에 저장 or setactive_false 하고 get에서 한번 더 확인하기1
    protected override void Dead()
    {
        _Hero = null;

        if (PrefsMgr.Instance.GetCameraShakeOnOff() == 0) {
            CameraMgr.Instance.SetShake(1);
        }

        //EffectMgr.Instance.CreateDieEffect(transform);      // 코인드랍 이펙트

        // 스테이지에서만 돈, 경험치 획득
        if (SystemMgr.Instance.GetSceneName() == "Stage") {
            if(BattleMgr.Instance != null) {
                BattleMgr.Instance.AddGold(PlayerMgr.Instance.GetRewardGoldWhenKillEnemy());
                BattleMgr.Instance.AddExp(CSVMgr.Instance.GetRewardExp(_TableNum));
            }
        }

        //Destroy(_Obj);                  // **매우큰 문제

        UnitMgr.Instance.RemoveEnemyList(this);
    }
    
    #endregion Dead
}
