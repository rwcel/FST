using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EpisodePopupType
{
    None,
    Skip, HScene,
}

public class EpisodePopupUIMgr : SingleTon<EpisodePopupUIMgr>
{
    public bool _IsOpen { get; private set; }

    [SerializeField] GameObject             m_Visible;

    [Header("Skip")]
    [SerializeField] GameObject             m_SkipPopup;
    [SerializeField] Text                   m_ContentText;
    [SerializeField] Text                   m_ConfirmText;    
    [Header("HScene")]
    [SerializeField] GameObject             m_HScenePopup;
    [SerializeField] Transform              m_HSceneParent;
    [SerializeField] Text                   m_PaymentText;

    private EpisodeHSceneItem[]             m_HSceneItems;
    private int                             m_SelectNum;
    private double                          m_Payment;

    private void Start() {
        _IsOpen = false;

        m_HSceneItems = new EpisodeHSceneItem[m_HSceneParent.childCount];
        for (int i = 0, length = m_HSceneItems.Length; i < length; i++) {
            m_HSceneItems[i] = m_HSceneParent.GetChild(i).GetComponent<EpisodeHSceneItem>();
        }
    }

    public void Show(EpisodePopupType popupType) {
        _IsOpen = (popupType != EpisodePopupType.None);
        m_Visible.SetActive(_IsOpen);
        m_SkipPopup.SetActive(popupType == EpisodePopupType.Skip);
        m_HScenePopup.SetActive(popupType == EpisodePopupType.HScene);

        if(popupType == EpisodePopupType.Skip) {
            //m_ContentText.text = "";
            m_ConfirmText.text = "Confirm";
        }

        if(popupType == EpisodePopupType.HScene) {
            int count = EpisodeSelectMgr.Instance.GetHSceneCount();
            for (int i = 0; i < count; i++) {
                m_HSceneItems[i].gameObject.SetActive(true);
            }
            for (int i = count, length = m_HSceneItems.Length; i < length; i++) {
                m_HSceneItems[i].gameObject.SetActive(false);
            }

            m_Payment = PlayerMgr.Instance.GetResource(PaymentType.Diamond);
            m_PaymentText.text = string.Format("{0:n0}", m_Payment);
            //EpisodeSelectMgr.Instance.SetHScene(m_SelectNum);
        }
    }

    public void SelectHScene(int num) {
        for (int i = 0, length = m_HSceneItems.Length; i < length; i++) {
            m_HSceneItems[i].SelectHScene(false);
        }

        m_HSceneItems[num].SelectHScene(true);
    }
    
    /// <summary>
    /// isClear면 무조건 0으로 들어옴
    /// </summary>
    public void ChoiceHScene(int num, double cost, bool isClear) {
        if(PlayerMgr.Instance.GetResource(PaymentType.Diamond) < cost) {
            return;
        }

        if(!isClear) {
            PrefsMgr.Instance.SetHSceneClear(EpisodeSelectMgr.Instance._HeroNum, num);
        }
        else {
            PlayerMgr.Instance.UseResource(PaymentType.Diamond, cost);
        }

        EpisodeSelectMgr.Instance.SetHScene(num);
        SystemMgr.Instance.MoveScene("Episode");
    }

    #region interaction

    public void Click_SkipEnter() {
        EpisodeUIMgr.Instance.SkipEpisode();
    }

    public void Click_SkipCancel() {
        Show(EpisodePopupType.None);
    }

    public void Click_HSceneEnter() {
        m_HScenePopup.SetActive(false);
        SystemMgr.Instance.MoveScene("Lobby");
    }

    #endregion

}
