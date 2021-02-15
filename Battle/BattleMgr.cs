using UnityEngine;
using System.Collections;

using System.Collections.Generic;


public class BattleMgr : SingleTon<BattleMgr>
{
    public Queue<BattleCharacter>   _BattleQueue;
    public System.Action            _OnBattleStart;             // 전투시작 -> 캐릭터들 공격준비
    public System.Action            _OnCharacterDead;           // 죽음 -> 전투 끝 등 확인
    public System.Action            _OnBattleEnd;               // 전투끝 -> 다음 페이즈까지 공격 중지

    [SerializeField] BattleTarget   m_HeroTarget;
    [SerializeField] BattleTarget   m_EnemyTarget;

    private BattleCharacter         m_Attacker;

    private bool                    m_IsBattle;                 // 전투 상태인지 확인
    private bool                    m_IsAttacking;              // 이미 공격중인 캐릭터가 있는지 확인
    private bool                    m_IsSkilling;               // 스킬 사용 대기 캐릭터 확인

    // 전투 다 끝나면 데이터 넘겨주기
    protected int                   m_CurPhase;
    protected BattleResult          m_Result;                   // 전투 결과 : 스테이지 재 로드하기때문에 초기화 할 필요가 없다
    protected bool                  m_IsClear;                  // 스테이지 클리어 조건

    protected bool                  m_OnNewBeeIcon;             // stage, boss에서만 작동

    private double                  m_GoldData;
    private double                  m_ExpData;

    private bool                    m_FightVoice = false;
    private bool                    m_CheerupVoice = false;


    protected virtual void Start() {
        m_CurPhase = 0;

        m_IsAttacking = false;
        _BattleQueue = new Queue<BattleCharacter>();

        m_IsBattle = false;
        _OnBattleEnd += () => m_IsBattle = false;
        _OnBattleStart += () => m_IsBattle = true;

        SetHero();

        Time.timeScale = PlayerMgr.Instance.GetPlaySpeed();
    }

    private void SetHero() {
        for (int i = 0, length = UnitMgr.Instance.GetHeroMgrCnt(); i < length; ++i) {
            HeroMgr newHero = UnitMgr.Instance.GetHeroMgr(i);
            newHero.SetHero(i);              // 영웅 세팅

            if (newHero._TableNum >= 0) {
                newHero.InitHP(true);        // *내에 BattleCharacter Set있음
                newHero.GetComponent<BattleCharacter>().SetData(i);
            }
        }
    }


    #region Battle

    /// <summary>
    /// 공격확인
    /// 공격이 끝났으면 공격할 수 있게 변경
    /// </summary>
    public void BattleCheck(bool isAttackEnd = false) {
        if (isAttackEnd) {
            m_IsAttacking = false;
        }

        if (m_IsAttacking || _BattleQueue.Count <= 0) {
            return;
        }

        // 스킬 사용
        if (m_IsSkilling) {
            SetSkill();
        }
        else {
            SetAttack();
        }
    }

    public void SkillEnd() {
        m_IsSkilling = false;
        BattleCheck(true);
    }

    /// <summary>
    /// 스킬 사용 시, Queue보다 우선순위로 적용
    /// **비전투시 return false
    /// </summary>
    /// <param name="attacker"></param>
    public bool SkillReady(BattleCharacter attacker) {
        if(!m_IsBattle) {
            return false;
        }
        if(m_IsSkilling) {
            return false;
        }

        m_Attacker = attacker;
        m_IsSkilling = true;
        return true;
    }

    private void SetAttack() {
        m_Attacker = _BattleQueue.Dequeue();
        // Debug.Log(attacker.name);
        if (m_Attacker.CompareTag("Hero")) {
            m_IsAttacking = m_Attacker.Attack(m_EnemyTarget.GetTargetByRandom());
        }
        else if (m_Attacker.CompareTag("Enemy")) {
            m_IsAttacking = m_Attacker.Attack(m_HeroTarget.GetTargetByRandom());
        }

        if (!m_IsAttacking) {
            m_Attacker.AttackEnd();
        }
    }

    private void SetSkill() {
        m_Attacker.SkillStart();
    }

