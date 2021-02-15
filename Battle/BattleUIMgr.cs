using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 전투 중 OutGameUI
/// </summary>
public class BattleUIMgr : SingleTon<BattleUIMgr>
{
    [SerializeField] GameObject         m_BattleStart;

    [SerializeField] GameObject         m_SkillCutIn;
    [SerializeField] Image              m_SkillImage;       // 캐릭터 얼굴

    private Animator                    m_Animator;


    private void Start() {
        m_Animator = GetComponent<Animator>();
    }

    public void Anim_BattleStart() {
        m_BattleStart.SetActive(true);
        m_Animator.SetTrigger("BattleStart");
    }

    public void AnimationEvent_BattleStart() {
        m_BattleStart.SetActive(false);

        if(BattleMgr.Instance._OnBattleStart != null) {
            BattleMgr.Instance._OnBattleStart();
        }
    }


    public void Anim_SkillCutIn(int heroNum) {
        m_SkillImage.sprite = ResourceMgr.Instance.GetSprite("HeroFull", heroNum % 6);       // $
        m_SkillCutIn.SetActive(true);
        m_Animator.SetTrigger("SkillCutIn");
    }

    public void AnimationEvent_SkillCutIn() {
        m_SkillCutIn.SetActive(false);
    }

}
