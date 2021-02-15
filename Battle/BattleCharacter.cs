using System.Collections;
using UnityEngine;

using DG.Tweening;

/// <summary>
/// 전투 함수
/// </summary>
public class BattleCharacter : MonoBehaviour
{
    public bool                             _IsLive { get; private set; }
    public Unit                             _Unit { get; private set; }

    [SerializeField] BattleCharacterAnim    m_Anim;
    [SerializeField] BattleCharacterUI      m_UI;

    private WaitForSeconds                  m_AttackReadyWaitTerm;
    private Transform                       m_MoveTr;                   // 자식이 움직이는 구조
    private BattleCharacter                 m_AttackTarget;

    protected double                        m_MaxHP = 0;
    protected double                        m_CurHP = 0;
    protected float                         m_AttackSpeed = 0;          // **데이터 필요
    protected float                         m_AttackRange = 0;          // 1<="근접"<1.4, 1.4<"원거리"<=2

    private int                             m_SkillID = 0;

    private Coroutine                       m_SkillCoroutine;
    private SkillEffect                     m_SkillEffect;

    private CSVMgr                          m_CSVMgr;

    // $ 스킬 번호
    private static readonly int _ID_Attack_Melee = 1;
    private static readonly int _ID_Attack_Range = 0;

    private void OnEnable() {
        m_MoveTr = transform.GetChild(0);
    }

    public void SetData(int arrayNum) {
        // component
        m_CSVMgr = CSVMgr.Instance;
        _Unit = GetComponent<Unit>();

        // stat
        _IsLive = true;
        m_AttackSpeed = Random.Range(1f, 1.5f);         // $ 사거리 표시
        m_AttackRange = _Unit.GetAttackRange();
        m_MaxHP = _Unit.GetMaxHP();
        m_CurHP = m_MaxHP;
        // ui
        m_UI.SetScreen((float)m_MaxHP);

        // anim  **slotNum -> tableNum
        m_Anim.SetData(gameObject.CompareTag("Hero"), arrayNum, arrayNum);
        m_Anim.SetAnim(BattleAnims.Run);

        m_AttackReadyWaitTerm = new WaitForSeconds(m_AttackSpeed);

        BattleMgr.Instance._OnBattleStart += () => AttackStart();
        BattleMgr.Instance._OnBattleEnd += () => StopCoroutine("AttackReady");
    }

    public void UpdateStat() {
        m_MaxHP = _Unit.GetMaxHP();
        m_UI.SetMaxHP((float)m_MaxHP);
    }

    #region Attack

    public void AttackStart() {
        StopCoroutine("AttackReady");
        if (gameObject.activeSelf) {
            StartCoroutine("AttackReady");
        }
    }

    IEnumerator AttackReady() {
        yield return m_AttackReadyWaitTerm;
        BattleMgr.Instance._BattleQueue.Enqueue(this);
        BattleMgr.Instance.BattleCheck();
    }

    public bool Attack(BattleCharacter target) {
        //Debug.Log(gameObject.name+"의 공격 : "+target.name);
        if (target == null) {
            return false;
        }
        if (m_CurHP <= 0f) {
            return false;
        }

        m_AttackTarget = target;

        if (m_AttackRange < 1.4f) {
            MeleeAttack();
        }
        else {
            RangeReady();
        }

        return true;
    }

    /// <summary>
    /// 근거리 공격 : 이동해서 공격
    /// </summary>
    private void MeleeAttack() {
        Vector3 dest = m_AttackTarget.transform.position;
        if(m_AttackTarget.CompareTag("Enemy")) {   // 왼쪽 -> 오른쪽
            dest += Vector3.left * 120f;
        }
        else if (m_AttackTarget.CompareTag("Hero")) { // 오른쪽 -> 왼쪽
            dest += Vector3.right * 120f;
        }

        m_Anim.SetAttackLayer(true, m_AttackTarget.m_Anim._StartOrderLayer);
        m_MoveTr.DOMove(dest, 0.3f).OnComplete(() => {
            m_Anim.SetAnim(BattleAnims.Attack);
            m_SkillEffect = EffectMgr.Instance.SetSkillEffect(m_MoveTr, _ID_Attack_Melee);//_Unit._AttackEffectID);
            if(gameObject.CompareTag("Hero")) {
                m_SkillEffect.transform.rotation = Quaternion.Euler(new Vector2(0, 180f));
            }
            else {
                m_SkillEffect.transform.rotation = Quaternion.identity;
            }
            m_SkillEffect.SetAttackObj();
            Invoke("AttackHit", 0.4f);
        });
    }

    /// <summary>
    /// 원거리 공격 준비 : 준비자세 후 공격
    /// **Animation Event
    /// </summary>
    private void RangeReady() {
        m_Anim.SetAnim(BattleAnims.Ready);
        Invoke("RangeAttack", 1.0f);
    }

    /// <summary>
    /// 원거리 공격
    /// </summary>
    private void RangeAttack() {
        m_Anim.SetAnim(BattleAnims.Skill);

        // 발사체일 경우
        m_SkillEffect = EffectMgr.Instance.SetSkillEffect(m_MoveTr, _ID_Attack_Range);
        m_SkillEffect.SetAttackObj();
        m_SkillEffect.transform.DOMove(m_AttackTarget.transform.position, 0.3f).OnComplete(() => {
            AttackHit();
            m_SkillEffect.SetHitObj(m_AttackTarget.transform);
        });

        // 소환형인 경우
        //m_RangeEffect.transform.position = m_Target.transform.position;
        //Invoke("AttackHit", 0.4f);
    }

