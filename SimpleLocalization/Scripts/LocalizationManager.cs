using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;
using System.Globalization;

namespace Assets.SimpleLocalization.Scripts
{
    /// <summary>
    /// Localization manager.
    /// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// Fired when localization changed.
        /// </summary>
        public static event Action OnLocalizationChanged = () => { };

        public static Dictionary<string, Dictionary<string, string>> Dictionary = new();
        public static Dictionary<string, Dictionary<string, object>> DictionaryResult = new();
        public static readonly Dictionary<string, string> DicDefine = new Dictionary<string, string>()
        {
            {"Tiếng Việt - vi","vi"},
            {"English - En","en"},
            
            {"Indonesian - Indo","id"},
            {"Pусский - Russian","ru"},
                {"Deutsch - German","de"},
                    {"日本語 - Japanese","ja"},
                    {"繁體漢字 - Chinese (Phồn thể)","zh-tw"},
                    {"Português - Portuguese","pt"},
                    {"Français - French","fr"},
                    {"한국어 - Korean","ko"},
                        {"Español - Spanish","es"},
                            {"Italiano - Italian","it"},
                                {"Hindi - India","in"}
        };
        private static string _language = "English";

        /// <summary>
        /// Get or set language.
        /// </summary>
        public static string Language
        {
            get => _language;
            set { _language = value; OnLocalizationChanged(); }
        }

        /// <summary>
        /// Set default language.
        /// </summary>
        public static void AutoLanguage()
        {
            Language = "English";
        }

        /// <summary>
        /// Read localization spreadsheets.
        /// </summary>
        public static void Read()
        {
            if (Dictionary.Count > 0) return;
            DictionaryResult.Clear();
            var keys = new List<string>();

            foreach (var sheet in LocalizationSettings.Instance.Sheets)
            {
                var textAsset = sheet.TextAsset;
                var lines = GetLines(textAsset.text);
                var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();

                if (languages.Count != languages.Distinct().Count())
                {
                    Debug.LogError($"Duplicated languages found in `{sheet.Name}`. This sheet is not loaded.");
                    continue;
                }

                for (var i = 1; i < languages.Count; i++)
                {
                    if (!Dictionary.ContainsKey(languages[i]))
                    {
                        Dictionary.Add(languages[i], new Dictionary<string, string>());
                    }
                }

                for (var i = 1; i < lines.Count; i++)
                {
                    var columns = GetColumns(lines[i]);
                    var key = columns[0];

                    if (key == "") continue;

                    if (keys.Contains(key))
                    {
                        Debug.LogError($"Duplicated key `{key}` found in `{sheet.Name}`. This key is not loaded.");
                        continue;
                    }

                    keys.Add(key);

                    for (var j = 1; j < languages.Count; j++)
                    {
                        if (Dictionary[languages[j]].ContainsKey(key))
                        {
                            Debug.LogError($"Duplicated key `{key}` in `{sheet.Name}`.");
                        }
                        else
                        {
                            string value = columns[j];
                            value = value.Replace("\\r", "\r");
                            value = value.Replace("\\n", "\n");
                            Dictionary[languages[j]].Add(key, value);
                        }
                    }
                }
            }
            var dicEn = Dictionary["English - En"];
            foreach (var item in dicEn)
            {
                string key = item.Key;
                Dictionary<string, object> r = new Dictionary<string, object>();
                foreach (var itemDic in Dictionary)
                {
                    string langKey = itemDic.Key;
                    if(!DicDefine.ContainsKey(langKey)) continue;
                    Dictionary<string, string> tDic = itemDic.Value;
                    r.Add(DicDefine[langKey], tDic[key]);
                }
                DictionaryResult.Add(key, r);
            }
            // GUIUtility.systemCopyBuffer = DecodeEncodedNonAsciiCharacters(MiniJSON.Json.Serialize(DictionaryResult));
            Debug.Log("Fetch Data Completed");
            AutoLanguage();
        }
        public static string GetResult()
        {
            return DecodeEncodedNonAsciiCharacters(MiniJSON.Json.Serialize(DictionaryResult));
        }
        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            Debug.Log(value.Length);
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        /// <summary>
        /// Check if a key exists in localization.
        /// </summary>
        public static bool HasKey(string localizationKey)
        {
            return Dictionary.ContainsKey(Language) && Dictionary[Language].ContainsKey(localizationKey);
        }

        /// <summary>
        /// Get localized value by localization key.
        /// </summary>
        public static string Localize(string localizationKey)
        {
            if (Dictionary.Count == 0)
            {
                Read();
            }

            if (!Dictionary.ContainsKey(Language)) throw new KeyNotFoundException("Language not found: " + Language);

            var missed = !Dictionary[Language].ContainsKey(localizationKey) || Dictionary[Language][localizationKey] == "";

            if (missed)
            {
                Debug.LogWarning($"Translation not found: {localizationKey} ({Language}).");

                return Dictionary["English"].ContainsKey(localizationKey) ? Dictionary["English"][localizationKey] : localizationKey;
            }

            return Dictionary[Language][localizationKey];
        }

        /// <summary>
        /// Get localized value by localization key.
        /// </summary>
        public static string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            return string.Format(pattern, args);
        }

        public static List<string> GetLines(string text)
        {
            text = text.Replace("\r\n", "\n").Replace("\"\"", "[_quote_]");

            var matches = Regex.Matches(text, "\"[\\s\\S]+?\"");

            foreach (Match match in matches)
            {
                text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[_comma_]").Replace("\n", "[_newline_]"));
            }

            // Making uGUI line breaks to work in asian texts.
            text = text.Replace("。", "。 ").Replace("、", "、 ").Replace("：", "： ").Replace("！", "！ ").Replace("（", " （").Replace("）", "） ").Trim();

            return text.Split('\n').Where(i => i != "").ToList();
        }

        public static List<string> GetColumns(string line)
        {
            return line.Split(',').Select(j => j.Trim()).Select(j => j.Replace("[_quote_]", "\"").Replace("[_comma_]", ",").Replace("[_newline_]", "\n")).ToList();
        }
    }
}