using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public static class GameUtils
{
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSplashScreen()
    {
        //return;
        Debug.Log("Before SplashScreen is shown and before the first scene is loaded.");
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.LogError("sceneName " + sceneName);
        if (sceneName == SceneConstant.SCENE_INIT) return;
        if(Camera.main != null) Camera.main.gameObject.SetActive(false);
        if (GameObject.FindAnyObjectByType<MainGameController>() == null)
        {
            MainGameController prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<MainGameController>("Assets/Prefabs/Core/MainGameController.prefab");
            GameObject.Instantiate(prefab);
        }
        if (GameObject.FindAnyObjectByType<GameEnviroment>() == null)
        {
            GameEnviroment prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameEnviroment>("Assets/Prefabs/Core/GameEnviroment.prefab");
            GameObject.Instantiate(prefab);
        }
        if (GameObject.FindAnyObjectByType<PoolManager>() == null)
        {
            PoolManager prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<PoolManager>("Assets/Prefabs/Core/PoolManager.prefab");
            GameObject.Instantiate(prefab);
        }
        foreach (var item in GameObject.FindObjectsOfType<CanvasCameraHelper>())
        {
            item.gameObject.SetActive(false);
            item.gameObject.SetActive(true);
        }
    }
#endif
    public static string ShortCutNumber(int value)
    {
        return value.ToString();
    }
    public static UnityEngine.Events.UnityAction DelegatActionWithNormalSound(System.Action action)
    {
        return delegate
        {
            SoundController.Instance.PlaySoundEffectOneShot("sndButtonClick");
            HapticHelper.DeviceVibrate();
            action?.Invoke();
        };
    }
    
    public static string ConvertSecondToMSS(int seconds)
    {
        int minute = Mathf.FloorToInt(seconds / 60);
        int s = seconds % 60;
        return minute.ToString("0") + ":" + s.ToString("00");
    }
    public static TimerController CreateTimer(Transform parent)
    {
        GameObject go = new GameObject();
        go.name = "Timer";
        go.transform.SetParent(parent);
        go.transform.localScale = Vector3.one;
        return go.AddComponent<TimerController>();
    }
    public static Color GetColorFromOption(BlockColor option)
    {
        switch (option)
        {
            case BlockColor.Blue: return new Color(0.137f, 0.419f, 0.996f);
            case BlockColor.Red: return new Color(0.996f, 0.149f, 0.224f);
            case BlockColor.Green: return new Color(0.243f, 0.890f, 0.313f);
            case BlockColor.Yellow: return new Color(0.996f, 0.890f, 0.169f);
            case BlockColor.Orange: return new Color(1f, 0.658f, 0f);
            case BlockColor.Purple: return new Color(0.816f, 0.235f, 0.996f);
            case BlockColor.Pink: return new Color(0.988f, 0.404f, 0.874f);
            case BlockColor.DarkGreen: return new Color(0.118f, 0.400f, 0.141f);
            case BlockColor.Cyan: return new Color(0.176f, 0.761f, 0.929f);
            case BlockColor.Brown_Removed: return new Color(0.513f, 0.333f, 0.231f);
            case BlockColor.DarkRed: return new Color(0.251f, 0.004f, 0.078f);
            default: return Color.white;
        }
    }
    public static float round(float number)
    {
        float result = Mathf.Floor(number * 100f) / 100f; ;
        return result;
    }
    public static Enums.Direction GetDirection(Vector3 point1, Vector3 point2)
    {

        decimal x1 = (decimal)point1.x;
        decimal y1 = (decimal)point1.y;
        decimal x2 = (decimal)point2.x;
        decimal y2 = (decimal)point2.y;
        decimal dx = System.Math.Round(x2 - x1, 2);
        decimal dy = System.Math.Round(y2 - y1, 2);

        if (dx == 0 && dy == 0)
        {
            return Enums.Direction.None;
        }
        else if (dx == 0 && dy > 0)
        {
            return Enums.Direction.Up;
        }
        else if (dx == 0 && dy < 0)
        {
            return Enums.Direction.Down;
        }
        else if (dy == 0 && dx > 0)
        {
            return Enums.Direction.Right;
        }
        else if (dy == 0 && dx < 0)
        {
            return Enums.Direction.Left;
        }
        else
        {
            return Enums.Direction.None;
        }
    }
    public static void ClearTransformImmediate(Transform transform)
    {
        if (transform == null) return;
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            GameObject go = transform.GetChild(i).gameObject;
            Object.DestroyImmediate(go);
        }
    }
    public static void ClearTransform(Transform transform)
    {
        if (transform == null) return;
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            GameObject go = transform.GetChild(i).gameObject;
            Object.Destroy(go);
        }
    }
    public static int RandomRange(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
    public static float RandomRange(float min, float max)
    {
        return Random.Range(min, max + 1);
    }
    public static void ForceSavePref()
    {
        if (GameStatic.CachePref.Count > 0)
        {
            PlayerPrefs.Save();
            GameStatic.CachePref.Clear();
        }
    }

    public static void DeleteDataPref(string strPref)
    {
        Debug.Log("DeleteDataPref " + strPref + "   ");
        PlayerPrefs.DeleteKey(getPlayerPref(strPref));
    }
    public static bool HaveDataPref(string strPref)
    {
        return PlayerPrefs.HasKey(getPlayerPref(strPref));
    }
    private static string getPlayerPref(string strPref, bool checkUser = true)
    {
        return "AntX_game_snake_0_" + strPref;
    }
    public static void SaveDataPref(string strPref, string value, bool checkUser = true, bool forceSave = true)
    {
        // Debug.Log("SaveDataPref " + strPref +"   " + forceSave);
        PlayerPrefs.SetString(getPlayerPref(strPref, checkUser), value);
        GameStatic.CachePref.Put(strPref, value);
        if (forceSave)
        {
            ForceSavePref();
        }
    }
    public static void SaveDataPref(string strPref, int value, bool checkUser = true, bool forceSave = true)
    {
        // Debug.Log("SaveDataPref " + strPref +"   " + forceSave);
        PlayerPrefs.SetInt(getPlayerPref(strPref, checkUser), value);
        GameStatic.CachePref.Put(strPref, value.ToString());
        if (forceSave)
        {
            ForceSavePref();
        }
    }
    public static string GetStringPref(string strPref, string _default, bool checkUser = true)
    {
        return PlayerPrefs.GetString(getPlayerPref(strPref, checkUser), _default);
    }
    public static int GetIntPref(string strPref, int _default, bool checkUser = true)
    {
        int result = PlayerPrefs.GetInt(getPlayerPref(strPref, checkUser), _default);
        return result;
    }
    public static string Get2LetterISOCodeFromSystemLanguage()
    {
        SystemLanguage lang = Application.systemLanguage;
        string res = "EN";
        switch (lang)
        {
            case SystemLanguage.Afrikaans: res = "AF"; break;
            case SystemLanguage.Arabic: res = "AR"; break;
            case SystemLanguage.Basque: res = "EU"; break;
            case SystemLanguage.Belarusian: res = "BY"; break;
            case SystemLanguage.Bulgarian: res = "BG"; break;
            case SystemLanguage.Catalan: res = "CA"; break;
            case SystemLanguage.Chinese: res = "ZH_CN"; break;
            case SystemLanguage.ChineseSimplified: res = "ZH_CN"; break;
            case SystemLanguage.ChineseTraditional: res = "ZH_TW"; break;
            case SystemLanguage.Czech: res = "CS"; break;
            case SystemLanguage.Danish: res = "DA"; break;
            case SystemLanguage.Dutch: res = "NL"; break;
            case SystemLanguage.English: res = "EN"; break;
            case SystemLanguage.Estonian: res = "ET"; break;
            case SystemLanguage.Faroese: res = "FO"; break;
            case SystemLanguage.Finnish: res = "FI"; break;
            case SystemLanguage.French: res = "FR"; break;
            case SystemLanguage.German: res = "DE"; break;
            case SystemLanguage.Greek: res = "EL"; break;
            case SystemLanguage.Hebrew: res = "IW"; break;
            case SystemLanguage.Hungarian: res = "HU"; break;
            case SystemLanguage.Icelandic: res = "IS"; break;
            case SystemLanguage.Indonesian: res = "ID"; break;
            case SystemLanguage.Italian: res = "IT"; break;
            case SystemLanguage.Japanese: res = "JA"; break;
            case SystemLanguage.Korean: res = "KO"; break;
            case SystemLanguage.Latvian: res = "LV"; break;
            case SystemLanguage.Lithuanian: res = "LT"; break;
            case SystemLanguage.Norwegian: res = "NO"; break;
            case SystemLanguage.Polish: res = "PL"; break;
            case SystemLanguage.Portuguese: res = "PT"; break;
            case SystemLanguage.Romanian: res = "RO"; break;
            case SystemLanguage.Russian: res = "RU"; break;
            case SystemLanguage.SerboCroatian: res = "SH"; break;
            case SystemLanguage.Slovak: res = "SK"; break;
            case SystemLanguage.Slovenian: res = "SL"; break;
            case SystemLanguage.Spanish: res = "ES"; break;
            case SystemLanguage.Swedish: res = "SV"; break;
            case SystemLanguage.Thai: res = "TH"; break;
            case SystemLanguage.Turkish: res = "TR"; break;
            case SystemLanguage.Ukrainian: res = "UK"; break;
            case SystemLanguage.Unknown: res = "EN"; break;
            case SystemLanguage.Vietnamese: res = "VI"; break;
        }
#if UNITY_EDITOR
        // return "vi";
#endif
        return "en";
        return res.ToLower();
    }
    public static int GetRequireLevelBooster(string booster)
    {
        int result = 0;
        try
        {
            result = GameStatic.ConfigLevel.GetDictionary("level_unlock").GetDictionary("booster").GetInt(booster);
        }
        catch (System.Exception)
        {
        }
        return result;
    }
    public static int GetRequireLevelNewGamePlay(string key)
    {
        int result = 0;
        try
        {
            result = GameStatic.ConfigLevel.GetDictionary("level_unlock").GetDictionary("new_gameplay").GetInt(key);
        }
        catch (System.Exception)
        {
        }
        return result;
    }
    public static void SetLayerRecursively(GameObject go, string name)
    {
        int layer = LayerMask.NameToLayer(name);
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layer;
        }
    }
    public static void SetLayerRecursively(GameObject go, int layer)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layer;
        }
    }
    public static bool IsPointerOverGameObject()
    {
        UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
        {
            Debug.Log(" IsPointerOver " + results.Count + " " + results[0].gameObject.name);
        }
        return results.Count > 0;
    }

    public static void InitOffLineData()
    {
        GameDataSO gameData = GameEnviroment.Instance?.GameDataSO;
        if (gameData == null) return;
        GameStatic.ConfigLevel = gameData.ConfigLevel.ToDictionary();
    }
    public static bool IsProduction()
    {
        Enviroment enviroment = GameEnviroment.Instance.Enviroment;
        if (enviroment == null)
        {
            enviroment = Resources.Load<Enviroment>("Enviroment");
        }
        return enviroment.BuildGame == BuildGame.Production;
    }
    public static float CaculateLengthOfText(Text text, bool inorgeRichText = false)
    {
        float totalLength = 0;
        Font myFont = text.font;
        string message = text.text;
        CharacterInfo characterInfo = new CharacterInfo();
        string nonRichTextString = message;
        if (inorgeRichText) nonRichTextString = System.Text.RegularExpressions.Regex.Replace(message, "<.*?>", string.Empty);
        char[] arr = nonRichTextString.ToCharArray();
        foreach (char c in arr)
        {
            myFont.RequestCharactersInTexture(c.ToString(), text.fontSize, text.fontStyle);
            myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);
            totalLength += characterInfo.advance;
        }
        return totalLength;
    }
    public static bool IsLevelHard(int level)
    {
        try
        {
            if (GameStatic.LIST_LEVEL_HARD.IsNullOrEmpty())
            {
                GameStatic.LIST_LEVEL_HARD = GameStatic.ConfigLevel.GetList("level_hard").Select(s => s.ToInt()).ToList();
            }
            return GameStatic.LIST_LEVEL_HARD.Contains(level);
        }
        catch (System.Exception)
        {
        }
        return false;
    }
    public static Dictionary<string, object> GetMissionDataTracking(int level)
    {
        Dictionary<string, object> missionData = new Dictionary<string, object>();
        missionData.Add("mission_type", TrackingConstant.MType_MAIN);
        missionData.Add("mission_name", TrackingConstant.MName_MAIN_NAME + level);
        missionData.Add("mission_id", level);
        missionData.Add("mission_attempt", GetIntPref($"count_play_{level}", 0));

        Dictionary<string, object> additionalData = new Dictionary<string, object>();
        additionalData.Add("mission_data", missionData);
        return additionalData;

    }
}
