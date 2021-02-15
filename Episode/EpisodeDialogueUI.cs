using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EpisodeDialogueUI : MonoBehaviour
{
    [SerializeField] Image                  m_Background;

    [SerializeField] Image                  m_FastButtonImage;
    [SerializeField] Image                  m_AutoButtonImage;

    [Header("[Dialogue]")]
    [SerializeField] Image                  m_LeftPortrait;
    [SerializeField] Image                  m_RightPortrait;
    [SerializeField] Text                   m_NameText;
    [SerializeField] Text                   m_ContentsText;
    [SerializeField] GameObject             m_NextCursor;

    // 현재 번호
    private EpisodeData                     m_EpisodeData;
    private Dialogue                        m_CurDialogue;
    private int                             m_Arr;
    private bool                            m_IsTypingEnd;      // 타이핑 치는 것 끝났는지
    private DialogueTalkerPos               m_CurDialoguePos;
    private WaitForSeconds                  m_TypingWaitTerm = new WaitForSeconds(0.06f);

    private bool                            m_IsFastText;       // 2배속
    private bool                            m_IsAutoText;       // 자동
    private bool                            m_IsActiveText;     


    private static readonly Color _Hide_Char_Color = new Color(0.4f, 0.4f, 0.4f);
    private static readonly Color _Null_Char_Color = new Color(1f, 1f, 1f, 0f);

    private static readonly Color _On_Button_Color = Color.yellow;
    private static readonly Color _Off_Button_Color = Color.white;

    private void Start() {
        m_IsTypingEnd = true;

        m_IsFastText = false;
        m_IsAutoText = false;
        m_IsActiveText = true;
    }

    public void SetEpisode(EpisodeData episodeData) {
        m_Arr = -1;
        m_EpisodeData = episodeData;
        m_Background.sprite = episodeData._DiagloueBackground;

        PlayEpisode();
    }

    public void EndEpisode(EpisodeData episodeData) {
        m_EpisodeData = episodeData;
        m_Background.sprite = episodeData._DiagloueBackground;
        m_Arr = episodeData._DialogueDatas.Length - 1;

        StopCoroutine("TypingText");

        SetDiaglogueUI();
        m_ContentsText.text = m_CurDialogue._Contents;

        EpisodeUIMgr.Instance.NextEpisode();
    }

    public void PlayEpisode() {
        if(++m_Arr >= m_EpisodeData._DialogueDatas.Length) {
            EpisodeUIMgr.Instance.NextEpisode();
            return;
        }

        SetDiaglogueUI();
        PlayDialogue(m_CurDialogue._DialogueType);
    }

    private void SetDiaglogueUI() {
        m_IsTypingEnd = false;
        m_NextCursor.SetActive(false);
        m_CurDialogue = m_EpisodeData._DialogueDatas[m_Arr];

        m_CurDialoguePos = m_CurDialogue._TalkerPos;

        m_LeftPortrait.sprite = m_CurDialogue._LeftPortrait;
        m_RightPortrait.sprite = m_CurDialogue._RightPortrait;
        m_LeftPortrait.color = m_CurDialogue._TalkerPos == DialogueTalkerPos.Left ? Color.white :
                                                        (m_LeftPortrait.sprite != null ? _Hide_Char_Color : _Null_Char_Color);
        m_RightPortrait.color = m_CurDialogue._TalkerPos == DialogueTalkerPos.Right ? Color.white :
                                                        (m_RightPortrait.sprite != null ? _Hide_Char_Color : _Null_Char_Color);
        m_NameText.text = m_CurDialogue._Name;
    }

    public void PlayDialogue(DialogueType dialogueType) {

        if (dialogueType == DialogueType.Once) {
            m_ContentsText.text = m_CurDialogue._Contents;
            TextEnd();
        }
        else if(dialogueType == DialogueType.Typing) {
            StopCoroutine("TypingText");
            StartCoroutine("TypingText");
        }
    }

    IEnumerator TypingText() {
        m_ContentsText.text = "";
        string typingText = m_CurDialogue._Contents;

        foreach (var letter in typingText.ToCharArray()) {
            yield return m_TypingWaitTerm;
            if (m_ContentsText.text != typingText) {        // **중간에 스킵할 경우 텍스트가 다음으로 넘어감
                m_ContentsText.text += letter;
            }
        }

        TextEnd();
    }

    /// <summary>
    /// 한번의 텍스트가 다 나왔을 때
    /// </summary>
    void TextEnd() {
        m_IsTypingEnd = true;
        if(!gameObject.activeSelf) {
            return;
        }

        m_NextCursor.SetActive(true);

        if(m_IsAutoText) {
            StopCoroutine("AutoText");
            StartCoroutine("AutoText");
        }
    }

    IEnumerator AutoText() {
        yield return new WaitForSeconds(2.0f);
        // 한번 더 체크 필요한가(Text진행중인지)
        if (m_IsTypingEnd && m_IsAutoText) {
            PlayEpisode();
        }
    }

    #region interaction

    public void Click_Dialogue() {
        if (m_IsTypingEnd) {
            PlayEpisode();
        }
        else {
            StopCoroutine("TypingText");
            PlayDialogue(DialogueType.Once);
        }
    }

    public void Click_Fast() {
        m_IsFastText = !m_IsFastText;
        m_TypingWaitTerm = m_IsFastText ? new WaitForSeconds(0.03f) : new WaitForSeconds(0.06f);
        m_FastButtonImage.color = m_IsFastText ? _On_Button_Color : _Off_Button_Color;
    }

    public void Click_Auto() {
        m_IsAutoText = !m_IsAutoText;
        m_AutoButtonImage.color = m_IsAutoText ? _On_Button_Color : _Off_Button_Color;

        StopCoroutine("AutoText");
        if (m_IsAutoText) {
            StartCoroutine("AutoText");
        }
    }

    public void Click_Skip() {
        EpisodePopupUIMgr.Instance.Show(EpisodePopupType.Skip);
    }

    #endregion
}