    /// <summary>
    /// 타겟 정하기
    /// </summary>
    /// <returns></returns>
    public BattleCharacter[] GetSkillTargets(SkillTargetType targetType) {
        BattleCharacter[] targets;

        bool heroFlag = false;
        bool multiFlag = false;

        // Debug.Log(targetType);

        if (targetType == SkillTargetType.Myself) {
            targets = new BattleCharacter[1];
            targets[0] = m_Attacker;
            return targets;

        }
        else if (targetType == SkillTargetType.Team) {
            if (m_Attacker.CompareTag("Hero")) {
                heroFlag = true;
                multiFlag = false;
            }
            else {
                heroFlag = false;
                multiFlag = false;
            }
        }
        else if (targetType == SkillTargetType.Enemy) {
            if (m_Attacker.CompareTag("Hero")) {
                heroFlag = false;
                multiFlag = false;
            }
            else {
                heroFlag = true;
                multiFlag = false;
            }
        }
        else if (targetType == SkillTargetType.TeamAll) {
            if (m_Attacker.CompareTag("Hero")) {
                heroFlag = true;
                multiFlag = true;
            }
            else {
                heroFlag = false;
                multiFlag = true;
            }
        }
        else if (targetType == SkillTargetType.EnemyAll) {
            if (m_Attacker.CompareTag("Hero")) {
                heroFlag = false;
                multiFlag = true;
            }
            else {
                heroFlag = true;
                multiFlag = true;
            }
        }

        if (heroFlag && !multiFlag) {  // Target : Hero Single
            targets = new BattleCharacter[1];
            targets[0] = m_HeroTarget.GetTargetByRandom();
        }
        else if (!heroFlag && !multiFlag) {  // Target : Enemy Single
            targets = new BattleCharacter[1];
            targets[0] = m_EnemyTarget.GetTargetByRandom();
        }
        else if (heroFlag && multiFlag) {  // Target : Hero Multi
            targets = new BattleCharacter[UnitMgr.Instance.GetHeroListCnt()];
            for (int i = 0, length = targets.Length; i < length; i++) {
                targets[i] = UnitMgr.Instance.GetHeroList(i)._BattleCharacter;
            }
        }
        else { //if (!heroFlag && multiFlag)  Target : Enemy Multi
            targets = new BattleCharacter[UnitMgr.Instance.GetEnemyListCnt()];
            for (int i = 0, length = targets.Length; i < length; i++) {
                targets[i] = UnitMgr.Instance.GetEnemyList(i)._BattleCharacter;
            }
        }

        return targets;
    }

    /// <summary>
    /// 전투 끝나면? : 
    /// 승리 유무에 따라 다음스테이지 ? 현재 스테이지
    /// result = true : 승리
    /// </summary>
    public virtual void BattleEnd(bool result) {
        if(result) {    // Animation끝나고 이벤트로 실행할 수 있게끔?
            Invoke("CheckPhase", 1.5f);
        }
        else {
            m_Result = BattleResult.Fail;
            // N1 스테이지에서는 죽으면 보스로 가기때문에 처리 안함
            if(PlayerMgr.Instance.GetStageInEpisode() != 0) {
                PlayerMgr.Instance.AddStage(-1);
            }
            TransitionUIMgr.Instance.TransitionBox(SystemMgr.Instance.MoveInGame, Vector2.left, 1f, true);
        }

        if(_OnBattleEnd != null) {
            _OnBattleEnd();
        }
    }

    #endregion Battle

    #region Voice

    public void SetFightVoice(bool _OnOff) {
        PlayerMgr.Instance.SetVoiceRotation();

        m_FightVoice = _OnOff;
    }

    public bool GetFightVoice() {
        return m_FightVoice;
    }

    public void CheckCheerupVoice() {
        if (!m_CheerupVoice && UnitMgr.Instance.GetHeroListCnt() > 0) {
            m_CheerupVoice = true;

            ResourceMgr.Instance.PlaySound("Voice", CSVMgr.Instance.GetHeroCheerupVoice(UnitMgr.Instance.GetHeroList(0)._TableNum));
        }
    }

    #endregion Voice

    #region Data

    public void AddGold(double value) {
        m_GoldData += value;
    }
    public void AddExp(double value) {
        m_ExpData += value;
    }

    public void ApplyData() {
        //Debug.Log(m_GoldData + "," + m_ExpData);
        if (m_GoldData > 0) {
            PlayerMgr.Instance.AddResource(PaymentType.Gold, m_GoldData);
        }

        if (m_ExpData > 0) {
            // ** 빠른 진행 위해 경험치 5배
            PlayerMgr.Instance.AddEXP((int)m_ExpData);
        }

        ResetData();
    }

    public void ResetData() {
        m_GoldData = 0;
        m_ExpData = 0;
    }

    #endregion Data


    #region Phase

    // 3부터 전투시작
    public virtual void CheckPhase()
    {
        m_CurPhase++;   // 페이즈 증가
    }

    #endregion Phase


    public virtual void CustomNextStage(int stageNum, bool up) {
    }
}
