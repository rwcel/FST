using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyMgr : EnemyMgr
{
    public override void SetStatus() {
        base.SetStatus();

        _Grade = PlayerMgr.Instance.GetStageFull() / 1000;
    }

    public override void InitHP() {
        _MaxHP = CSVMgr.Instance.GetTotalHP(_TableNum, _LV);
        _MaxHP *= 1 + (CSVMgr.Instance.GetBossHpMultiply() * 0.001f);

        base.InitHP();
    }
}
