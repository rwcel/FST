using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleCharacterUI : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] Image          m_HPBar = null;
    [SerializeField] Image          m_DamageBar = null;         // *Slider의 경우 튀어나오는 경우가 있어서 Image 사용
    [SerializeField] Text           m_HPValue = null;
    [Header("Font")]
    [SerializeField] Text           m_DamageText = null;
    [SerializeField] Text           m_HealText = null;
    [SerializeField] Text           m_CriticalText = null;

    private float                   m_MaxHP;
    private float                   m_CurHP;
    private float                   m_DamageHP;

    // Font Anim
    private Animation               m_DamageTextAnim = null;
    private Animation               m_HealTextAnim = null;
    private Animation               m_CriticalTextAnim = null;

    private static readonly float _FillAmount_Time = 1.0f;


    private void Start() {
        m_DamageTextAnim = m_DamageText.GetComponent<Animation>();
        m_HealTextAnim = m_HealText.GetComponent<Animation>();
        m_CriticalTextAnim = m_CriticalText.GetComponent<Animation>();
        
    }

    public void SetScreen(float maxHP) {
        m_MaxHP = maxHP;

        SetMaxHP(m_MaxHP);
        SetCurHP(m_MaxHP, DamageFontType.None);
    }

    public void SetMaxHP(float maxHP) {
        m_MaxHP = maxHP;
        UpdateHP();
    }

    /// <summary>
    /// *하나로 통일하고 머티리얼, font를 바꾸는 방법도 있습니다.
    /// </summary>
    public void SetCurHP(float curHP, DamageFontType fontType, int damage = 0) {
        m_CurHP = curHP;
        UpdateHP();

        if(fontType == DamageFontType.None) {
            return;
        }
        else if(fontType == DamageFontType.Damage) {
            m_DamageText.text = damage.ToString();
            m_DamageTextAnim.Play("DamageFont");
        }
        else if (fontType == DamageFontType.Heal) {
            m_HealText.text = damage.ToString();
            m_HealTextAnim.Play("DamageFont");
        }
        else if (fontType == DamageFontType.Critical) {
            m_CriticalText.text = damage.ToString();
            m_CriticalTextAnim.Play("DamageFont");
        }
    }

    public void UpdateHP() {
        m_HPBar.fillAmount = m_CurHP / m_MaxHP;
        if(m_HPValue != null) {
            m_HPValue.text = string.Format("{0}/{1}", (int)m_CurHP, (int)m_MaxHP);
        }

        m_DamageHP = Mathf.Clamp(m_HPBar.fillAmount - 0.01f, 0f, 1f);
        m_DamageBar.DOFillAmount(m_DamageHP, _FillAmount_Time).SetEase(Ease.InCirc);
    }
}
