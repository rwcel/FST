using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DPS정보
/// **전투중에 증가하지 않고 단순 레벨에 따른 공격력 수치로 사용중입니다.
/// </summary>
public class DPSInfo : MonoBehaviour
{
    [System.Serializable] 
    private struct DPSCharacter
    {
        public GameObject   Obj;
        public Image        Image;
        [HideInInspector]
        public double       DPSValue;
        public Text         DPS;
        public Text         Percent;
    }

    [SerializeField] Animator           m_DPSAnim = null;
    [SerializeField] GameObject         m_DPSInfo = null;
    [SerializeField] Text               m_TotalDPS = null;

    [SerializeField] DPSCharacter[]     m_DPSCharacters;


    private double                      m_TotalDPSValue = 0;
    private bool                        m_ShowDPSInfo;

    private const string _Animation_SideOnOff = "ShowOnOff";


    private void Start() {
        m_ShowDPSInfo = PrefsMgr.Instance.GetShowDPSInfo();
        m_DPSAnim.SetBool(_Animation_SideOnOff, m_ShowDPSInfo);
        SetScreen();
    }

    public void Click_DPS() {
        m_ShowDPSInfo = !m_ShowDPSInfo;

        PrefsMgr.Instance.SetShowDPSInfo(m_ShowDPSInfo);
        m_DPSAnim.SetBool(_Animation_SideOnOff, m_ShowDPSInfo);

        SetScreen();
    }

    private void SetScreen() {
        PlayerMgr playerMgr = PlayerMgr.Instance;
        int heroIDX, heroLv;
        m_TotalDPSValue = 0f;
        for (int i = 0, length = m_DPSCharacters.Length; i < length; i++) {
            heroIDX = playerMgr.GetHeroSlot(i);
            m_DPSCharacters[i].Obj.SetActive(heroIDX >= 0);
            if (heroIDX >= 0) {
                heroLv = playerMgr.GetHeroLv(heroIDX) + 1;
                m_DPSCharacters[i].Image.sprite = ResourceMgr.Instance.GetSprite("HeroFace", heroIDX);
                m_DPSCharacters[i].DPSValue = CSVMgr.Instance.GetTotalATK(heroIDX, heroLv);
                m_DPSCharacters[i].DPS.text = SystemMgr.Instance.GetDoubleString(m_DPSCharacters[i].DPSValue);
                m_TotalDPSValue += m_DPSCharacters[i].DPSValue;
            }
        }
        m_TotalDPS.text = SystemMgr.Instance.GetDoubleString(m_TotalDPSValue);

        for (int i = 0, length = m_DPSCharacters.Length; i < length; i++) {
            m_DPSCharacters[i].Percent.text = string.Format("{0}%", 
                        Mathf.RoundToInt((float)m_DPSCharacters[i].DPSValue / (float)m_TotalDPSValue * 100f));
        }

    }
}
