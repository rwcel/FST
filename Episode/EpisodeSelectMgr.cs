using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EpisodeType
{
    Story, HScene
}

/// <summary>
/// 외부에서 선택한 값이 들어가는 장소
/// </summary>
public class EpisodeSelectMgr : SingleTon<EpisodeSelectMgr>
{
    public EpisodeType      _EpisodeType { get; private set; }
    public EpisodeData[]    _EpisodeDatas { get; private set; }
    public HSceneData       _HSceneData { get; private set; }
    public int              _HeroNum { get; private set; }          // #리스트넘버 0,1,2,3,... (영웅번호가 아님)

    public void SetEpisodeStory(int heroNum) {
        _HeroNum = heroNum %= 2;                                    // # 임시로 2개만 생성
        _EpisodeType = EpisodeType.Story;
        _EpisodeDatas = ResourceMgr.Instance.GetEpisodeDatas("Hero", heroNum);
    }

    public int GetHSceneCount() {
        Object[] obj = Resources.LoadAll("Episode/Hero" + _HeroNum + "/HScene");
        return obj.Length;
    }

    public void SetHScene(int num) {
        _EpisodeType = EpisodeType.HScene;
        _HSceneData = ResourceMgr.Instance.GetHSceneData("Hero", _HeroNum, "HScene", num);
    }

}
