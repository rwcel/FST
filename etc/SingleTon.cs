using UnityEngine;
using System.Collections;

public class SingleTon<T> : MonoBehaviour {

	private static T _Instance;

    public static T Instance
    {
        get
        {
            return _Instance;
        }
    }

    protected virtual void Awake()
    {
        _Instance = this.GetComponent<T>();
    }
}
