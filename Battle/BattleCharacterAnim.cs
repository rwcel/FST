using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

// **Idle Parameters는 존재하지 않음
public enum BattleAnims
{
    Idle, Run, Ready,           // Bool
    Attack = 10, Skill,         // Trigger
    Dead = 20,                  // Trigger + Any State
}

public class BattleCharacterAnim : MonoBehaviour
{
    private GameObject          m_Character;

    private Animator            m_Animator;
    private BattleAnims         m_BattleAnim;
    private SkeletonMecanim     m_SkeletonMecanim;
    private MeshRenderer        m_MeshRenderer;

    public int _StartOrderLayer { get; private set; }

    private System.Action       m_OnAnimEnd;
    private bool                m_IsNude;

    // 10으로 나누었을 때 수에 따라 결정
    private static readonly int _Anim_Type_Bool = 0;
    private static readonly int _Anim_Type_Trigger = 1;
    private static readonly int _Anim_Type_Trigger_Any = 2;

    [SerializeField] private string _Skin_Normal;
    [SerializeField] private string _Skin_Nude;

    /// <summary>
    /// **tableNum이 현재는 slotNum으로 들어옴
    /// </summary>
    public void SetData(bool isHero, int tableNum, int arrayNum) {
        if(isHero) {
            m_Character = ResourceMgr.Instance.GetPrefab("BattleHero", tableNum, transform);
        }
        else {      // 0 -> tableNum
            m_Character = ResourceMgr.Instance.GetPrefab("Enemy", 0, transform);
        }

        if(m_Character == null) {
            Debug.Log(tableNum);
        }

        m_Animator = m_Character.GetComponent<Animator>();
        m_SkeletonMecanim = m_Character.GetComponent<SkeletonMecanim>();
        m_MeshRenderer = m_SkeletonMecanim.GetComponent<MeshRenderer>();

        m_IsNude = false;
        // SetNude(m_isNude);

        SetInitLayer(isHero, arrayNum); 
    }

    public void SetAnim(BattleAnims animType) {
        m_BattleAnim = animType;

        if((int)m_BattleAnim / 10 == _Anim_Type_Bool) {
            if(m_BattleAnim == BattleAnims.Idle) {
                m_Animator.SetBool(BattleAnims.Run.ToString(), false);
                m_Animator.SetBool(BattleAnims.Ready.ToString(), false);
            }
            else if (m_BattleAnim == BattleAnims.Run) {
                m_Animator.SetBool(BattleAnims.Run.ToString(), true);
            }
            else if (m_BattleAnim == BattleAnims.Ready) {
                m_Animator.SetBool(BattleAnims.Ready.ToString(), true);
            }

        }
        else if ((int)m_BattleAnim / 10 == _Anim_Type_Trigger) {
            m_Animator.SetTrigger(m_BattleAnim.ToString());

            if (m_BattleAnim == BattleAnims.Skill) {
                m_Animator.SetBool(BattleAnims.Ready.ToString(), false);
                // 스킬 이미지 등 띄우기 -> BattleUI
            }
        }
        else if ((int)m_BattleAnim / 10 == _Anim_Type_Trigger_Any) {
            m_Animator.SetTrigger(m_BattleAnim.ToString());

            //StartCoroutine("AnimEndCheck");
        }
    }

    public void DeadActive() {
        m_Character.SetActive(false);
    }

    public static readonly int[] _Layer_Hero    = { 22, 12, 13, 23 };
    public static readonly int[] _Layer_Enemy   = { 11, 21, 1, 10, 20, 0, 12, 2 };

    /// <summary>
    /// Hero : 22, 12, 13, 23 
    /// Enemy : 11, 21, 1, 10, 20, 0, 12, 2
    /// </summary>
    private void SetInitLayer(bool isHero, int arrayNum) {
        if(isHero) {
            m_MeshRenderer.sortingOrder = _Layer_Hero[arrayNum];
        }
        else {
            m_MeshRenderer.sortingOrder = _Layer_Enemy[arrayNum];
        }
        _StartOrderLayer = m_MeshRenderer.sortingOrder;
    }

    /// <summary>
    /// 레이어 조절함수
    /// * 아래보다 안크게 조절하면서 올려야한다
    /// </summary>
    public void SetAttackLayer(bool isAttack, int targetOrderLayer = 0) {
        if(!isAttack) {
            m_MeshRenderer.sortingOrder = _StartOrderLayer;
            return;
        }

        // 공격중일때 
        m_MeshRenderer.sortingOrder = targetOrderLayer + 5;
    }

    public void SetNude(bool isNude) {
        if(m_IsNude) {
            return;     // 두번은 변하지 않기
        }
        m_IsNude = isNude;

        //Debug.Log(_Skin_Nude + "," + _Skin_Normal);

        try {
            m_SkeletonMecanim.skeleton.SetSkin(m_IsNude ? "tula" : "tula_001");
        }
        catch { Debug.Log("Skin이 없습니다"); };
    }

    /// <summary>
    /// 애니메이션 이벤트 대신해서 끝날때 발동해주는 함수
    /// </summary>
    IEnumerator AnimEndCheck() {
        WaitForSeconds waitTerm = new WaitForSeconds(0.1f);
        while(true) {
            Debug.Log(m_Animator.GetCurrentAnimatorStateInfo(0).length + ">" +
            m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (m_Animator.GetCurrentAnimatorStateInfo(0).length >
            m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime) {
                break;
            }
            yield return waitTerm;
        }

        if(m_OnAnimEnd != null) {
            m_OnAnimEnd();
            m_OnAnimEnd = null;
        }
    }


}
