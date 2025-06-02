using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    [SerializeField]
    protected bool isDontDestroyOnLoad = true;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //not instance => find in hierarchy
                instance = FindObjectOfType<T>();
#if UNITY_EDITOR
                if(instance == null) 
                {
                    GameObject go = new GameObject();
                    //go.name = 
                    instance = go.AddComponent<T>();
                    go.name = instance.GetType().Name;
                }
#endif
            }
            return instance;
        }
    }
    public void EditorInit()
    { 
    }
    protected virtual void Awake()
    {
        if (isDontDestroyOnLoad == true)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void DestroyInstance()
    {
        instance = null;
    }
}
