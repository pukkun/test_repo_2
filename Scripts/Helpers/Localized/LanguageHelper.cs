using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LanguageHelper
{
    public static string LanguageSetting = "en";//vi, en
    private static Dictionary<string, object> dataLanguage;
    public static readonly string[] LIST_LANGUAGE = { 
        "en",
        "vi",
        "id",
        "ru",
        "de",
        "ja",
        "zh-tw",
        "pt",
        "fr",
        "ko",
        "es",
        "it",
        "in"
    }; 
    
    public static Dictionary<string, string> LANGUAGE_NAME = new Dictionary<string, string>()
    {
        {"en", "English" },
        {"fr", "Français" },
        {"ru", "Pусский" },
        {"pt", "Português" },
        {"de", "Deutsch" },
        {"it", "Italiano" },
        {"es", "Español" },
        {"ja", "日本語" },
        {"ko", "한국어" },
        {"id", "Indonesian" },
        {"zh-tw", "繁體漢字" },
        {"vi", "Tiếng Việt" },
        {"in", "Hindi" },
    };
    public static void InitLanguage()
    {
        // #if !UNITY_EDITOR
        //         // test editor truoc
        //         return;
        // #endif
    #if TOOL_EDITOR
             return;
    #endif
        LanguageSetting = GameUtils.GetStringPref("language", "");
        if(LanguageSetting == "") LanguageSetting = GameUtils.Get2LetterISOCodeFromSystemLanguage();
        if (GameEnviroment.Instance.GameDataSO)
        {
            SetData(GameEnviroment.Instance.GameDataSO.TextLanguage.text.ToDictionary());
        }
    }

    public static void SetData(Dictionary<string, object> data)
    {
        dataLanguage = data;
    }
    public static string GetTextByKey(string key , Enums.FormatLocalized formatLocalized = Enums.FormatLocalized.None)
    {
        if (dataLanguage == null) return key;

        if (dataLanguage.ContainsKey(key))
        {
            Dictionary<string, object> data = dataLanguage[key] as Dictionary<string, object>;
            string result = null;
            try
            {
                result = data[LanguageSetting].ToString();
                if (result.IsNullOrEmpty())
                {
                    result = data["en"].ToString();
                }
            }
            catch (System.Exception)
            {
                result = data["en"].ToString();
            }

            if (formatLocalized == Enums.FormatLocalized.UpperAll)
            {
                result = result.ToUpper();
            }
            else if (formatLocalized == Enums.FormatLocalized.UpperWord)
            {
                result = result.UpperCaseEachWord(LanguageSetting);
            }
            else if (formatLocalized == Enums.FormatLocalized.UpperFirst)
            {
                result = result.FirstCharToUpper();
            }
            return result;
        }
        return key;
    }

    
    

    public static void UpdateLanguage(string language)
    {
        LanguageSetting = language;
        LocalizedManager.Instance.ChangeAllText();
        GameUtils.SaveDataPref("language", language);
    }
    
}
