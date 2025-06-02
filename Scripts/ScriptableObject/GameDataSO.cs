using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameDataSO", fileName = "GameDataSO", order = 100)]
public class GameDataSO : ScriptableObject
{
    public DevMode PrefabDevMode;
    public TextAsset TextLanguage;
    public DataMaterial[] DataMaterials;
    public DataBorder[] DataBorders;
    public Material WhiteMaterial;
    public Material AlphaMaterial;

    [Header("Prefab Fx")]
    public GameObject PrefabFxAppearButterFly = null;
    public GameObject PrefabFxPairDestroy = null;
    public GameObject PrefabFxPodBreak = null;
    public GameObject PrefabFxXichBreak = null;
    public GameObject PrefabFxBombRelease = null;
    public GameObject PrefabFxBombExplosion = null;


    [Header("Prefab Game")]
    public ButterFly PrefabButterFly = null;
    public GameObject PrefabTrailGhost;
    public GameObject PrefabHeadGhosting;
    public GameObject PrefabClickStartGhosting;


    public string ConfigLevel;



    [System.Serializable]
    public class DataMaterial
    {
        public Enums.BlockColor BlockColor;
        public Material Material;
        public Material Material_Outline;
        public Material WingsMat;
        public Material TargetMat;
        public Sprite SpritePortal_1;
        public Sprite SpriteBlank;
        public Sprite SpriteCubeTarget;
        public Sprite SpriteCircleTarget;
        public Color MainColor;
        public Material TargetShootMat;
    }
    [System.Serializable]
    public class DataBorder
    {
        public Enums.BorderType BorderType;
        public GameObject PrefabBorder;
    }
    public Material GetMaterial(Enums.BlockColor blockColor)
    {
        foreach (var item in DataMaterials)
        {
            if (item.BlockColor == blockColor)
            {
                return item.Material;
            }
        }
        return null;
    }
    public Material GetTargetMaterial(Enums.BlockColor blockColor)
    {
        foreach (var item in DataMaterials)
        {
            if (item.BlockColor == blockColor)
            {
                return item.TargetMat;
            }
        }
        return null;
    }
    public DataMaterial GetDataMaterial(Enums.BlockColor blockColor)
    {
        foreach (var item in DataMaterials)
        {
            if (item.BlockColor == blockColor)
            {
                return item;
            }
        }
        return null;
    }
    public GameObject GetPrefabBorder(Enums.BorderType borderType)
    {
        foreach (var item in DataBorders)
        {
            if (item.BorderType == borderType)
            {
                return item.PrefabBorder;
            }
        }
        return null;
    }
    public static GameDataSO Instance
    {
        get { return GameEnviroment.Instance.GameDataSO; }
    }
}
