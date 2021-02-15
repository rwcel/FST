using UnityEngine;
using System.Collections;

public class StageEnemyMgr : EnemyMgr
{
    #region etc

    public override void SetStatus() {
        base.SetStatus();

        _Grade = PlayerMgr.Instance.GetStageFull() / 1000;
    }

    public override void InitHP() {
        _MaxHP = CSVMgr.Instance.GetTotalHP(_TableNum, _LV);

        base.InitHP();
    }

    #endregion etc
}
