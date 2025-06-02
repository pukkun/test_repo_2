using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : Singleton<MainGameController>
{
    public UserInfoUI UserInfoUI;
    public MainSO MainSO;
    Setting setting;
    GameObject alertNoAds;
    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR

        //if (MainSO == null) MainSO = UnityEditor.AssetDatabase.LoadAssetAtPath<MainSO>("Assets/Scripts/ScriptableObjects/MainSO.asset");
        //if (UserInfoUI == null)
        //{
        //    Destroy(gameObject);
        //    MainGameController prefab =  UnityEditor.AssetDatabase.LoadAssetAtPath<MainGameController>("Assets/Prefabs/Core/MainGameController.prefab");
        //    Instance = Instantiate(prefab);
        //}


#endif
    }
    private void Start()
    {
        UserInfoUI.HideAll();

        this.Wait(1, checkCacheFail);
    }
    void checkCacheFail()
    {
        string cacheFail = GameUtils.GetStringPref(PrefConstant.CACHE_MISSION_FAIL, string.Empty);
        if (!cacheFail.IsNullOrEmpty())
        {
            try
            {
                Dictionary<string, object> dataCache = cacheFail.ToDictionary();
                int level = dataCache.GetInt("lv");
                int score = dataCache.GetInt("score");
                Enums.TypeLoseGame typeLoseGame = (Enums.TypeLoseGame)dataCache.GetInt("type_lose");
                TrackingHelper.MissionFailed(level == 1,
                    TrackingConstant.MType_MAIN,
                    TrackingConstant.MName_MAIN_NAME + level,
                    level.ToString(),
                    score,
                        GameUtils.GetMissionDataTracking(level),
                    typeLoseGame.ToString());
                GameUtils.SaveDataPref(PrefConstant.CACHE_MISSION_FAIL, string.Empty);
            }
            catch (System.Exception)
            {
            }

        }
    }
    public void ShowSetting(bool inGamePlay = false, System.Action onClickHome = null)
    {
        if (setting == null) setting = Instantiate(MainSO.PrefabSetting, transform);
        setting.OnClickHome = onClickHome;
        setting.OnShow(inGamePlay);
    }
    public void HideSetting()
    {
        if (setting != null) setting.HideSetting();
    }
    public void ShowBubbleAlertNoAds()
    {
        if (alertNoAds == null) alertNoAds = Instantiate(MainSO.PrefabBubbleAlertNoAds, transform);
        alertNoAds.SetActive(true);
        CanvasGroup canvasGroup = alertNoAds.GetComponentInChildren<CanvasGroup>();
        alertNoAds.transform.localPosition = new Vector3(0, -4.0f);
        float scale = 1.0f;
        float ratio = (float)Screen.height / Screen.width;
        if (ratio >= 2.5f) scale = 0.8f;
        else if (ratio >= 2.22f) scale = 0.9f;
        alertNoAds.transform.localScale = Vector3.one * scale;
        var activeTweens = DOTween.TweensById(alertNoAds, false);
        if (activeTweens != null && activeTweens.Count > 0)
        {
            foreach (var tween in activeTweens)
            {
                if (tween.ElapsedPercentage(true) < 0.33f) return;
                if (tween.ElapsedPercentage(true) < 0.8f) // 0.86
                {
                    DOTween.Kill(alertNoAds, complete: false);
                    Sequence newSequence = DOTween.Sequence();
                    newSequence.SetId(alertNoAds);
                    newSequence.Append(canvasGroup.DOFade(1, 0.33f));
                    newSequence.AppendInterval(1.66f);
                    newSequence.Append(canvasGroup.DOFade(0, 0.33f));
                    newSequence.AppendCallback(() => {
                        alertNoAds.SetActive(false);
                    });
                    return;
                }
            }
            DOTween.Kill(alertNoAds, complete: false);
        }
        canvasGroup.alpha = 0;
        Sequence sequence = DOTween.Sequence();
        sequence.SetId(alertNoAds);
        sequence.Append(canvasGroup.DOFade(1, 0.33f));
        sequence.AppendInterval(1.66f);
        sequence.Append(canvasGroup.DOFade(0, 0.33f));
        sequence.AppendCallback(() => {
            alertNoAds.SetActive(false);
        });
    }
}
