using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameExtension
{
    public static void SetHeight(this Transform transform, float h)
    {
        Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, h);
    }
    public static void SetWidth(this Transform transform, float w)
    {
        Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(w, sizeDelta.y);
    }
    public static void SetSize(this Transform transform, Vector3 size)
    {
        Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y);
    }
    public static string ToJson(this Dictionary<string, object> dictionary)
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, object>();
        }
        return MiniJSON.Json.Serialize(dictionary);
    }
    public static string ToJson(this object dictionary)
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, object>();
        }
        return MiniJSON.Json.Serialize(dictionary);
    }
    public static string ToJson<K, V>(this Dictionary<K, V> dictionary)
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<K, V>();
        }
        return MiniJSON.Json.Serialize(dictionary);
    }
    public static string ToJson<K>(this List<K> list)
    {
        if (list == null)
        {
            list = new List<K>();
        }
        return MiniJSON.Json.Serialize(list);
    }
    public static string GetString(this Dictionary<string, object> keyValuePairs, string key)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            if (keyValuePairs[key] != null)
            {
                return keyValuePairs[key].ToString();
            }
            return null;
        }
        return null;
    }
    public static V Get<K, V>(this Dictionary<K, V> keyValuePairs, K key)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            return keyValuePairs[key];
        }
        return default(V);
    }
    public static void Put<K, V>(this Dictionary<K, V> source, K key, V value)
    {
        if (source.ContainsKey(key))
        {
            source.Remove(key);
        }
        source.Add(key, value);
    }
    public static void Wait(this MonoBehaviour mono, float delay, System.Action action, bool stopIfInactive = false)
    {
        if (stopIfInactive)
        {
            if (mono == null) return;
            if (!mono.gameObject.activeInHierarchy) return;
        }
        mono.StartCoroutine(excute(delay, action));
    }
    private static IEnumerator excute(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
    public static string LogColor(string sColor, string log)
    {
        return "<color=#" + sColor + ">" + log + "</color>";
    }
    public static string ToColor(this string log, Color color)
    {
        string s = ColorUtility.ToHtmlStringRGBA(color);
        return "<color=#" + s + ">" + log + "</color>";
    }
    public static bool IsNullOrEmpty(this string _string)
    {
        return _string == null || _string == string.Empty;
    }
    public static bool IsNullOrEmpty<T>(this T[] array)
    {
        return array == null || array.Length == 0;
    }
    public static bool IsNullOrEmpty(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }
    public static void SetAlpha(this UnityEngine.UI.Button button, float alpha)
    {
        button.image.SetAlpha(alpha);
    }
    public static void SetAlpha(this UnityEngine.UI.MaskableGraphic maskableGraphic, float alpha)
    {
        var color = maskableGraphic.color;
        color.a = alpha;
        maskableGraphic.color = color;
    }

    public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
    public static int GetInt(this Dictionary<string, object> source, string key)
    {
        int result = 0;
        try
        {
            if (source.ContainsKey(key))
            {
                int.TryParse(source[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {

        }

        return result;
    }
    public static int GetInt(this object source, string key)
    {
        int result = 0;

        try
        {
            Dictionary<string, object> data = source as Dictionary<string, object>;
            if (data.ContainsKey(key))
            {
                int.TryParse(data[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {

        }

        return result;
    }
    public static long GetLong(this Dictionary<string, object> source, string key)
    {
        long result = 0;
        try
        {
            if (source.ContainsKey(key))
            {
                long.TryParse(source[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {

        }

        return result;
    }

    public static int GetInt<K, V>(this Dictionary<K, V> source, K key)
    {
        int result = 0;
        try
        {
            if (source.ContainsKey(key))
            {
                int.TryParse(source[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {

        }

        return result;
    }

    public static float GetFloat<K, V>(this Dictionary<K, V> source, K key)
    {
        float result = 0;
        try
        {
            if (source.ContainsKey(key))
            {
                float.TryParse(source[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {

        }
        return result;
    }
    public static float ToFloat(this object obj)
    {
        if (obj == null) return 0;
        return obj.ToString().toFloat();
    }
    public static int ToInt(this object obj)
    {
        if (obj == null) return 0;
        return obj.ToString().toInt();
    }
    private static int toInt(this string str)
    {
        int result = 0;
        int.TryParse(str, out result);
        return result;
    }
    private static float toFloat(this string str)
    {
        float result = 0f;
        float.TryParse(str, out result);
        return result;
    }
    public static void WrapperSetText(this Text text, string value)
    {
        text.text = value;
        text.supportRichText = true;
    }
    public static Dictionary<string, object> ToDictionary(this string json)
    {
        return MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
    }
    public static List<object> ToList(this string json)
    {
        return MiniJSON.Json.Deserialize(json) as List<object>;
    }
    public static string UpperCaseEachWord(this string str, string culture = "en")
    {
        if (string.IsNullOrEmpty(str))
            return str;
        if (culture == "in") culture = "hi"; // ngon ngu an do la hindi
        System.Globalization.TextInfo textInfo;
        try
        {
            textInfo = new System.Globalization.CultureInfo(culture, false).TextInfo;
        }
        catch (System.Globalization.CultureNotFoundException)
        {
            Debug.LogError($"Culture '{culture}' is not valid. Using default 'en-US'.");
            textInfo = new System.Globalization.CultureInfo("en-US").TextInfo;
        }
        return textInfo.ToTitleCase(str.ToLower());
    }
    public static string FirstCharToUpper(this string input)
    {
        if (System.String.IsNullOrEmpty(input))
            throw new System.ArgumentException(" string IsNullOrEmpty");
        return input[0].ToString().ToUpper() + input.Substring(1);
    }
    public static Dictionary<string, object> GetDictionary(this Dictionary<string, object> keyValuePairs, string key)
    {
        if (keyValuePairs != null && keyValuePairs.ContainsKey(key))
        {
            return keyValuePairs[key] as Dictionary<string, object>;
        }
        return null;
    }
    public static List<object> GetList(this Dictionary<string, object> source, string key)
    {
        if (source.ContainsKey(key)) return source[key] as List<object>;
        return null;
    }
    public static void SquishItem(this Transform item, float strength = 1, float speed = 1.0f, TweenCallback tweenCb = null)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(item.DOScale(1 + (0.65f - 1) * strength, 0.01f * speed));
        sequence.Append(item.DOScale(1 + (1.35f - 1) * strength, 0.25f * speed).SetEase(Ease.OutCubic));
        sequence.Append(item.DOScale(1 + (0.8f - 1) * strength, 0.15f * speed).SetEase(Ease.OutCubic));
        sequence.Append(item.DOScale(1, 0.15f * speed).SetEase(Ease.OutCubic));
        sequence.AppendCallback(tweenCb);
        sequence.Play();
    }
    public static void ForEach<T>(this T[] source, System.Action<T> action)
    {
        foreach (T element in source)
            action(element);
    }
}
