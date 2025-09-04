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

}
