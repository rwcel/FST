using UnityEngine;
using System.Collections;

using System;

public class OneButtonMgr : NormalPopup<OneButtonMgr>
{
    #region type def

    private enum Labels
    {
        Title,
        Info,
        Button,
    }

    #endregion

    public UILabel[] _Label;

    public UILabel _LabelInfo2;

    public UISprite _ItemSprite;
    public UISprite _ArtifactSprite;
    public UITexture _HeroTexture;

    public GameObject _Effect;
    public ParticleSystem _Particle;

    public GameObject _ItemBG;

    private Action _OnFinished;
    private int _Timer;

    public void Show(Action _OnFinished, int _Button, int _Title, int _Info)
    {
        base.Open();

        this._OnFinished = _OnFinished;

        _Label[(int)Labels.Title].text = CSVMgr.Instance.GetLanguage(_Title);
        _Label[(int)Labels.Info].text = CSVMgr.Instance.GetLanguage(_Info);
        _Label[(int)Labels.Button].text = CSVMgr.Instance.GetLanguage(_Button);

        _LabelInfo2.text = "";

        _ItemSprite.spriteName = "Empty";
        _ArtifactSprite.spriteName = "Empty";
        _HeroTexture.mainTexture = null;

        _ItemBG.SetActive(false);
        _Effect.SetActive(false);
    }

    public void Show(Action _OnFinished, int _Button, string _Info)
    {
        base.Open();

        string[] lines = _Info.Split('\n');
        //if(lines.Length == 1) {
        //    _ItemSprite.height = 300;
        //}
        //else {
        //    _ItemSprite.height = 410;
        //}

        this._OnFinished = _OnFinished;

        _Label[(int)Labels.Title].text = CSVMgr.Instance.GetLanguage(772);
        _Label[(int)Labels.Info].text = _Info;
        _Label[(int)Labels.Button].text = CSVMgr.Instance.GetLanguage(_Button);

        _LabelInfo2.text = "";

        _ItemSprite.spriteName = "Empty";
        _ArtifactSprite.spriteName = "Empty";
        _HeroTexture.mainTexture = null;

        _ItemBG.SetActive(false);
        _Effect.SetActive(false);
    }

    public void Show(Action _OnFinished, int _Button, int _Title, int _Info, int _ItemType, int _ItemIDX, double _ItemValue, int _Timer = 0)
    {
        base.Open();

        this._OnFinished = _OnFinished;

        _Label[(int)Labels.Title].text = CSVMgr.Instance.GetLanguage(_Title);
        _Label[(int)Labels.Info].text = "\n\n\n\n\n" + CSVMgr.Instance.GetLanguage(_Info);
        _Label[(int)Labels.Button].text = CSVMgr.Instance.GetLanguage(_Button);

        switch (_ItemType)
        {
            case 0:
                if (_ItemIDX == 2)
                {
                    _LabelInfo2.text = SystemMgr.Instance.GetDoubleString(_ItemValue);
                }
                else
                {
                    _LabelInfo2.text = string.Format("{0:0}", _ItemValue);
                }

                _ItemSprite.spriteName = "Item" + _ItemIDX;
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
            case 1:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "Empty";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = ResourceMgr.Instance.GetTexture("face", _ItemIDX);
                break;
            case 2:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "Empty";
                _ArtifactSprite.spriteName = "Artifact" + _ItemIDX;
                _HeroTexture.mainTexture = null;
                break;
            case 3:
                _LabelInfo2.text = SystemMgr.Instance.GetDateTime(double.Parse(_ItemValue.ToString()), 2);

                _ItemSprite.spriteName = string.Format("Speed{0}", _ItemIDX);
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
            case 4:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "TreasureChest_01";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
            case 5:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "Icon_EXP";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;

            case 7:
                _LabelInfo2.text = SystemMgr.Instance.GetDateTime(double.Parse(_ItemValue.ToString()), 2);

                _ItemSprite.spriteName = "Icon_auto_skill";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;

            case -1:
                _LabelInfo2.text = "";

                _ItemSprite.spriteName = "Empty";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
        }

        if (_Timer > 0)
        {
            this._Timer = _Timer;

            StopCoroutine("CloseTimer");
            StartCoroutine("CloseTimer");
        }

        _ItemBG.SetActive(true);
        _Effect.SetActive(true);
        _Particle.Play(true);
    }

