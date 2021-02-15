using UnityEngine;
using System.Collections;

public class StageHeroMgr : HeroMgr
{
    public override void SetHero(int _SlotNum) {
        base.SetHero(_SlotNum);

        if(m_HeroBtn != null) {
            m_HeroBtn._LevelUpAction += (() => _BattleCharacter.UpdateStat());
        }

    }   
    
    #region Dead

    protected override void Dead()
    {
        base.Dead();

        BattleMgr.Instance.CheckCheerupVoice();            
    }

    #endregion Dead
}
