using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAtlasHelper
{
    public static Sprite GetSpriteByName(Sprite[] sprites, string nameSprite)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i]!= null && nameSprite == sprites[i].name)
            {
                return sprites[i];
            }
        }
        return null;
    }

    public static Texture GetTextureByName(Texture[] textures, string nameSprite)
    {
        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null && nameSprite == textures[i].name)
            {
                return textures[i];
            }
        }
        return null;
    }
}
