using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroBtn : MonoBehaviour
{
    private enum Images
    {
        Face,
        Grade,
        Cooltime
    }

    private enum Texts
    {
        Level,
        LevelUpCost,
    }

    public System.Action                _LevelUpAction;

    [SerializeField] Image[]            m_Images;
    [SerializeField] Text[]             m_Texts;
    [SerializeField] Transform          m_StarParent;
    [SerializeField] Button             m_LvUpButton;
    [SerializeField] ParticleSystem     m_LvUpParticle;

    private int                         m_Num;
    private int                         m_TableNum;
    private int                         m_IDX;
    private int                         m_Level;

    private int                         m_LvGetUp;
    private double                      m_LvUpPrice;

    private bool                        m_CanSkill;

    private HeroMgr                     m_HeroMgr;
    private PlayerMgr                   m_PlayerMgr;


    private readonly string _On_Face_Scene = "Stage Boss";

    /// <summary>
    /// 한번만 받는 과정
    /// </summary>
    public void SetData(HeroMgr heroMgr) {
        m_HeroMgr = heroMgr;
        m_PlayerMgr = PlayerMgr.Instance;

        m_TableNum = m_HeroMgr._TableNum;
        if (m_TableNum < 0) {
            gameObject.SetActive(false);
            return;
        }

        m_Num = int.Parse(gameObject.name.Substring(name.Length - 2, 1));
        //Debug.Log(m_Num+") 누구야 :"+unitTableNum);
        m_IDX = m_PlayerMgr.GetHeroSlot(m_Num);

        SetScreen();
        SetLevel();
    }

    public void SetScreen() {
        m_Images[(int)Images.Face].sprite = ResourceMgr.Instance.GetSprite("HeroFace", m_IDX % 4);        // $
        for (int i = m_PlayerMgr.GetHeroAwaken(m_TableNum) + 1, length = m_StarParent.childCount; i < length; i++) {
            m_StarParent.GetChild(i).gameObject.SetActive(false);
        }
        m_Images[(int)Images.Cooltime].fillAmount = 1;

        StopCoroutine("SkillCooltime");
        StartCoroutine("SkillCooltime");
    }

    /// <summary>
    /// 레벨 설정
    /// </summary>
    public void SetLevel() {
        m_Level = m_PlayerMgr.GetHeroLv(m_IDX);
        m_Texts[(int)Texts.Level].text = string.Format("Lv {0}", m_Level + 1);

        if (_LevelUpAction != null) {
            _LevelUpAction();
        }

        SetLevelUpCost();
    }

    public void SetLevelUpCost() {
        m_LvGetUp = PrefsMgr.Instance.GetUp();
        m_LvUpPrice = GetLvUpPrice(m_IDX, m_Level, m_LvGetUp);
        m_Texts[(int)Texts.LevelUpCost].text = SystemMgr.Instance.GetDoubleString(m_LvUpPrice);     // *더 호출하긴 함
        m_LvUpButton.interactable = (PlayerMgr.Instance.GetResource(PaymentType.Gold) >= m_LvUpPrice);
    }

    public void Click_LevelUp() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        if (m_IDX < 0) {
            return;
        }

        if (PrefsMgr.Instance._IsLvUpMax) {     // MaxLevel
            int upValue = m_Level;
            while (true) {                      // *한번에 할 수 없음 (현재 돈과에 맞춰서 계산을 해야하기에)
                m_LvUpPrice = GetLvUpPrice(m_IDX, upValue, m_LvGetUp);
                // Debug.Log(m_LvUpPrice + "<" + PlayerMgr.Instance.GetResource((int)PaymentType.Gold));
                if (m_PlayerMgr.UseResource(PaymentType.Gold, m_LvUpPrice, false)) {
                    upValue++;
                }
                else {
                    if(upValue == m_Level) {
                        return;
                    }
                    upValue -= m_Level;
                    m_PlayerMgr.SetHeroLv(m_IDX, upValue);
                    m_HeroMgr.SetStatus();
                    m_PlayerMgr.AddAchievementValue(0, upValue);
                    m_PlayerMgr.AddEventValue(0, upValue);
                    SetLevel();

                    m_LvUpParticle.Play();
                    return;
                }
            }
        }

        // 1, 10 LvUp
        if (m_PlayerMgr.UseResource(PaymentType.Gold, m_LvUpPrice)) {
            m_PlayerMgr.SetHeroLv(m_IDX, m_LvGetUp);

            m_HeroMgr.SetStatus();

            m_PlayerMgr.AddAchievementValue(0, m_LvGetUp);
            m_PlayerMgr.AddEventValue(0, m_LvGetUp);

            SetLevel();

            m_LvUpParticle.Play();
        }
    }

    // 얼굴 클릭시 스킬
    public void Click_Skill() {
        // CSVMgr.Instance.GetAwaken(unitTableNum) <= 0 : return
        if(!m_CanSkill) {
            return;
        }

        if(BattleMgr.Instance.SkillReady(m_HeroMgr._BattleCharacter) == false) {
            return;     // 이미 스킬 사용중이면 쿨타임 돌리지 않기
        }

        StopCoroutine("SkillCooltime");
        StartCoroutine("SkillCooltime");
    }

    IEnumerator SkillCooltime() {
        WaitForSeconds waitTerm = new WaitForSeconds(0.2f);
        WaitUntil waitUntil = new WaitUntil(() => m_HeroMgr.GetSkillCoolPercent() >= 0f);

        //Debug.Log(gameObject.name+","+ m_HeroMgr.GetSkillCoolSec());
        m_CanSkill = false;

        m_Images[(int)Images.Cooltime].fillAmount = 1;

        yield return waitUntil;         // **스킬 쿨 적용이 늦어서 안돌아감

        while (m_Images[(int)Images.Cooltime].fillAmount > 0.01f) {
            m_Images[(int)Images.Cooltime].fillAmount = m_HeroMgr.GetSkillCoolPercent();
            // Debug.Log(gameObject.name + "," + m_HeroMgr.GetSkillCoolPercent()*10);
            yield return waitTerm;
        }

        m_Images[(int)Images.Cooltime].fillAmount = 0;
        m_CanSkill = true;
    }

    private double GetLvUpPrice(int _Num, int _LV, int _Up) {
        double _CostPer = 1 + (CSVMgr.Instance.GetCostPer(_Num) / 1000.0);
        double _A = CSVMgr.Instance.GetCost(_Num) * System.Math.Pow(_CostPer, _LV);
        double _B = (1 - System.Math.Pow(_CostPer, _Up)) / (1 - _CostPer);
        float _SkillRate = (1000.0f - (PlayerMgr.Instance.GetPlayerSkillBuff(AbilityType.ReduceHeroLvUpCostRate) + 
                                    PlayerMgr.Instance.GetFairySkillBuff(AbilityType.ReduceHeroLvUpCostRate))) / 1000.0f;
        return System.Math.Round(_A * _SkillRate * _B);
    }
}
