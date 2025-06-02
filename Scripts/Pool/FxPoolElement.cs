using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FxPoolElement : MonoBehaviour
{
    public IObjectPool<FxPoolElement> Pool;
    public bool InPool;

    protected virtual void Awake()
    {
        
    }
    protected virtual void OnDisable()
    {

    }
    public void ReturnToPool()
    {
        if (gameObject.activeSelf)
        {
            Pool.Release(this);
        }
    }
    public virtual void OnRelease()
    {

    }
}
