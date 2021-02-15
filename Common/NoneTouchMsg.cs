using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 자동으로 없어지는 알림형 메시지
/// </summary>
public class NoneTouchMsg : VisibleUIMgr<NoneTouchMsg>
{
    [SerializeField] Text           m_Title;
    [SerializeField] Text           m_Contents;

    private Animation               m_Animation;

    private const string _Develop_Text = "We're working on a development.";

    protected override void SetData() {
        m_Animation = GetComponent<Animation>();
    }

    public void ShowDeveloping() {
        m_Contents.text = _Develop_Text;

        base.Show();
    }

    public void ShowText(string contents) {
        m_Contents.text = contents;

        base.Show();
    }

    public override void SetScreen() {
        m_Animation.Play();
    }
}
