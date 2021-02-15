using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class BossResultMgr : SingleTon<BossResultMgr>
{
    [System.Serializable]
    private struct ResultType
    {
        public GameObject       _Obj;
        public Text             _Text;
        public Button           _Button;
        public ParticleSystem   _ParticleSystem;
    }

    [System.Serializable]
    private struct RewardItem
    {
        public GameObject   _Obj;
        public Image        _Image;
        public Text         _Count;
    }

    [SerializeField] GameObject m_VisibleObj;

    [SerializeField] ResultType m_Win;
    [SerializeField] ResultType m_Lose;

    [SerializeField] RewardItem m_Item;
    [SerializeField] RewardItem m_Medal;

    private List<CSVMgr.RewardGroup> _Reward = new List<CSVMgr.RewardGroup>();
    private VisibleTimer             m_VisibleTimer;
    private Animator                 m_Animator;

    private bool _Victory;
    private int _RewardIDX;
    private int _RewardNum;
    private int _MedalValue = 1;

    private ItemType _Type;
    private int _ID;

    private const string _Anim_Win      = "Win";
    private const string _Anim_Lose     = "Lose";

    public void Show(bool victory, int rewardIDX = -1) {
        _Victory = victory;
        _RewardIDX = rewardIDX;

        Time.timeScale = 0;
        SetScreen();
    }

    public void SetScreen() {
        m_Win._Obj.SetActive(_Victory);
        m_Lose._Obj.SetActive(!_Victory);

        if (_Victory) {
            m_VisibleTimer = m_Win._Obj.GetComponent<VisibleTimer>();
            m_Win._Text.text = CSVMgr.Instance.GetLanguage(1088);
            m_Win._ParticleSystem.Play();
            m_Animator.SetTrigger(_Anim_Win);

            OpenReward();
        }
        else {
            m_VisibleTimer = m_Lose._Obj.GetComponent<VisibleTimer>();
            m_Lose._Text.text = string.Format("{0}\n\n{1}", 
                    CSVMgr.Instance.GetLanguage(921), CSVMgr.Instance.GetLanguage(922));
            m_Lose._ParticleSystem.Play();
            m_Animator.SetTrigger(_Anim_Lose);
        }

        m_VisibleTimer._HideAction += (() => {
            TransitionUIMgr.Instance.TransitionBox(SystemMgr.Instance.MoveInGame, 
                                                   Vector2.right, 2f, true);
            Time.timeScale = 1f;
        });
        m_VisibleObj.SetActive(true);
    }

    void OpenReward() {
        _Reward.Clear();
        _Reward = CSVMgr.Instance.GetRewardGroup(_RewardIDX);
        _RewardNum = Random.Range(0, _Reward.Count);
        _Type = (ItemType)_Reward[_RewardNum]._RewardType;
        _ID = _Reward[_RewardNum]._RewardID;
        double _Cnt = _Reward[_RewardNum]._RewardCount;


        if (_Type == ItemType.Goods) {
            m_Item._Count.text = (_ID == (int)PaymentType.Gold) ? SystemMgr.Instance.GetDoubleString(_Cnt) :
                                                                                   _Cnt.ToString();
            m_Item._Image.sprite = ResourceMgr.Instance.GetSprite("Item", _ID);

            PlayerMgr.Instance.AddResource(_ID, _Cnt);
        }
        else if (_Type == ItemType.HeroPiece) {
            m_Item._Count.text = _Cnt.ToString();

            Debug.Log("=========Face 이후에 채우기=======");
            //m_Item._Image.sprite = ResourceMgr.Instance.GetSprite("Face", _ID);

            PlayerMgr.Instance.AddHero(_ID, 0, true);
        }
        else if (_Type == ItemType.Artifact) {
            m_Item._Count.text = _Cnt.ToString();

            m_Item._Image.sprite = ResourceMgr.Instance.GetSprite("Artifact", _ID);

            PlayerMgr.Instance.AddArtifact(_ID, (int)_Cnt);
        }

        // **무조건 1개만 얻음
        PlayerMgr.Instance.AddResource((int)PaymentType.Medal, _MedalValue);
        m_Medal._Image.sprite = ResourceMgr.Instance.GetSprite("Item", (int)PaymentType.Medal);
        m_Medal._Count.text = _MedalValue.ToString();
    }

    public void Click_Close() {
        m_VisibleObj.SetActive(false);

        TransitionUIMgr.Instance.TransitionBox(SystemMgr.Instance.MoveInGame, Vector2.right, 2f, true);
        Time.timeScale = 1f;
        // LoadingMgr.Instance.Show();
        // FadeMgr.Instance.FaceShow(3f, MoveStageScene);
    }

    public void Click_Item() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        ToolTipMgr.Instance.Show(m_Item._Obj.transform, (int)_Type, _ID);
    }

    public void Click_Medal() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        ToolTipMgr.Instance.ShowLanguage(m_Medal._Obj.transform, 2311, 2314);
    }
}
