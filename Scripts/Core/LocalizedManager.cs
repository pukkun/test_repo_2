using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedManager : Singleton<LocalizedManager> {

    public static System.Action OnUpdateLanguage;
    [SerializeField]
    private List<ILocalized> localizedKeys = new List<ILocalized>();

    protected override void Awake()
    {
        LanguageHelper.InitLanguage();
        base.Awake();
        Instance.EmptyInit();
    }
    public void EmptyInit()
    {
        
    }
    public void Add(ILocalized localizedKey)
    {
        localizedKeys.Add(localizedKey);
    }
    public void Remove(ILocalized localizedKey)
    {
        if (localizedKeys.Contains(localizedKey))
        {
            localizedKeys.Remove(localizedKey);
        }
    }
    public void ChangeAllText()
    {
        for (int i = 0; i < localizedKeys.Count; i++)
        {
            if (localizedKeys[i] != null)
            {
                localizedKeys[i].UpdateLanguage();
            }
        }
        OnUpdateLanguage?.Invoke();
    }
    #if UNITY_EDITOR
    [SerializeField] public string TestLanguage;
    [UnityEditor.CustomEditor(typeof(LocalizedManager))]
    public class CustomLocalizedManager:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            LocalizedManager localizedManager = target as LocalizedManager;
            if (GUILayout.Button("ApplyLanguage"))
            {
                LanguageHelper.LanguageSetting = localizedManager.TestLanguage;
                LocalizedManager.Instance.ChangeAllText();
                GameUtils.SaveDataPref("language", LanguageHelper.LanguageSetting);
            }
        }
    }
#endif
}
