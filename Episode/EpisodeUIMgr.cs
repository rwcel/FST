using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class EpisodeUIMgr : SingleTon<EpisodeUIMgr>
{
    [Header("Episode")]
    [SerializeField] GameObject             m_EpisodeObj;
    [SerializeField] EpisodeDialogueUI      m_DialogueUI;
    // **버튼 색 변경 조절

    [Header("HScene")]
    [SerializeField] GameObject             m_HSceneObj;
    [SerializeField] HSceneDialogueUI       m_HSceneUI;
    //[SerializeField] SkeletonAnimation      m_Animation;
    // **버튼 색 변경 조절

    private EpisodeType m_EpisodeType;
    private EpisodeData[]                   m_EpisodeDatas;
    private HSceneData                      m_HSceneData;
    private int                             m_EpisodeNum;

    private void Start() {
        m_EpisodeType = EpisodeSelectMgr.Instance._EpisodeType;
        m_EpisodeNum = -1;

        if(m_EpisodeType == EpisodeType.Story) {
            m_EpisodeObj.SetActive(true);
            m_HSceneObj.SetActive(false);
            m_EpisodeDatas = EpisodeSelectMgr.Instance._EpisodeDatas;
            NextEpisode();
        }
        else {
            m_EpisodeObj.SetActive(false);
            m_HSceneObj.SetActive(true);
            m_HSceneData = EpisodeSelectMgr.Instance._HSceneData;
            NextHScene();
        }
    }

    public void NextEpisode() {
        if (++m_EpisodeNum >= m_EpisodeDatas.Length) {      // 모두 종료
            EpisodePopupUIMgr.Instance.Show(EpisodePopupType.HScene);
            return;
        }

        m_DialogueUI.SetEpisode(m_EpisodeDatas[m_EpisodeNum]);
    }

    public void NextHScene() {
        if(++m_EpisodeNum >= 1) {
            TransitionUIMgr.Instance.FadeOut(() => SystemMgr.Instance.MoveScene("Lobby"),0.5f, true);
            return;
        }
        m_HSceneUI.SetScene(m_HSceneData);
    }

    /// <summary>
    /// 팝업에서 보내줌
    /// </summary>
    public void SkipEpisode() {
        m_EpisodeNum = m_EpisodeDatas.Length - 1;
        m_DialogueUI.EndEpisode(m_EpisodeDatas[m_EpisodeNum]);
    }
}
