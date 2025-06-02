using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUtils : MonoBehaviour
{
    public static Sprite GetSpriteItemByData(Dictionary<string, object> dicItem)
    {
        string itemKey = dicItem.GetString("item");
        return SpriteAtlasHelper.GetSpriteByName(MainGameController.Instance.MainSO.Sprites, $"item_{itemKey}");
    }
}
