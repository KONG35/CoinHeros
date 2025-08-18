using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected bool isDone=true;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            
            if (isDone)
            {
                if (this.transform.parent == null)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
                else
                {
                    Transform rootParent = this.transform.root;
                    if (rootParent != this.transform)
                    {
                        DontDestroyOnLoad(rootParent.gameObject);
                    }
                    else
                    {
                        DontDestroyOnLoad(this.gameObject);
                    }
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}