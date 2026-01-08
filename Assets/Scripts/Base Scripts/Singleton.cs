using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
            }
            return instance;
        }
    }

    static T instance;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)(MonoBehaviour)this;
        }
        Debug.Log(typeof(T).Name + " Singleton Awake");
    }

}
