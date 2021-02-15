using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hero -> EnemyBattleTarget 이용해야함
/// </summary>
public class BattleTarget : MonoBehaviour
{
    private List<BattleCharacter>   m_TargetCharacterList;
    private BattleCharacter[]       m_TargetCharacters;

    private void Start() {
        m_TargetCharacterList = new List<BattleCharacter>();
        m_TargetCharacters = GetComponentsInChildren<BattleCharacter>();

        BattleMgr.Instance._OnBattleStart += () => CheckTargetList();
        BattleMgr.Instance._OnCharacterDead += () => CheckTargetList();
    }

    /// <summary>
    /// 거리에 따라 가장 가까운 유닛 공격
    /// </summary>
    public BattleCharacter GetTargetByDist(BattleCharacter attacker) {
        Debug.Log(attacker.transform.localPosition+","+ attacker.transform.position);

        int targetNum = 0;
        float minDist = 9999f;
        float resultDist = minDist;
        for (int i = 0, length = m_TargetCharacterList.Count; i < length; i++) {
            resultDist = Vector2.SqrMagnitude(m_TargetCharacterList[i].transform.position - attacker.transform.position);
            if(resultDist < minDist) {
                targetNum = i;
            }
        }

        return m_TargetCharacterList[targetNum];
    }

    /// <summary>
    /// 랜덤 공격
    /// </summary>
    /// <returns></returns>
    public BattleCharacter GetTargetByRandom() {
        if(m_TargetCharacterList.Count == 0) {
            return null;
        }

        return m_TargetCharacterList[Random.Range(0, m_TargetCharacterList.Count)];
    }

    /// <summary>
    /// 타겟 리스트 갱신 : 죽었을 때
    /// </summary>
    public void CheckTargetList() {
        m_TargetCharacterList.Clear();

        for (int i = 0, length = m_TargetCharacters.Length; i < length; i++) {
            if (m_TargetCharacters[i]._IsLive) {
                m_TargetCharacterList.Add(m_TargetCharacters[i]);
            }
        }

        if(m_TargetCharacterList.Count == 0) {
            // Enemy면 승리, Hero면 패배
            bool result = gameObject.tag == "Enemy" ? true : false; 
            BattleMgr.Instance.BattleEnd(result);
        }

        //foreach (var item in m_TargetCharacters) {
        //    if (item.gameObject.activeSelf) {
        //        m_TargetCharacterList.Add(item);
        //    }
        //}
    }

}
