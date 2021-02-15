using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisibleTimer : MonoBehaviour
{
    public System.Action        _HideAction;

    [SerializeField] Text       m_TimerText = null;
    [SerializeField] float      m_CloseTime = 5f;

    private float               m_RemainTime;
    private float               m_UpdateTerm = 1.0f;

    private void OnEnable() {
        m_RemainTime = m_CloseTime;

        StopCoroutine("CloseTimer");
        StartCoroutine("CloseTimer");
    }

    IEnumerator CloseTimer() {
        WaitForSecondsRealtime waitTerm = new WaitForSecondsRealtime(m_UpdateTerm);

        while (m_RemainTime > 0f) {
            m_TimerText.text = string.Format(CSVMgr.Instance.GetLanguage(1735), m_RemainTime);
            yield return waitTerm;
            m_RemainTime -= m_UpdateTerm;
        }

        gameObject.SetActive(false);
        if(_HideAction != null) {
            _HideAction();
            _HideAction = null;
        }
    }
}
