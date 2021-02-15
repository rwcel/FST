using UnityEngine;

public class PartyUIMgr : VisibleUIMgr<PartyUIMgr>
{
    [SerializeField] GameObject     m_Party;
    [SerializeField] GameObject     m_Book;
    [SerializeField] GameObject     m_Relation;

    [Header("OtherScripts")]
    [SerializeField] SlideTab       m_SlideTab;


    public override void Show() {
        base.Show();

        m_SlideTab.SetButton(0, "Party", ShowParty, true);                     // CSVMgr.Instance.GetLanguage(529)
        m_SlideTab.SetButton(1, "Book", ShowBook,false,true);                  // CSVMgr.Instance.GetLanguage(1749)
        m_SlideTab.SetButton(2, "Relation", ShowRelation, false, true);        // CSVMgr.Instance.GetLanguage(1950)
    }

    private void ShowParty() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        m_Party.SetActive(true);
        m_Book.SetActive(false);
        m_Relation.SetActive(false);
    }
    private void ShowBook() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();

        //m_Party.SetActive(false);
        //m_Book.SetActive(true);
        //m_Relation.SetActive(false);
    }
    private void ShowRelation() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        NoneTouchMsg.Instance.ShowDeveloping();

        //m_Party.SetActive(false);
        //m_Book.SetActive(false);
        //m_Relation.SetActive(true);
    }
}
