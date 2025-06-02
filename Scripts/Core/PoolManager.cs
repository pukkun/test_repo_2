using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    protected override void Awake()
    {
        base.Awake();
        PoolOfTargetBlock.SetCapacity(50, 100);
        //EarlyInitTileBlock();
    }
    IEnumerator Start()
    {
        yield return null;
    }
    public PoolBasic PoolOfGold;
    public PoolBasic PoolOfFxCollect;
    public PoolBasic PoolOfFxHole;
    public PoolBasic PoolOfTargetBlock;
    public PoolBasic PoolOfWormProjectile;

    public GoldItem GetGoldItem()
    {
        return PoolOfGold.Pool.Get() as GoldItem;
    }
    public FxCollectItem GetFxCollectItem()
    {
        return PoolOfFxCollect.Pool.Get() as FxCollectItem;
    }
    public FxHoleItem GetFxHoleItem()
    {
        return PoolOfFxHole.Pool.Get() as FxHoleItem;
    }
    public TargetBlock GetTargetBlockItem()
    {
        return PoolOfTargetBlock.Pool.Get() as TargetBlock;
    }
    public FxWormProjectile GetFxWormProjectile()
    {
        return PoolOfWormProjectile.Pool.Get() as FxWormProjectile;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GoldItem GoldItem = GetGoldItem();
                this.Wait(1, () =>
                {
                    GoldItem.ReturnToPool();
                });
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
            }
        }
    }
#endif
}
