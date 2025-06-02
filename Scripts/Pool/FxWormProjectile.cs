using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FxWormProjectile : FxPoolElement
{

    [SerializeField] Transform projectile = null;
    [SerializeField] Transform explosion = null;
    [SerializeField] ParticleSystemRenderer[] psChangeMat = null;
    [SerializeField] ParticleSystem[] psChangeColor = null;

    public void SetColor(Enums.BlockColor blockColor)
    {
        GameDataSO.DataMaterial dataMaterial = GameManager.Instance.GameDataSO.GetDataMaterial(blockColor);
        foreach (var item in psChangeMat)
        {
            item.material = dataMaterial.TargetShootMat;
        }
        foreach (var item in psChangeColor)
        {
            var mainSpark = item.main;
            mainSpark.startColor = dataMaterial.MainColor;
        }
    }

    public void ShowProjectile()
    {
        projectile.gameObject.SetActive(true);
        explosion.gameObject.SetActive(false);
    }
    public void ShowExplosion()
    {
        projectile.gameObject.SetActive(false);
        explosion.gameObject.SetActive(true);
        DOVirtual.DelayedCall(1, ReturnToPool);
    }
    protected override void OnDisable()
    {
        projectile.gameObject.SetActive(false);
        explosion.gameObject.SetActive(false);
    }
}
