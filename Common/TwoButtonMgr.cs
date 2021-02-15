using UnityEngine;
using System.Collections;

using System;
using UnityEngine.UI;

public class TwoButtonMgr : VisibleUIMgr<TwoButtonMgr>
{
    /// <summary>
    /// TwoButtonPopupType enum 순서대로 들어간다고 가정
    /// </summary>
    [System.Serializable]
    private struct PopupTypes
    {
        public TwoButtonPopupType   PopupType;
        public GameObject           Obj;
        public Text                 ContentsText;
    }

    [SerializeField] Text           m_TitleText;
    [SerializeField] Button         m_LeftButton;
    [SerializeField] Button         m_RightButton;
    [SerializeField] PopupTypes[]   m_PopupTypes;

    private Animator                m_Animator; 
    private PopupTypes              m_CurPopupType;
    private Text                    m_LeftButtonText;
    private Text                    m_RightButtonText;

    private Action                  m_OnLeftButton;
    private Action                  m_OnRightButton;

    private const string _Animation_SideOnOff = "ShowOnOff";

    protected override void SetData() {
        m_Animator = GetComponent<Animator>();

        m_LeftButtonText = m_LeftButton.GetComponentInChildren<Text>();
        m_RightButtonText = m_RightButton.GetComponentInChildren<Text>();
    }

    #region show

    /// <summary>
    /// contents가 GetLanguage
    /// </summary>
    public void Show(Action onLeftButton, Action onRightButton, int leftBtn, int rightBtn, int title, int contents,
                     TwoButtonPopupType popupType = TwoButtonPopupType.Normal) {
        m_Animator.SetBool(_Animation_SideOnOff, true);

        m_CurPopupType = m_PopupTypes[(int)popupType];
        m_CurPopupType.Obj.SetActive(true);

        m_OnLeftButton = onLeftButton;
        m_OnRightButton = onRightButton;

        m_TitleText.text = CSVMgr.Instance.GetLanguage(title);
        m_CurPopupType.ContentsText.text = CSVMgr.Instance.GetLanguage(contents);
        m_LeftButtonText.text = CSVMgr.Instance.GetLanguage(leftBtn);
        m_RightButtonText.text = CSVMgr.Instance.GetLanguage(rightBtn);

        base.Show();
    }

    /// <summary>
    /// contents가 자체
    /// </summary>
    public void Show(Action onLeftButton, Action onRightButton, int leftBtn, int rightBtn, int title, string contents,
                     TwoButtonPopupType popupType = TwoButtonPopupType.Normal) {
        m_Animator.SetBool(_Animation_SideOnOff, true);

        m_CurPopupType = m_PopupTypes[(int)popupType];
        m_CurPopupType.Obj.SetActive(true);

        m_OnLeftButton = onLeftButton;
        m_OnRightButton = onRightButton;

        m_TitleText.text = CSVMgr.Instance.GetLanguage(title);
        m_CurPopupType.ContentsText.text = contents;
        m_LeftButtonText.text = CSVMgr.Instance.GetLanguage(leftBtn);
        m_RightButtonText.text = CSVMgr.Instance.GetLanguage(rightBtn);

        base.Show();
    }

    /// <summary>
    /// 하위에 코드가 있어서 여는 역할만 하는 것
    /// </summary>
    public void Show(Action onOpen, int leftBtn, int rightBtn, int title, TwoButtonPopupType popupType) {
        m_Animator.SetBool(_Animation_SideOnOff, true);

        m_CurPopupType = m_PopupTypes[(int)popupType];
        m_CurPopupType.Obj.SetActive(true);

        m_TitleText.text = CSVMgr.Instance.GetLanguage(title);
        m_LeftButtonText.text = CSVMgr.Instance.GetLanguage(leftBtn);
        m_RightButtonText.text = CSVMgr.Instance.GetLanguage(rightBtn);

        base.Show();
        if (onOpen != null) {
            onOpen();
        }
    }

    /// <summary>
    /// 내용과 액션을 다른곳에서 추가하는 경우
    /// </summary>
    public void SetAction(Action onLeftButton, Action onRightButton) {
        m_OnLeftButton = onLeftButton;
        m_OnRightButton = onRightButton;
    }

    #endregion show

    #region interaction

    public override void Close() {
        m_Animator.SetBool(_Animation_SideOnOff, false);

        try {
            m_CurPopupType.Obj.SetActive(false);
        }
        catch {
            //Debug.LogError("초기선언 과정중에 오류 한번 발생");
            Debug.Log("초기선언 과정중에 오류 한번 발생");
        }

        base.Close();
    }

    public void Click_Left() {
        Click_Close();

        if (m_OnLeftButton != null) {
            m_OnLeftButton();
        }
    }

    public void Click_Right() {
        Click_Close();

        if (m_OnRightButton != null) {
            m_OnRightButton();
        }
    }

    #endregion interaction

    #region Rebirth

    public void Click_RebirthDia() {
        // money Check

        if(RebirthMgr.Instance.ShowBonus() == true) {
            Click_Close();
        }
    }

    #endregion Rebirth
}