    /// <summary>
    /// 공격 맞춤
    /// **Animation Event
    /// </summary>
    public void AttackHit() {
        if (m_AttackRange <= 1.5f) {
            m_SkillEffect.SetHitObj(m_AttackTarget.transform);
        }

        //int _HitSound = m_CSVMgr.GetNormalHitSound(_TableNum);
        //if (_HitSound >= 0) {
        //    ResourceMgr.Instance.PlaySound("FX", m_CSVMgr.GetNormalHitSound(_TableNum));
        //}

        DamageParam _DP = new DamageParam(_Unit._TableNum, -1, "Normal");
        _DP.SetDamage(_Unit);
        m_AttackTarget.SetDamage(_DP);

        m_Anim.SetAttackLayer(false);
        m_MoveTr.transform.DOLocalMove(Vector3.zero, 0.3f).OnComplete(() => {
            AttackEnd();
        });
    }

    // **Animation Event
    public void AttackEnd() {
        BattleMgr.Instance.BattleCheck(true);
        AttackStart();
    }

    #endregion Attack

    #region Skill

    public void SkillStart() {
        ResourceMgr.Instance.PlaySound("Voice", m_CSVMgr.GetHeroSkillVoice(_Unit._TableNum));

        //BattleCamera.Instance.SetFocus(this);
        m_SkillID = _Unit._SkillEffectID;                       // *스킬이 여러개일 가능성 배제

        m_SkillEffect = EffectMgr.Instance.SetSkillEffect(m_MoveTr, m_SkillID);
        ResourceMgr.Instance.PlaySound("FX", m_CSVMgr.GetSkillCastSound(m_SkillID));

        _Unit.ApplySkillCool();

        m_Anim.SetAnim(BattleAnims.Ready);

        if(m_SkillCoroutine == null) {
            m_SkillCoroutine = StartCoroutine("SkillReady");
        }
    }

    /// <summary>
    /// ** AnimaionEvent
    /// </summary>
    IEnumerator SkillReady() {
        BattleUIMgr.Instance.Anim_SkillCutIn(_Unit._TableNum);
        yield return new WaitForSeconds(m_CSVMgr.GetSkillEffectPlayTime(m_SkillID));
        BattleUIMgr.Instance.AnimationEvent_SkillCutIn();

        m_Anim.SetAnim(BattleAnims.Skill);

        SkillHit();

        m_SkillCoroutine = null;
    }

    // **Animation Event || Action Event
    private void SkillHit() {
        BattleCharacter[] targets = BattleMgr.Instance.GetSkillTargets(m_CSVMgr.GetSkillTarget(m_SkillID));

        //m_SkillEffect.SetHitObj(m_SkillTargets[0].transform);
        ResourceMgr.Instance.PlaySound("FX", m_CSVMgr.GetSkillShotSound(m_SkillID));

        DamageParam _DP = new DamageParam(_Unit._TableNum, m_SkillID, "Active");
        _DP.SetDamage(_Unit);
        for (int i = 0, length = targets.Length; i < length; i++) {
            targets[i].SetDamage(_DP);
        }

        Invoke("SkillEnd", 0.5f);
    }

    // **Animation Event
    private void SkillEnd() {
        BattleMgr.Instance.SkillEnd();
    }

    #endregion Skill


    #region Damage

    // **빠른 진행위해 데미지 3배 조정
    public void SetDamage(DamageParam dp) {
        if (m_CurHP <= 0) {
            return;
        }
        //Debug.Log(string.Format("데미지 : {0} / 방어력 : {1}", damage, _Unit.GetDefense()));

        DamageType damageType = dp.GetDamageType();
        double resultDamage = dp.CalcDamage(_Unit.GetDefense());

        if (damageType == DamageType.Heal)                          // 힐
        {
            m_CurHP += resultDamage;
            if (m_CurHP > m_MaxHP) {
                m_CurHP = m_MaxHP;
            }
            m_UI.SetCurHP((float)m_CurHP, DamageFontType.Heal, (int)resultDamage);
        }
        else if (damageType == DamageType.Attack) {
            //if (_DP.CalcIsHit(GetAvoid()) == true)
            m_CurHP -= resultDamage;
            m_UI.SetCurHP((float)m_CurHP, DamageFontType.Damage, (int)resultDamage);
        }
        else if (damageType == DamageType.Buff)                     // 버프 적용
        {
            // Debug.Log("버프 : "+dp._EffectType);
            if (dp._EffectType == AbilityType.Provocation)          // 도발처리
            {
                Debug.Log("도발 미구현");
                return;

                //if (Random.Range(0, 1) < _Damage) {
                //    SetActiveBuff(_DP._SkillID, _DP.GetBuffDurationTime());

                //    for (int i = 0; i < UnitMgr.Instance.GetEnemyListCnt(); i++)
                //        UnitMgr.Instance.GetEnemyList(i).FindStart();
                //}
            }
            else {
                _Unit.SetActiveBuff(dp._SkillID, dp.GetBuffDurationTime());
            }
        }

        //ResourceMgr.Instance.PlaySound("FX", _DP._HitSound);            // 히트 효과음

        if (m_CurHP <= 0) {
            m_CurHP = 0f;
            Dead();
        }
        //else if (m_CurHP <= m_MaxHP * 0.5f) {                             // 누드 처리
        //    m_Anim.SetNude(true);
        //}
    }

    private void Dead() {
        _IsLive = false;
        _Unit.DeadStart();
        StopCoroutine("AttackReady");
        m_Anim.SetAnim(BattleAnims.Dead);
        BattleMgr.Instance._OnCharacterDead();

        Invoke("DeadEnd", 1.0f);
    }

    /// <summary>
    /// **애니메이션 이벤트로 해야함
    /// </summary>
    public void DeadEnd() {
        gameObject.SetActive(false);
        m_Anim.DeadActive();
    }

    #endregion Damage
}
