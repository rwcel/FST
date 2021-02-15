using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 우상단 스테이지 정보
/// </summary>
public class StageInfo : MonoBehaviour
{
    [SerializeField] Text           m_StageName;
    [SerializeField] Text           m_StageValue;
    [SerializeField] Transform      m_StagePointParent;

    private static readonly Color _Color_Cur_Stage      = new Color(1f, 0f, 0.56f);
    private static readonly Color _Color_Clear_Stage    = new Color(0.7f, 0.7f, 0.7f);
    // _Color_NotClear_Stage = White

    private void Start() {
        int stageValue = PlayerMgr.Instance.GetStageInEpisode();
        m_StageName.text = CSVMgr.Instance.GetEpisodeName();
        m_StageValue.text = (stageValue + 1).ToString();

        // 일의자리 계산
        for (int i = 0; i < stageValue; i++) {
            m_StagePointParent.GetChild(i).GetComponent<Image>().color = _Color_Clear_Stage;
        }
        m_StagePointParent.GetChild(stageValue).GetComponent<Image>().color = _Color_Cur_Stage;
    }
}
