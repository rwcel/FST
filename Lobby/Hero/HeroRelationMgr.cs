using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class HeroRelationMgr : ScrollPopup<HeroRelationMgr>
{
    // public UISprite[] _Tab;

    private PartyEffectType _SelectType = PartyEffectType.None;

    private List<int> _PE_IDX = new List<int>();

    /// <summary>
    /// 적용중인거 기본으로 보여주기
    /// </summary>
    /// <param name="effectType"></param>
    public void Show(PartyEffectType effectType = PartyEffectType.None)
    {
        base.Open();

        ResourceMgr.Instance.PlaySound("FX", 0);

        if(effectType == PartyEffectType.None && _SelectType == PartyEffectType.None) {
            _SelectType = PartyEffectType.Apply;
        }
        else if(effectType != PartyEffectType.None) {
            _SelectType = effectType;
        }

        _PE_IDX.Clear();
        _PE_IDX = _SelectType != PartyEffectType.Apply ? CSVMgr.Instance.GetPartyEffect((int)_SelectType) :
                                                        PlayerMgr.Instance.GetTeamColor();      // 팀컬러 가져오기  **처음에 안됨 왜?

        _DynamicScroll[0].SetCnt(_PE_IDX.Count);
        _DynamicScroll[0].SetList();
        _DynamicScroll[0].PositionReset();
    }

    public int GetTeamColorIDX(int _Num)
    {
        return _PE_IDX[_Num];
    }

    public void Close() {
        _SelectType = PartyEffectType.None;

        base.Close();
    }
}
