using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedImage : MonoBehaviour, ILocalized
{
    [SerializeField] private Sprite[] sprites = null;


    Image image = null;
    void Awake()
    {
        image = GetComponent<Image>();
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
                image.sprite = item;
                image.SetNativeSize();
                break;
            }
        }
        if (!have && spriteEn != null)
        {
            image.sprite = spriteEn;
            image.SetNativeSize();
        }
    }


    
}
