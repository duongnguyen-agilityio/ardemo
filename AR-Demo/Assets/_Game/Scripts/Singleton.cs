using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if((_instance = FindObjectOfType<T>()) == null)
				{
					var obj = new GameObject();
					_instance = obj.AddComponent<T>();
				}				
            }
            return _instance;
        }
    }

    public static bool IsActive
    {
        get
        {
            return _instance != null;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

    }
}
