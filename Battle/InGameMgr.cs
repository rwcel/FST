using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMgr : BattleMgr
{
    protected override void Start() {
        base.Start();

        InfoEnd();
    }

    void SetNewBeeIcon() {
        m_OnNewBeeIcon = false;
        // 뉴비 설정
        for (int i = 0, length = UnitMgr.Instance.GetHeroMgrCnt(); i < length; i++) {
            if (UnitMgr.Instance.GetHeroMgr(i)._CanStartStat == true) {
                m_OnNewBeeIcon = true;
                break;
            }
        }
        // # StageUIMgr.Instance.SetNewbeeIcon(_OnNewBeeIcon);
    }

    void LoadEnd() {
        SystemMgr.Instance.ShowNotice();

        if (PrefsMgr.Instance.GetInfo() == 0 && PlayerMgr.Instance.GetLv() <= 0) {
            InfoMgr.Instance.Show();
        }
        else {
            InfoEnd();
        }
    }

    public void InfoEnd() {
        CheckPhase();
    }

    #region Phase

    /// <summary>
    /// 페이즈 체크
    /// </summary>
    public override void CheckPhase() {
        base.CheckPhase();

        // 각 페이즈별 행동값 테이블에서 가져오기
        PhaseStep phaseStep = CSVMgr.Instance.GetStageStep(PlayerMgr.Instance.GetStageInTable(), m_CurPhase);

        // 보스 한번 잡으면 끝내기
        if (m_IsClear) {
            phaseStep = PhaseStep.End;
        }

        if (phaseStep == PhaseStep.None) {
            CheckPhase();
        }
        else if (phaseStep == PhaseStep.Battle) {
            SetEnemy();
        }
        else if (phaseStep == PhaseStep.Move) {
            // **이동 어떻게 처리할것인지 확인 필요
            //MoveNext();
            CheckPhase();
        }
        else if (phaseStep == PhaseStep.End) {
            m_Result = BattleResult.Success; // 적 전멸
            NextStage();
        }
    }

    #endregion Phase

    protected virtual void SetEnemy() {
        UnitMgr unitMgr = UnitMgr.Instance;
        CSVMgr csvMgr = CSVMgr.Instance;
        EnemyMgr enmeyMgr;

        int maxEnemyCount = unitMgr.GetEnemyMgrCnt();

        // 이동 컨트롤
        BattleCamera.Instance.SetEnemy();

        // 체력 세팅
        for (int i = 0, length = unitMgr.GetEnemyListCnt(); i < length; i++) {
            enmeyMgr = unitMgr.GetEnemyMgr(i);
            enmeyMgr.InitHP();
            enmeyMgr.GetComponent<BattleCharacter>().SetData(i);
        }

        // Debug.Log(unitMgr.GetEnemyListCnt() + "," + maxEnemyCount);

        // 나머지 끄기
        for (int i = unitMgr.GetEnemyListCnt(), length = maxEnemyCount; i < length; i++) {
            unitMgr.GetEnemyMgr(i).SetEnemy(-1);
        }

    }


    public virtual void NextStage() { }

    /// <summary>
    /// stageLevel - up or set
    /// </summary>
    /// <param name="up"></param>
    /// <param name="stageNum"></param>
    public override void CustomNextStage(int stageNum, bool up) {
        //유저 트래킹
        SystemMgr.Instance.Tracker_AccumulateStage();

        if (up) {
            PlayerMgr.Instance.AddStage(stageNum);
        }
        else {
            PlayerMgr.Instance.SetStage(stageNum);
        }

        PlayerMgr.Instance.AutoCollectFairy();

        TransitionUIMgr.Instance.FadeOut(MoveStageScene, 0.3f);
    }

    public void MoveStageScene() {
        if (PlayerMgr.Instance.GetStageInEpisode() == 9) {
            SystemMgr.Instance.MoveScene("Boss");
        }
        else {
            SystemMgr.Instance.MoveScene("Stage");  // 스테이지 재로딩
        }
    }
}
