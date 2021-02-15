using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배경 돌려쓰기
/// </summary>
public class BattleBackgroundMgr : SingleTon<BattleBackgroundMgr>
{
    [SerializeField] BattleComposition[]    m_BattleCompositions;
    
    public BattleComposition GetEpisodeBattleComposition() {
        return m_BattleCompositions[PlayerMgr.Instance.GetEpisodeFull() % m_BattleCompositions.Length];
    }

}
