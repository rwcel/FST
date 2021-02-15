using UnityEngine;
using UnityEngine.UI;

public class ChallengeSubItem : MonoBehaviour
{
    [SerializeField] Text           m_NameText;
    [SerializeField] Text           m_CloseText;

    private ChallengeType           m_Type;

    public void SetScreen(ChallengeType type, string nameText) {
        m_Type = type;

        m_NameText.text = nameText;
        m_CloseText.text = "CLOSE";                 // $ Language 데이터 부족

        if(m_Type == ChallengeType.Daily) {
            SetDailyItem();
        }
        else if (m_Type == ChallengeType.Boss) {
            SetBossItem();
        }
    }

    private void SetDailyItem() {

    }

    private void SetBossItem() {

    }

}
