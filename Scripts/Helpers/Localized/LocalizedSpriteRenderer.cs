using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedSpriteRenderer : MonoBehaviour, ILocalized
{
    [SerializeField] private Sprite[] sprites = null;


    SpriteRenderer spriteRenderer = null;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LocalizedManager.Instance?.Add(this);
        UpdateLanguage();
    }
    void OnDestroy()
    {
        LocalizedManager.Instance?.Remove(this);
    }

    public void UpdateLanguage()
    {
        Sprite spriteEn = null;
        bool have = false;
        foreach (var item in sprites)
        {
            if (item.name.IndexOf(LanguageHelper.LanguageSetting) == 0)
            {
                if (spriteEn == null && LanguageHelper.LanguageSetting == "en")
                {
                    spriteEn = item;
                }
                have = true;
                spriteRenderer.sprite = item;
                break;
            }
        }
        if (!have && spriteEn != null)
        {
            spriteRenderer.sprite = spriteEn;
        }
    }
}
