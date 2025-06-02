using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolBasic : MonoBehaviour
{
    [SerializeField] private FxPoolElement prefabItem = null;
    IObjectPool<FxPoolElement> m_Pool;
    public bool collectionChecks = true;
    protected int maxPoolSize = 30;
    protected int defaultCapacity = 20;
    public IObjectPool<FxPoolElement> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                m_Pool = new ObjectPool<FxPoolElement>(CreateItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, defaultCapacity, maxPoolSize);
            }
            return m_Pool;
        }
    }
    public void SetCapacity(int cap, int max)
    {
        defaultCapacity = cap;
        maxPoolSize = max;
    }
    public FxPoolElement CreateItem()
    {
        FxPoolElement item = Instantiate(prefabItem, transform);
        item.Pool = Pool;
        return item;
    }
    void OnReturnedToPool(FxPoolElement system)
    {
        system.InPool = true;
        system.OnRelease();
        system.gameObject.SetActive(false);
        system.transform.SetParent(transform);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(FxPoolElement system)
    {
        system.InPool = false;
        system.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(FxPoolElement system)
    {
        Destroy(system.gameObject);
    }
}
