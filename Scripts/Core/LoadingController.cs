using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LoadingController : Singleton<LoadingController>
{
    [Header("MainLoader")]
    //[SerializeField] private SpriteRenderer[] spriteRenderers = null;
    [SerializeField] private CanvasGroup canvasGroupMainLoader = null;
    [SerializeField] private MainLoader mainLoader = null;
    [SerializeField] private GameLoader gameLoader = null;
    //[SerializeField] private Transform transformWatingCanvas = null;
    //[SerializeField] private Transform transformWatingAnimation = null;
    [SerializeField] private Text txtLoading = null;
    [SerializeField] private Image imageProgress = null;
    [SerializeField] private Transform transformWormLoading = null;
    [SerializeField] private Image imageWorm = null;
    //[SerializeField] private Transform transformScrewLoadingRotate = null;
    Vector2 posLoading;
    //Vector3 rorateScrew;
    //int count = 5;

    float wLoader;
    string strFomart;
    protected override void Awake()
    {
        base.Awake();
        try
        {
            wLoader = imageProgress.transform.GetComponent<RectTransform>().sizeDelta.x;
        }
        catch (Exception)
        {
        }

    }
    public void InitText()
    {
        strFomart = LanguageHelper.GetTextByKey("loading_preloader");
        txtLoading.WrapperSetText(string.Format(strFomart, string.Empty));
        float size = GameUtils.CaculateLengthOfText(txtLoading, true);
        Vector2 pos = txtLoading.transform.localPosition;
        pos.x = -size / 2 - 10;
        txtLoading.transform.localPosition = pos;
    }
    public void ShowMainSlash()
    {
        mainLoader.transform.gameObject.SetActive(true);
    }
    Tween tween;
    bool a = false;
    public void UpdateProgress(int percent)
    {
        float _value = (float)percent / 100;
        imageProgress.fillAmount = _value;
        float x = (_value * wLoader);
        x = Math.Clamp(x, 0, 483);
        posLoading.x = x;
        transformWormLoading.localPosition = posLoading;
        if (tween == null)
        {
            a = !a;
            if (a)
            {
                tween = imageWorm.transform.DOScaleX(0.9f, 0.1f).OnComplete(() =>
                {
                    tween = null;
                });
            }
            else
            {
                tween = imageWorm.transform.DOScaleX(1, 0.1f).OnComplete(() =>
                {
                    tween = null;
                });
            }
        }
        float r = percent * 9;
        strFomart = LanguageHelper.GetTextByKey("loading_preloader");
        txtLoading.WrapperSetText(string.Format(strFomart, percent));
    }
    public void ShowCompleteFirstLoader(System.Action callback)
    {
        if (mainLoader == null)
        {
            callback?.Invoke();
            return;
        }
        mainLoader.RunClose(callback);
    }
    public void HideMainSlash(bool fade = false)
    {
        if (fade)
        {
            float time = 0.3f;
            canvasGroupMainLoader.DOFade(0, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                mainLoader.transform.gameObject.SetActive(false);
            });
        }
        else
        {
            mainLoader.transform.gameObject.SetActive(false);
        }
    }
    //public void ShowCompleteFirstLoader(System.Action callback)
    //{
    //    mainLoaderVer2.RunClose(callback);
    //}
    //private void onCompleteFirstLoader()
    //{
    //    HideMainSlash(true);
    //}

    public void ShowGameSlash()
    {
        return;
        gameLoader.gameObject.SetActive(true);
    }

    public void HideGameSlash()
    {
        return;
        System.Action callback = () => { gameLoader.gameObject.SetActive(false); };
        if (gameLoader == null)
        {
            callback?.Invoke();
            return;
        }
        gameLoader.RunClose(callback);
    }
    //public void ShowGameLoader(bool isModeLevel)
    //{
    //    bool isMaxLevel = UserInfo.Level > GameStatic.MAX_LEVEL;
    //    int level = UserInfo.Level;
    //    gameLoader.gameObject.SetActive(true);
    //    gameLoader.ShowAppear(isModeLevel, isMaxLevel, level);
    //}
    //public void GameLoaderClose(System.Action callback)
    //{
    //    if(gameLoader == null)
    //    {
    //        callback?.Invoke();
    //        return;
    //    }
    //    if (!gameLoader.gameObject.activeInHierarchy)
    //    {
    //        callback?.Invoke();
    //        return;
    //    }
    //    this.Wait(0.5f, ()=>{
    //        gameLoader.Disappear(callback);
    //    });
    //}
    //public void HideGameLoader()
    //{
    //    if(gameLoader == null) return; 
    //    gameLoader.gameObject.SetActive(false);
    //}
    //public void ShowWaiting(bool showAnimation = false)
    //{
    //    Debug.LogError("ShowWaiting");
    //    transformWatingCanvas?.gameObject.SetActive(true);
    //    transformWatingAnimation?.gameObject.SetActive(false);
    //    if(showAnimation) transformWatingAnimation?.gameObject.SetActive(true);
    //}

    public void HideWaiting()
    {
        Debug.LogError("HideWaiting");
        //transformWatingCanvas?.gameObject.SetActive(false);
        //transformWatingAnimation?.gameObject.SetActive(false);
    }
}
