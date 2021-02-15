using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개수가 제한 1~5기때문에 이름을 그대로 불러옴
/// </summary>
public class EpisodeHSceneItem : MonoBehaviour
{
    [SerializeField] Image              m_Image;
    [SerializeField] GameObject         m_SelectObject;
    [SerializeField] GameObject         m_CostObject;

    [SerializeField] Text               m_ChoiceText;  
    [SerializeField] Text               m_CostValueText;

    private int                         m_Num;
    private bool                        m_IsClear;         
    private double                      m_CostValue;

    private void Start() {
        m_Num = int.Parse(gameObject.name.Substring(name.Length - 2, 1));
        m_IsClear = PrefsMgr.Instance.GetHSceneClear(EpisodeSelectMgr.Instance._HeroNum, m_Num);

        // **m_Image = 
        m_CostValue = m_IsClear ? 0 : m_Num * 100;
        m_CostValueText.text = string.Format("{0:n0}", m_CostValue.ToString());
        m_ChoiceText.text = "CHOICE";

        m_SelectObject.SetActive(false);
    }

    /// <summary>
    /// 선택 시(true) 비용, 버튼 보여주기
    /// </summary>
    public void SelectHScene(bool isSelect) {
        m_SelectObject.SetActive(isSelect);
        m_CostObject.SetActive(m_CostValue != 0);       // *업적등을 위해 : 0인것도 클리어를 해야하기에 m_IsClear로 조절안함
    }


    #region interaction

    public void Click_Object() {
        EpisodePopupUIMgr.Instance.SelectHScene(m_Num);
    }

    /// <summary>
    /// 선택 : PopupUIMgr에서 처리하게
    /// </summary>
    public void Click_Choice() {
        EpisodePopupUIMgr.Instance.ChoiceHScene(m_Num, m_CostValue, m_IsClear);
    }

    #endregion interaction
}
