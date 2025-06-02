using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo
{
    public static System.Action OnChangeGold;
    public static bool Inited = false;
    public static int Gold = 0;
    public static int Level = 1;
    public static List<object> ListItem;
    public static void InitResource()
    {
        Level = GameUtils.GetIntPref("level", 1);
        Gold = GameUtils.GetIntPref("gold", 0);
#if UNITY_EDITOR
        Level = GameEnviroment.Instance.LocalEditorData.Level;
#endif
        string json = GameUtils.GetStringPref(PrefConstant.USER_RESOURCE, "{}");
        Dictionary<string, object> dataUser = null;
        try
        {
            dataUser = json.ToDictionary();
        }
        catch (System.Exception)
        {
            dataUser = new Dictionary<string, object>();
        }
        if (dataUser.Count == 0)//init default
        {
            
        }
        else
        {
            ListItem = dataUser.GetList("boost_item");
        }
        Inited = true;
    }
    public static void IncreaseLevel()
    {
        Level++;
        //if (Level > GameStatic.MAX_LEVEL) Level = GameStatic.MAX_LEVEL;
        GameUtils.SaveDataPref("level", Level);
    }
    public static int GetItemByKey(string itemKey)
    {
        if (ListItem == null) return 0;
        foreach (var item in ListItem)
        {
            Dictionary<string, object> itemData = item as Dictionary<string, object>;
            if (itemData.GetString("item") == itemKey)
            {
                return itemData.GetInt("q");
            }
        }
        return 0;
    }
    public static void SaveData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("boost_item", ListItem);
        string json = data.ToJson();
        GameUtils.SaveDataPref(PrefConstant.USER_RESOURCE, json);
    }
    public static void AddItem(string itemKey, int amount, string source, string module)
    {
        if (ListItem == null) ListItem = new List<object>();
        bool haveUpdate = false;
        foreach (var item in ListItem)
        {
            Dictionary<string, object> itemData = item as Dictionary<string, object>;
            if (itemData.GetString("item") == itemKey)
            {
                haveUpdate = true;
                itemData.Put("q", itemData.GetInt("q") + amount);
            }
        }
        if (!haveUpdate)
        {
            Dictionary<string, object> item = new Dictionary<string, object>();
            item.Add("item", itemKey);
            item.Add("q", amount);
            ListItem.Add(item);
        }
        SaveData();
    }
    public static void UseItem(string itemKey)
    {
        foreach (var item in ListItem)
        {
            Dictionary<string, object> itemData = item as Dictionary<string, object>;
            if (itemData.GetString("item") == itemKey)
            {
                int value = itemData.GetInt("q") - 1;
                value = Mathf.Max(value, 0);
                itemData.Put("q", value);
            }
        }
        SaveData();
    }
    public static void AddGold(int value, string reasonSpecific = "", string moduleGeneral = "")
    {
        Gold += value;
        SaveData();
        OnChangeGold?.Invoke();
    }
}
