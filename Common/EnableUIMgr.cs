using UnityEngine;

/// <summary>
/// VisibleUIMgr에서 Enable시키는 Object
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnableUIMgr<T> : SingleTon<T>
{
    protected bool m_IsOpened = false;

    protected virtual void OnEnable() {
        if(!m_IsOpened) {
            m_IsOpened = true;
            SetData();
        }

        Show();
    }

    protected virtual void SetData() { }

    protected virtual void Show() { }
}
