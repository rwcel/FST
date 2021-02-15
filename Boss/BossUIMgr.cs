using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUIMgr : SingleTon<BossUIMgr>
{
    [SerializeField] Image              m_Profile;
    [SerializeField] Text               m_BossName;


    public void SetScreen() {
        int tableNum = UnitMgr.Instance.GetEnemyList(0)._TableNum;
        m_Profile.sprite = ResourceMgr.Instance.GetSprite("EnemyFace", 0);          // $
        m_BossName.text = string.Format(CSVMgr.Instance.GetHeroName(tableNum));
    }

}
