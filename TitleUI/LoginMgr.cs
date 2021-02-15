using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Facebook.Unity;
using GooglePlayGames;
using UnityEngine.SignInWithApple;

public class LoginMgr : NormalPopup<LoginMgr>
{
    private enum Labels
    {
        Login,
        Google,
        Apple,
        Facebook,
        Guest,
    }

    private enum Objects
    {
        Google,
        Apple,
    }

	[SerializeField] UILabel[] _Labels;
    [SerializeField] GameObject[] _Obj;

    private List<string> _Permissions = new List<string>();

    public void Show()
    {
        base.Open();

        _Labels[(int)Labels.Login].text = CSVMgr.Instance.GetSystemMsg((int)SystemMsgType.Login);

        _Labels[(int)Labels.Apple].text = CSVMgr.Instance.GetSystemMsg((int)SystemMsgType.Login_GameCenter);
        _Labels[(int)Labels.Google].text = CSVMgr.Instance.GetSystemMsg((int)SystemMsgType.Login_Google);
        _Labels[(int)Labels.Facebook].text = CSVMgr.Instance.GetSystemMsg((int)SystemMsgType.Login_Facebook);
        _Labels[(int)Labels.Guest].text = CSVMgr.Instance.GetSystemMsg((int)SystemMsgType.Login_Guest);

        if (ConfigMgr.Instance.GetMarketType() == (int)MarketType.Apple) {
            _Obj[(int)Objects.Apple].SetActive(true);
            _Obj[(int)Objects.Google].SetActive(false);
        }
        else {      // Google, OneStore
            _Obj[(int)Objects.Apple].SetActive(false);
            _Obj[(int)Objects.Google].SetActive(true);
        }
    }

    public void Close()
    {
        base.Close();

        WaitingMgr.Instance.Show();

        CSVMgr.Instance.SetCSV();
    }

    public void Click_Google()
    {
        ResourceMgr.Instance.PlaySound("FX", 0);

        if (!Application.isEditor)
        {
            WaitingMgr.Instance.Show();

            Social.localUser.Authenticate(Google_AuthBack);
        }
        else
        {
            PrefsMgr.Instance.SetLoginType("GPGS");
            PrefsMgr.Instance.SetLoginID(SystemMgr.Instance._GoogleID);

            Close();
        }
    }

    void Google_AuthBack(bool _Result)
    {
        WaitingMgr.Instance.Close();

        if (_Result)
        {
            PrefsMgr.Instance.SetLoginType("GPGS");
            PrefsMgr.Instance.SetLoginID(Social.localUser.id);

            Close();
        }
    }

    public void Click_Apple() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        if (!Application.isEditor) {
            WaitingMgr.Instance.Show();

            //SignInWithAppleButtonPressed
            var signInWithApple = AppleMgr.Instance._SignInWithApple;
            signInWithApple.Login(Apple_AuthBack);

        }
        else if (SystemMgr.Instance._AppleID != "") {
            PrefsMgr.Instance.SetLoginType("Apple");
            PrefsMgr.Instance.SetLoginID(SystemMgr.Instance._AppleID);

            Close();
        }
        else {      // Editor 상에서 자동으로 만들어지게
            SystemMgr.Instance._AppleID = "Apple_Test";
            PrefsMgr.Instance.SetLoginType("Apple");
            PrefsMgr.Instance.SetLoginID(SystemMgr.Instance._AppleID);

            Close();
        }

    }

    void Apple_AuthBack(SignInWithApple.CallbackArgs _Result) {
        WaitingMgr.Instance.Close();

        if (_Result.error != null) {
            return;
        }

        PrefsMgr.Instance.SetLoginType("Apple");
        PrefsMgr.Instance.SetLoginID(_Result.userInfo.userId);

        Close();
    }

    public void Click_Facebook()
    {
        ResourceMgr.Instance.PlaySound("FX", 0);

        if (!Application.isEditor)
        {
            WaitingMgr.Instance.Show();

            _Permissions.Clear();
            _Permissions.Add("email");
            _Permissions.Add("public_profile");
            _Permissions.Add("user_friends");

            FB.LogInWithReadPermissions(_Permissions, Facebook_AuthBack);
        }
        else if (SystemMgr.Instance._FacebookID != "")
        {
            PrefsMgr.Instance.SetLoginType("Facebook");
            PrefsMgr.Instance.SetLoginID(SystemMgr.Instance._FacebookID);

            Close();
        }
    }

    void Facebook_AuthBack(IResult _Result)
    {
        WaitingMgr.Instance.Close();

        if (FB.IsLoggedIn)
        {
            PrefsMgr.Instance.SetLoginType("Facebook");
            PrefsMgr.Instance.SetLoginID(AccessToken.CurrentAccessToken.UserId);

            Close();
        }
    }

    public void Click_Guest()
    {
        ResourceMgr.Instance.PlaySound("FX", 0);

        PrefsMgr.Instance.SetLoginType("UUID");
        PrefsMgr.Instance.SetLoginID(SystemInfo.deviceUniqueIdentifier);

        Close();
    }
}
