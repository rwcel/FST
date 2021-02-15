using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeCharItem : ListItemData
{
    [SerializeField] Image          m_CharImage;
    [SerializeField] GameObject     m_DisableObject;           // 조건 미충족

    protected override void SetScreen() {
        base.SetScreen();

        m_CharImage.sprite = ResourceMgr.Instance.GetSprite("HeroSD", m_Num % 5);
        m_CharImage.SetNativeSize();

        m_DisableObject.SetActive(PlayerMgr.Instance.GetResurrection() < m_Num);
        // m_UnOwn.SetActive(PlayerMgr.Instance.GetHeroLv(num) < 0);
    }

    public override int GetKey() {
        return m_Num;
    }

    public override void Click_Item() {
        base.Click_Item();

        EpisodeSelectUIMgr.Instance.SelectHeroListItem(m_Num);
    }
}
