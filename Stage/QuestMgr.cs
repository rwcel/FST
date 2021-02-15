using UnityEngine;
using System.Collections;
using System;

public class QuestMgr : SingleTon<QuestMgr>
{
    #region type def

    [Serializable]
    public struct QuestReward {
        public UISprite rootBG;
        public UISprite item;
        public UISprite artifact;
        public UITexture faceTexture;
        public UILabel amount;

        public void SetItem(int itemType, int rewardIdx, int itemAmount = 0) {
            if (0 > itemType) {
                rootBG.gameObject.SetActive(false);
                return;
            }

            rootBG.gameObject.SetActive(true);

            if (0 == itemType) {
                item.spriteName = string.Format("Item{0}", rewardIdx);
                artifact.spriteName = "Empty";
                faceTexture.mainTexture = null;
            }
            else if (1 == itemType) {
                item.spriteName = "Empty";
                artifact.spriteName = "Empty";
                faceTexture.mainTexture = ResourceMgr.Instance.GetTexture("face", rewardIdx);
            }
            else if (2 == itemType) {
                item.spriteName = "Empty";
                artifact.spriteName = string.Format("Artifact{0}", rewardIdx);
                faceTexture.mainTexture = null;
            }
            else if (3 == itemType) {
                item.spriteName = string.Format("Speed{0}", rewardIdx);
                artifact.spriteName = "Empty";
                faceTexture.mainTexture = null;
            }
            else if (4 == itemType) {
                item.spriteName = "TreasureChest_01";
                artifact.spriteName = "Empty";
                faceTexture.mainTexture = null;
            }
            else if (5 == itemType) {
                item.spriteName = "Icon_EXP";
                artifact.spriteName = "Empty";
                faceTexture.mainTexture = null;
            }

            amount.text = itemAmount.ToString();
        }
    }

    enum Label
    {
        Title,
        Desc,
        Count,
        Receive,
        Info,
    }

    #endregion // type def



    #region serialize field

    [SerializeField] private GameObject visible = null;
    [SerializeField] private UILabel[] labels = null;
    [SerializeField] private QuestReward[] rewards = null;
    [SerializeField] private UIButton buttonReceive = null;

    #endregion // serialize field


    #region private variables

    private int initTimeQuest = 0;
    private double achieveValue;
    private int targetAchieveValue;

    #endregion // private variables



    #region Mono functions

    private void Awake() {
        base.Awake();

        if (null != SystemMgr.Instance && SystemMgr.Instance.ShowingQuestMgr) {
            Show();
        }
    }

    #endregion // Mono functions



    #region public functions

    public void Show() {
        if (visible.activeSelf) {
            Close();
            return;
        }

        SetScreen(true);
    }

    public void Close() {
        SetScreen(false);
    }

    #region interactions

    public void Click_Receive() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        OneButtonMgr.Instance.Show(null, 2, 772, 1088);
        PlayerMgr.Instance.SetQuest(1, true);

        for (int i = 0; i < 3; i++) {
            int itemType = CSVMgr.Instance.GetQuestRewardType(initTimeQuest, i);

            if (itemType >= 0) {
                int rewardType = CSVMgr.Instance.GetQuestRewardIDX(initTimeQuest, i);
                int itemAmount = CSVMgr.Instance.GetQuestRewardCount(initTimeQuest, i);
                NetworkMgr.Instance.InsertMail(2133, 2134, itemType, rewardType, itemAmount);
            }
        }

        SetScreen(true);
    }

    public void Click_Item_0() {
        RewardPropertyClicked(0);
    }

    public void Click_Item_1() {
        RewardPropertyClicked(1);
    }

    public void Click_Item_2() {
        RewardPropertyClicked(2);
    }

    #endregion // interactions

    #endregion // public functions



    #region private functions

    private void SetScreen(bool show) {
        SystemMgr.Instance.ShowingQuestMgr = show;

        if(SystemMgr.Instance.GetSceneName() != "Stage") 
            return;
        else if (!show) {
            visible.SetActive(false);
            return;
        }

        initTimeQuest = PlayerMgr.Instance.GetQuest();

        //if (PlayerMgr.Instance.GetResurrection() > 0 && initTimeQuest < CSVMgr.Instance.GetQuestCnt()) {
            visible.SetActive(true);

            labels[(int)Label.Title].text = CSVMgr.Instance.GetQuestName(initTimeQuest);
            labels[(int)Label.Desc].text = string.Format(CSVMgr.Instance.GetQuestDescription(initTimeQuest),
                CSVMgr.Instance.GetQuestAchieveValue(initTimeQuest));
            labels[(int)Label.Receive].text = CSVMgr.Instance.GetLanguage(226);
            labels[(int)Label.Info].text = CSVMgr.Instance.GetLanguage(2223);

            for (int i = 0, count = rewards.Length; i < count; i++) {
                rewards[i].SetItem(CSVMgr.Instance.GetQuestRewardType(initTimeQuest, i),
                    CSVMgr.Instance.GetQuestRewardIDX(initTimeQuest, i),
                    CSVMgr.Instance.GetQuestRewardCount(initTimeQuest, i));
            }
        //}
        //else {
        //    visible.SetActive(false);
        //    return;
        //}

        StartCoroutine(UpdateContents());
    }

    private void RewardPropertyClicked(int num) {
        ResourceMgr.Instance.PlaySound("FX", 0);

        int rewardType = CSVMgr.Instance.GetQuestRewardType(initTimeQuest, num);

        if (0 <= rewardType) {
            int itemIdx = CSVMgr.Instance.GetQuestRewardIDX(initTimeQuest, num);
            ToolTipMgr.Instance.Show(rewards[num].rootBG.transform, rewardType, itemIdx);
        }
    }

    private void SetReceiveButton(bool enable) {
        if (enable) {
            buttonReceive.normalSprite = "Btn_nor";
            buttonReceive.isEnabled = true;
        }
        else {
            buttonReceive.normalSprite = "Btn_onClick";
            buttonReceive.isEnabled = false;
        }
    }

    private IEnumerator UpdateContents() {
        var waitTerm = new WaitForSeconds(0.25f);

        while (true) {
            if (initTimeQuest < CSVMgr.Instance.GetQuestCnt()) {
                int achieveType = CSVMgr.Instance.GetQuestAchieveType(initTimeQuest);
                achieveValue = PlayerMgr.Instance.GetAchievementValue(2, achieveType);

                if (7 == achieveType || 11 == achieveType || 14 == achieveType) {
                    achieveValue++;
                }

                targetAchieveValue = CSVMgr.Instance.GetQuestAchieveValue(initTimeQuest);

                labels[(int)Label.Count].text = string.Format("{0}/{1}", achieveValue, targetAchieveValue);

                if (achieveValue >= targetAchieveValue) {
                    SetReceiveButton(true);
                }
                else {
                    SetReceiveButton(false);
                }
            }
            else {
                labels[(int)Label.Count].text = string.Empty;
                SetReceiveButton(false);
            }

            yield return waitTerm;
        }
    }

    #endregion // private functions



    #region old codes

    //public void Click_OnOff() {
    //    if (!StageUIMgr.Instance.HideExpandMenus()) {
    //        return;
    //    }

    //    ResourceMgr.Instance.PlaySound("FX", 0);

    //    switch (PrefsMgr.Instance.GetQuest())
    //    {
    //        case 0:
    //            PrefsMgr.Instance.SetQuest(1);
    //            break;
    //        case 1:
    //            PrefsMgr.Instance.SetQuest(0);
    //            break;
    //    }

    //    SetScreen();
    //}

    #endregion // old codes
}
