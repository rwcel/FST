using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldLevel : MonoBehaviour
{
    #region serialized fields

    [SerializeField] Text       m_GoldRewardText;       // 999.99Z/999s
    [SerializeField] Text       m_GoldUpCostText;

    [SerializeField] Button     m_UpButton;
    [SerializeField] Slider     m_GoldSlider;

    private int                 m_GetUp;
    private double              m_UpPrice;

    #endregion


    #region private varialbes

    private PlayerMgr           m_PlayerMgr;
    private PrefsMgr            m_PrefsMgr;
    private SystemMgr           m_SystemMgr;

    #endregion


    #region default functions

    private void Start() {
        m_PlayerMgr = PlayerMgr.Instance;
        m_PrefsMgr = PrefsMgr.Instance;
        m_SystemMgr = SystemMgr.Instance;

        SetLevelUpCost();

        StopCoroutine("UpdateContents");
        StartCoroutine("UpdateContents");
    }

    #endregion


    #region public functions

    public void SetLevelUpCost() {
        m_GetUp = m_PrefsMgr.GetUp();
        m_UpPrice = m_PlayerMgr.GetFairyLvUpCost(m_GetUp);
        m_GoldUpCostText.text = m_PlayerMgr.GetFairyIsMaxLv() ?
                        "MAX" : m_SystemMgr.GetDoubleString(m_PlayerMgr.GetFairyLvUpCost(m_GetUp));
        m_UpButton.interactable = (PlayerMgr.Instance.GetResource(PaymentType.Gold) >= m_UpPrice);

        m_GoldRewardText.text = string.Format("<color=#FF0090>{0}</color>/{1}s",
                        SystemMgr.Instance.GetDoubleString(m_PlayerMgr.GetFairyReward()), m_PlayerMgr.GetFairyTime());
    }

    #endregion

    #region interaction

    public void Click_GoldLevelUp() {
        ResourceMgr.Instance.PlaySound("FX", 0);
        if (m_PlayerMgr.UseResource(PaymentType.Gold, m_UpPrice) == true) {
            m_PlayerMgr.AddFairyLv(m_GetUp);
            SetLevelUpCost();
        }

        if (PrefsMgr.Instance._IsLvUpMax) {
            while (true) {          // Max : 계속 더해주고 마지막에 적용
                m_UpPrice = m_PlayerMgr.GetFairyLvUpCost(m_GetUp);
                if (m_PlayerMgr.UseResource(PaymentType.Gold, m_UpPrice, false)) {
                    m_PlayerMgr.AddFairyLv(m_GetUp);
                }
                else {
                    SetLevelUpCost();
                    return;
                }
            }
        }
        else {      // 1, 10레벨업
            if (!m_PlayerMgr.UseResource((int)PaymentType.Gold, m_UpPrice)) {
                m_PlayerMgr.AddFairyLv(m_GetUp);
                SetLevelUpCost();
            }
        }
    }


    #endregion

    private double m_WorkStartedTime;
    private double m_TotalWorkTime;
    private double m_WorkDuration;

    private IEnumerator UpdateContents() {
        var waitTerm = new WaitForSecondsRealtime(0.05f);
        m_WorkStartedTime = m_PlayerMgr.GetFairyWorkTime();
        if (m_WorkStartedTime == 0) {
            m_PlayerMgr.StartFairyWork();
        }

        while (true) {
            m_TotalWorkTime = SystemMgr.Instance.GetTimestamp() - m_WorkStartedTime;
            m_WorkDuration = m_PlayerMgr.GetFairyTime();
            m_GoldSlider.value = (float)m_TotalWorkTime / (float)m_WorkDuration;
            if (m_TotalWorkTime >= m_WorkDuration) {    // 경험치 획득
                m_PlayerMgr.AddResource(PaymentType.Gold, m_PlayerMgr.GetFairyReward());
                m_PlayerMgr.StartFairyWork();
                m_WorkStartedTime = m_PlayerMgr.GetFairyWorkTime();
                SetLevelUpCost();
            }
            yield return waitTerm;
        }
    }



}
