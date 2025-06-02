using UnityEngine;

[CreateAssetMenu(fileName = "Main", menuName = "ScriptableObjects/MainSO", order = 1)]
public class MainSO : ScriptableObject
{
    public Sprite[] Sprites;

    public GameObject PrefabBubbleAlertNoAds;
    public Setting PrefabSetting;
    public TMPro.TextMeshPro PrefabTextRemainInSnake;
    public GameObject PrefabBorder;


    public static MainSO Instance
    {
        get { return MainGameController.Instance.MainSO; }
    }
}
