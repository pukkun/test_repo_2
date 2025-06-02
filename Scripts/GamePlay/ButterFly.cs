using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static GameDataSO;

public class ButterFly : MonoBehaviour
{
    [SerializeField] Renderer _renderer = null;
    [SerializeField] Animator animator = null;
    [SerializeField] Material[] materials = null;
    private void Awake()
    {
        materials = _renderer.sharedMaterials;
    }
    public void InitColor(GameDataSO gameDataSO, BlockColor blockColor)
    {
        DataMaterial dataMaterial = gameDataSO.GetDataMaterial(blockColor);
        Material[] materials = _renderer.sharedMaterials;

        materials[0] = dataMaterial.WingsMat;
        materials[2] = dataMaterial.Material;
        _renderer.materials = materials;
        
    }
    public void Fly()
    {
        //return;
        animator.enabled = true;
        //return;
        this.Wait(5, () => { 
            gameObject.SetActive(false);
            Destroy(gameObject);
        });
    }
}
