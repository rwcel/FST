using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;

public class HSceneDialogueUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] GameObject         m_ButtonObj;
    [SerializeField] Button             m_SkipButton;
    [SerializeField] Button             m_AutoButton;
    [SerializeField] Button             m_HideButton;
    [SerializeField] Button             m_ExitButton;

    [Header("[Dialogue]")]
    [SerializeField] GameObject         m_DialgoueObj;
    [SerializeField] SkeletonAnimation  m_Animation;
    [SerializeField] Text               m_NameText;
    [SerializeField] Text               m_ContentsText;
    [SerializeField] GameObject         m_NextCursor;

    private HSceneData                  m_HSceneData;
    private HSceneLog                   m_CurDialogue;
    private int                         m_Arr;
    private bool                        m_IsTypingEnd;      // 타이핑 치는 것 끝났는지
    private WaitForSeconds              m_TypingWaitTerm = new WaitForSeconds(0.06f);

    private bool                        m_IsAutoText;       // 자동
    private bool                        m_IsHideText;

    private Coroutine                   m_EndCoroutine;

    private static readonly Color _On_Button_Color = Color.yellow;
    private static readonly Color _Off_Button_Color = Color.white;


    private void Start() {
        m_IsTypingEnd = true;

        m_IsAutoText = false;
        m_IsHideText = false;
        m_EndCoroutine = null;
    }

    public void SetScene(HSceneData hSceneData) {
        m_Arr = -1;
        m_HSceneData = hSceneData;
        //m_Animation.skeletonDataAsset = hSceneData._SpineAnimation.skeletonDataAsset;

        PlayScene();
    }

    public void EndScene(HSceneData hSceneData) {
        //m_Background.sprite = episodeData._DiagloueBackground;
        m_Arr = hSceneData._DialogueDatas.Length - 2;

        // 마지막이 Typing일때 Once로 변경하기
        StopCoroutine("TypingText");
        PlayDialogue(DialogueType.Once);

        PlayScene();
    }

    public void PlayScene() {
        if (++m_Arr >= m_HSceneData._DialogueDatas.Length) {
            EpisodeUIMgr.Instance.NextHScene();
            return;
        }

        m_IsTypingEnd = false;
        m_NextCursor.SetActive(false);

        m_CurDialogue = m_HSceneData._DialogueDatas[m_Arr];

        m_NameText.text = m_CurDialogue._Name;

        PlayDialogue(m_CurDialogue._DialogueType);
        PlayScene(m_CurDialogue._HSceneType);
    }

    public void PlayDialogue(DialogueType dialogueType) {

        if (dialogueType == DialogueType.Once) {
            m_ContentsText.text = m_CurDialogue._Contents;
            TextEnd();
        }
        else if (dialogueType == DialogueType.Typing) {
            StopCoroutine("TypingText");
            StartCoroutine("TypingText");
        }
        else if(dialogueType == DialogueType.None)  {       // Once + None
            m_ContentsText.text = m_CurDialogue._Contents;
            TextEnd();

            Click_Hide();
        }
    }

    public void PlayScene(HSceneType sceneType) {
        if (sceneType == HSceneType.Normal) {
            var trackEntry = m_Animation.state.SetAnimation(0, m_CurDialogue._AnimationName, true);
            trackEntry.TimeScale = 1f;
        }
        else if (sceneType == HSceneType.Fast) {
            var trackEntry = m_Animation.state.SetAnimation(0, m_CurDialogue._AnimationName, true);
            trackEntry.TimeScale = 1.5f;
        }
        else if (sceneType == HSceneType.Finish) {
            // white Fade in out
            TransitionUIMgr.Instance.FadeOut(() => { m_Animation.state.SetAnimation(0, m_CurDialogue._AnimationName, false);
                                                     Click_Dialogue(); },
                                                  2.0f, false, new Color(1f,1f,1f,0.6f));
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
    private void TextEnd() {
        m_IsTypingEnd = true;

        m_NextCursor.SetActive(true);

        if (m_IsAutoText) {
            StopCoroutine("AutoText");
            StartCoroutine("AutoText");
        }
    }

    IEnumerator AutoText() {
        yield return new WaitForSeconds(2.0f);
        // 한번 더 체크 필요한가(Text진행중인지)
        if (m_IsTypingEnd && m_IsAutoText) {
            PlayScene();
        }
    }

    IEnumerator EndScene(float waitTerm) {
        yield return new WaitForSeconds(2.0f);

        EpisodeUIMgr.Instance.NextHScene();
    }


    #region interaction

    public void Click_Auto() {
        m_IsAutoText = !m_IsAutoText;
        m_AutoButton.image.color = m_IsAutoText ? _On_Button_Color : _Off_Button_Color;

        StopCoroutine("AutoText");
        if (m_IsAutoText) {
            StartCoroutine("AutoText");
        }
    }

    public void Click_Dialogue() {
        if(m_IsHideText) {
            m_IsHideText = false;

            m_DialgoueObj.SetActive(true);
            m_ButtonObj.SetActive(true);
            // m_ContentsText.text = m_CurDialogue._Contents;
            // TextEnd();
            return;
        }

        if (m_IsTypingEnd) {
            PlayScene();
        }
        else {
            StopCoroutine("TypingText");
            PlayDialogue(DialogueType.Once);
        }
    }

    /// <summary>
    /// Finish 재생구역까지 찾기
    /// </summary>
    public void Click_Skip() {
        // m_SkipButton.image.color = _On_Button_Color;

        while(m_Arr < m_HSceneData._DialogueDatas.Length - 1) { 
            if(m_HSceneData._DialogueDatas[++m_Arr]._HSceneType == HSceneType.Finish) {
                m_Arr--;
                break;
            }
        }

        PlayScene();
    }

    public void Click_Hide() {
        m_IsHideText = true;
        m_DialgoueObj.SetActive(false);
        m_ButtonObj.SetActive(false);

        if (m_IsAutoText) {
            Click_Auto();       // Hide시키기
        }
    }

    public void Click_Exit() {
        m_ExitButton.image.color = _On_Button_Color;
        TwoButtonMgr.Instance.Show(() => m_ExitButton.image.color = _Off_Button_Color,
                                   () => EpisodeUIMgr.Instance.NextHScene(), 
                                   3, 2, 772, "Are you sure to Exit");
    }

    #endregion interaction
}