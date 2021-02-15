using UnityEngine;

public class DungeonChallengeItem : ChallengeItem
{

    public override void Click_Select() {
        m_ListType.SetActive(true);
    }

    public override void Click_CloseSelect() {
        m_ListType.SetActive(false);
    }

}
