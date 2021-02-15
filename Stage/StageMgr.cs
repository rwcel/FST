using UnityEngine;

public class StageMgr : InGameMgr
{
    private static readonly int _Max_Enemy_Type = 5;

    protected override void SetEnemy() {
        UnitMgr unitMgr = UnitMgr.Instance;
        CSVMgr csvMgr = CSVMgr.Instance;

        int episodeIdx = csvMgr.GetEpZoneIDX(
                // 3x3 존에서 나오는게 다름 : 랜덤처리
                0, PlayerMgr.Instance.GetEpisodeInTable(), Random.Range(0, 9)
                );

        // 총 5종류의 적들을 생성한다.
        int[] enemyIdx = new int[_Max_Enemy_Type];
        int[] enemyCount = new int[_Max_Enemy_Type];
        for (int i = 0; i < _Max_Enemy_Type; i++) {
            enemyIdx[i]     = csvMgr.GetEpZoneCharacterIDX(episodeIdx, i);          // 다음 존에 리젠될 적의 번호
            enemyCount[i]   = csvMgr.GetEpZoneCharacterCount(episodeIdx, i);        // 다음 존에 리젠될 적의 수
        }

        // 아무리 많이 입력해도 _EnemyMgr.Count보다 많이 못 만든다
        for (int i = 0; i < _Max_Enemy_Type; i++) {
            for (int j = 0, maxEnemyCount = unitMgr.GetEnemyMgrCnt(); j < enemyCount[i] && j < maxEnemyCount; ++j) {
                unitMgr.GetEnemyMgr(j).SetEnemy(enemyIdx[i]);
            }
        }

        base.SetEnemy();
    }

    public override void NextStage() {
        if (m_Result == BattleResult.Success) {
            SystemMgr.Instance.Tracker_AccumulateStage();

            PlayerMgr.Instance.AddStage(1);

            PlayerMgr.Instance.AutoCollectFairy();
        }

        TransitionUIMgr.Instance.TransitionBox(SystemMgr.Instance.MoveInGame, Vector2.right, 2f, true);
    }
}
