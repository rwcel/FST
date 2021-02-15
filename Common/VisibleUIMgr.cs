using UnityEngine;

/// <summary> NormalPopup 등 대체용도 </summary>
public class VisibleUIMgr<T> : SingleTon<T>
{
    public System.Action _OnClose;

    protected GameObject    m_Visible;

    protected virtual void Start() {
        m_Visible = transform.GetChild(0).gameObject;

        SetData();
        Close();
    }

    /// <summary> 한번만 호출하는 값 </summary>
    protected virtual void SetData() { }

    public virtual void Show() {
        m_Visible.SetActive(true);
        SetScreen();
    }

    public virtual void SetScreen() { }

    public virtual void Close() {
        if (_OnClose != null) {
            _OnClose();
            _OnClose = null;
        }
        m_Visible.SetActive(false);
    }

    public void Click_Close() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        Close();
    }
}