    /// <summary>
    /// **enum처리
    /// </summary>
    /// <param name="_OnFinished">버튼 클릭시 실행할 함수</param>
    /// <param name="_Button">버튼 텍스트</param>
    /// <param name="_Title">제목 텍스트</param>
    /// <param name="_Info">들어갈내용</param>
    /// <param name="_ItemType">아이템 종류</param>
    /// <param name="_ItemIDX">아이템번호</param>
    /// <param name="_ItemValue">아이템 개수</param>
    /// <param name="_Timer">시간</param>
    public void Show(Action _OnFinished, int _Button, int _Title, string _Info, int _ItemType, int _ItemIDX, double _ItemValue, int _Timer = 0)
    {
        base.Open();

        this._OnFinished = _OnFinished;

        _Label[(int)Labels.Info].text = "\n\n\n\n\n" + _Info;
        _Label[(int)Labels.Button].text = CSVMgr.Instance.GetLanguage(_Button);
        _Label[(int)Labels.Title].text = CSVMgr.Instance.GetLanguage(_Title);

        switch (_ItemType)
        {
            case 0:
                if (_ItemIDX == 2)
                {
                    _LabelInfo2.text = SystemMgr.Instance.GetDoubleString(_ItemValue);
                }
                else
                {
                    _LabelInfo2.text = _ItemValue.ToString();
                }

                _ItemSprite.spriteName = "Item" + _ItemIDX;
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
            case 1:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "Empty";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = ResourceMgr.Instance.GetTexture("face", _ItemIDX);
                break;
            case 2:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "Empty";
                _ArtifactSprite.spriteName = "Artifact" + _ItemIDX;
                _HeroTexture.mainTexture = null;
                break;
            case 3:
                _LabelInfo2.text = SystemMgr.Instance.GetDateTime(double.Parse(_ItemValue.ToString()), 2);

                _ItemSprite.spriteName = string.Format("Speed{0}", _ItemIDX);
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
            case 4:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "TreasureChest_01";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
            case 5:
                _LabelInfo2.text = _ItemValue.ToString();

                _ItemSprite.spriteName = "Icon_EXP";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;

            case 7:
                _LabelInfo2.text = SystemMgr.Instance.GetDateTime(double.Parse(_ItemValue.ToString()), 2);

                _ItemSprite.spriteName = "Icon_auto_skill";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;

            case -1:
                _LabelInfo2.text = "";

                _ItemSprite.spriteName = "Empty";
                _ArtifactSprite.spriteName = "Empty";
                _HeroTexture.mainTexture = null;
                break;
        }

        if (_Timer > 0)
        {
            this._Timer = _Timer;

            StopCoroutine("CloseTimer");
            StartCoroutine("CloseTimer");
        }

        _ItemBG.SetActive(true);
        _Effect.SetActive(true);
        _Particle.Play(true);
    }

    public void Click_Close()
    {
        ResourceMgr.Instance.PlaySound("FX", 0);

        base.Close();

        if (_OnFinished != null)
        {
            _OnFinished();
        }
        
        StopCoroutine("CloseTimer");
    }

    IEnumerator CloseTimer()
    {
        _Label[(int)Labels.Info].text = string.Format(CSVMgr.Instance.GetLanguage(1735), _Timer);

        yield return new WaitForSecondsRealtime(1f);

        _Timer--;

        if (_Timer > 0)
        {
            StartCoroutine("CloseTimer");
        }
        else
        {
            Click_Close();
        }
    }
}
