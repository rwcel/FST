using UnityEngine;

using Facebook.Unity;
using GooglePlayGames;

/// <summary>
/// Ready - TitleUI
/// </summary>
public class ReadyUIMgr : SingleTon<ReadyUIMgr>
{
    [SerializeField] UnityEngine.UI.Text        m_CopyRight;

    void Start()
    {
        ConfigMgr.Instance.LoadConfig();
        m_CopyRight.text = ConfigMgr.Instance.GetCopylight();

        PrefsMgr.Instance.LoadPrefs();
        CSVMgr.Instance.LoadSystemMsg();
        CSVMgr.Instance.LoadWord();

#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
#endif
        FB.Init(FacebookComplete);
    }

    // 콜백함수
    void FacebookComplete()
    {
        SystemMgr.Instance.SetSystem();

        WaitingMgr.Instance.Show();
        SystemMgr.Instance.MoveScene("Title");
    }
}
