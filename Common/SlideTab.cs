using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 움직이는 탭
/// </summary>
public class SlideTab : MonoBehaviour
{
    [System.Serializable] 
    private struct SlideTabs
    {
        public Button       _Button;
        public Text         _Text;
    }

    [SerializeField] Transform          m_Switch = null;
    [SerializeField] Transform          m_Tabs = null;

    private SlideTabs[]                 m_SlideTabs;


    private static readonly Color _Color_Off = new Color(0.7f, 0.7f, 0.7f);
    private static readonly Color _Color_On = Color.black;


    private void Awake() {
        m_SlideTabs = new SlideTabs[m_Tabs.childCount];

        for (int i = 0, length = m_Tabs.childCount; i < length; i++) {
            Transform tr = m_Tabs.GetChild(i);
            m_SlideTabs[i]._Button  = tr.GetComponent<Button>();
            m_SlideTabs[i]._Text    = tr.GetComponent<Text>();
        }
    }

    /// <summary>
    /// 버튼 세팅
    /// </summary>
    /// <param name="num">슬라이드 배열</param>
    /// <param name="unDevelop">**개발중 표시 (나중에 지우기)</param>
    public void SetButton(int num, string text, Action onClick, bool isOpen = false, bool unDevelop = false) {
        m_SlideTabs[num]._Text.text = text;

        // ****개발 못해서 생긴 것. 나중에 지우고 밑에 주석처리 살리기
        if (!unDevelop) {
            m_SlideTabs[num]._Button.onClick.AddListener(() => { onClick(); SetSlideTab(num); });
        }
        else {
            m_SlideTabs[num]._Button.onClick.AddListener(() => { onClick(); if (!unDevelop) { SetSlideTab(num); }; });
        }

        //m_SlideTabs[num]._Button.onClick.AddListener(() => onClick());
        //m_SlideTabs[num]._Button.onClick.AddListener(delegate { SetButton(num); });

        if (isOpen) {
            ClickButton(num);
        }
    }


    private void SetSlideTab(int select) {
        for (int i = 0, length = m_SlideTabs.Length; i < length; i++) {
            m_SlideTabs[i]._Text.color = _Color_Off;
        }
        m_Switch.DOMoveX(m_SlideTabs[select]._Button.transform.position.x, 0.5f).OnComplete(
            () => {
                // 계속 바꿀경우 2개 이상이 블랙 처리 될 수 있기에 한번더 회색처리
                for (int i = 0, length = m_SlideTabs.Length; i < length; i++) {
                    m_SlideTabs[i]._Text.color = _Color_Off;
                }
                m_SlideTabs[select]._Text.color = _Color_On;
            });
    }

    public void ClickButton(int num) {
        m_SlideTabs[num]._Button.onClick.Invoke();      // OnClick 반응 호출
    }
}
