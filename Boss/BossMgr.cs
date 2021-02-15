using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMgr : InGameMgr
{
    protected override void SetEnemy() {
        UnitMgr unitMgr = UnitMgr.Instance;
        CSVMgr csvMgr = CSVMgr.Instance;

        unitMgr.GetEnemyMgr(0).SetEnemy(csvMgr.GetBossIDX());
        m_IsClear = true;   // 다시 Phase진행하면 무조건 클리어

        base.SetEnemy();

        BossUIMgr.Instance.SetScreen();
    }

    public override void BattleEnd(bool result) {
        m_Result = result ? BattleResult.Success : BattleResult.Fail;
        Invoke("NextStage", 1.5f);
    }

    public override void NextStage() {
        if (m_Result == BattleResult.Success) {
            PlayerMgr.Instance.AddAchievementValue(3, 1);
            PlayerMgr.Instance.AddEventValue(3, 1);
            PlayerMgr.Instance.AddStage(1);
            PlayerMgr.Instance.AutoCollectFairy();
            SystemMgr.Instance.Tracker_AccumulateStage();

            BossResultMgr.Instance.Show(true, (int)ResultType());
        }   // 전투 성공일때 현재 위치값 저장 후 스테이지값 증가
        else {
            if (PlayerMgr.Instance.GetStageInEpisode() != 0) {
                PlayerMgr.Instance.AddStage(-1);
            }
            BossResultMgr.Instance.Show(false);
        }
    }


    TreasureType ResultType()
    {
        int _Random = Random.Range(0, 100);

        if (PlayerMgr.Instance.GetStageFull() > 8000)
        {
            return _Random >= 75 ? TreasureType.Gold : (_Random >= 40 ? TreasureType.Sliver : TreasureType.Bronze);
        }
        else if (PlayerMgr.Instance.GetStageFull() > 4000)
        {
            return _Random >= 95 ? TreasureType.Gold : (_Random >= 50 ? TreasureType.Sliver : TreasureType.Bronze);
        }
        else if (PlayerMgr.Instance.GetStageFull() > 1000)
        {
            return _Random >= 97 ? TreasureType.Gold : (_Random >= 65 ? TreasureType.Sliver : TreasureType.Bronze);
        }
        else
        {
            return _Random >= 99 ? TreasureType.Gold : (_Random >= 95 ? TreasureType.Sliver : TreasureType.Bronze);
        }
    }
}
