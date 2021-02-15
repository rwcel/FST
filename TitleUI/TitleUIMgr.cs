using UnityEngine;
using System.Collections;
using System.Text;

using SimpleJSON;

public class TitleUIMgr : SingleTon<TitleUIMgr>
{
    [SerializeField] UnityEngine.UI.Text m_CopyRight;
    [SerializeField] UnityEngine.UI.Text m_StartText;

    private bool m_IsStart;

    private float _TimeOut = 20f;

    void Start()
    {
        m_IsStart = false;

        if (PrefsMgr.Instance.GetLanguage() != 1) {
            PrefsMgr.Instance.SetLanguage(1);
        }

        m_CopyRight.text = ConfigMgr.Instance.GetCopylight();
        m_StartText.text = CSVMgr.Instance.GetSystemMsg((int)SystemMsgType.GameStart);
        //_TitleTexture.mainTexture = Resources.Load<Texture>("Textures/TitleUI/Background");

        WaitingMgr.Instance.Close();

        UnityEngine.Time.timeScale = 1f;
    }

    public void Click_Title() {
        if(m_IsStart) {
            return;
        }

        WaitingMgr.Instance.Show();

        StopCoroutine("TestCheck");
        StartCoroutine("TestCheck");
        m_IsStart = true;
    }

    public void Maintenance()
    {
        OneButtonMgr.Instance.Show(SystemMgr.Instance.Quit, 2, 772, 1);
    }

    IEnumerator TestCheck()
    {
        WWW www = new WWW(ConfigMgr.Instance.GetTestCheck());

        StartCoroutine("TimeOut");

        yield return www;

        StopCoroutine("TimeOut");

        if (www.error != null)
        {
            SystemMgr.Instance.Restart();

            yield break;
        }
        else
        {
            JSONNode _Json = JSON.Parse(www.text);

            if (SystemMgr.Instance._Version >= int.Parse(_Json["Version"]))
            {
                ConfigMgr.Instance.SetServerType(false);
                ConfigMgr.Instance.SetIP(_Json["IP"]["Test"]);                
                CSVMgr.Instance.SetCDN(_Json["CDN"]["Test"]);
            }
            else
            {
                ConfigMgr.Instance.SetServerType(true);

                switch(ConfigMgr.Instance.GetCountry())
                {
                    case "Global":
                        ConfigMgr.Instance.SetIP(_Json["IP"]["Global"]);
                        CSVMgr.Instance.SetCDN(_Json["CDN"]["Global"]);
                        break;
                    default:
                        ConfigMgr.Instance.SetIP(_Json["IP"]["Live"]);
                        CSVMgr.Instance.SetCDN(_Json["CDN"]["Live"]);
                        break;
                }
                
            }

            ConfigMgr.Instance.SetChannel(int.Parse(_Json["Channel"]));

            TermCheck();
        }
    }

    // 일정 시간동안 응답이 없으면 재시작
    IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(_TimeOut);

        SystemMgr.Instance.Restart();
    }

    void TermCheck()
    {
        //if (PrefsMgr.Instance.GetTerm() != 0)
        //{

        //}
        //else
        //{
        //    switch (ConfigMgr.Instance.GetCountry())
        //    {
        //        case "Global":
        //            if (PrefsMgr.Instance.GetLanguage() == 0)
        //            {
        //                WaitingMgr.Instance.Close();

        //                TermMgr.Instance.Show(false);
        //            }
        //            else
        //            {
        //                LoginCheck();
        //            }
        //            break;
        //        default:
        //            WaitingMgr.Instance.Close();

        //            TermMgr.Instance.Show(false);
        //            break;
        //    }
        //}

        LoginCheck();
    }

    public void LoginCheck()
    {
        ResourceMgr.Instance.PlaySound("FX", 0);

        PrefsMgr.Instance.SetLoginType("UUID");
        PrefsMgr.Instance.SetLoginID(SystemInfo.deviceUniqueIdentifier);

        CSVMgr.Instance.SetCSV();

        //switch (PrefsMgr.Instance.GetLoginType())
        //{
        //    case "None":
        //        WaitingMgr.Instance.Close();

        //        LoginMgr.Instance.Show();
        //        break;
        //    default:
        //        CSVMgr.Instance.SetCSV();
        //        break;
        //}
    }

    public void Login()
    {
        PlayerMgr.Instance.SetArray();

        NetworkMgr.Instance.StartPost();
        NetworkMgr.Instance.Time();

        NetworkMgr.Instance.Login();
    }

    public void ChannelCheck()
    {
        switch(ConfigMgr.Instance.GetMarketType())
        {
            case 0:
            case 1:
                UnityIAPMgr.Instance.SetUnityIAP();
                break;
            case 2:
                OneStoreMgr.Instance.SetOneStore();
                break;
        }

        if (PlayerMgr.Instance.GetChannel() == 0)
        {
            PlayerMgr.Instance.SetChannel(0);
            NetworkMgr.Instance.UpdateChannel();

            SystemMgr.Instance.MoveScene("Lobby");
            //ChannelMgr.Instance.Show();
        }
        else
        {
            PlayerMgr.Instance.SetTempleCool();

            SystemMgr.Instance.MoveScene("Lobby");

            //if (PrefsMgr.Instance.GetPrologueShowed() == false && PlayerMgr.Instance.GetLv() <= 0) {
            //    SystemMgr.Instance.MoveScene("Prologue");

        // 최초 접속 보상
            //    if (PlayerMgr.Instance.GetFirstAccess() == 1) {
            //        PlayerMgr.Instance.SetFirstAccess(0);

            //        int _Attendance = Mathf.Clamp(PlayerMgr.Instance.GetAttendance(), 0, CSVMgr.Instance.GetAttendanceCnt() - 1);
            //        int _RewardType = CSVMgr.Instance.GetAttendanceRewardType(_Attendance);
            //        int _RewardIDX = CSVMgr.Instance.GetAttendanceRewardIDX(_Attendance);
            //        int _RewardCount = CSVMgr.Instance.GetAttendanceRewardCount(_Attendance);

            //        NetworkMgr.Instance.InsertMail(1084, 1085, _RewardType, _RewardIDX, _RewardCount);

            //        AttendanceMgr.Instance.Show();
            //    }

            //}
            //else {
            //    if (PlayerMgr.Instance.GetFairyWorkTime() == 0) {
            //        SystemMgr.Instance.MoveScene("Lobby");
            //    }
            //    else {
            //        SystemMgr.Instance.MoveScene("OfflineReward");
            //    }
            //}
        }
    }

    public void Version()
    {
        Application.OpenURL(ConfigMgr.Instance.GetStore());

        SystemMgr.Instance.Quit();
    }
}
