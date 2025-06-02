using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] BuyBooster buyBooster = null;
    [SerializeField] BoostItem[] boostItems = null;
    [SerializeField] Transform transformHandGuideBooster = null;

    [SerializeField] Transform transformContainerBooster = null;
    [SerializeField] CanvasGroup cvgReady = null;
    [SerializeField] Image imgReadyMain = null;
    [SerializeField] Button btnCloseReady = null;
    [SerializeField] Text txtReadyGuide = null;
    [SerializeField] Image imageIconBoosterReady = null;

    public System.Action<string> UseBoost = null;
    public System.Action CancelBoost = null;

    string currentGuideBooster;

    private void Awake()
    {
        btnCloseReady.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(() => {
            HideReady();
            CancelBoost?.Invoke();
        }));
    }
    public void InitBoost()
    {
        foreach (var item in boostItems)
        {
            item.OnClick = onUseItem;
            item.OnBuyItem = onBuyItem;
            int quan = UserInfo.GetItemByKey(item.Key);
            item.InitData(quan);
        }
    }
    private void onUseItem(BoostItem boostItem)
    {
        Debug.Log("onUseItem " + boostItem.Key);
        if (!currentGuideBooster.IsNullOrEmpty() && boostItem.Key != currentGuideBooster) return;
        if (!currentGuideBooster.IsNullOrEmpty())
        {
            
        }
        UseBoost?.Invoke(boostItem.Key);
    }
    private void onBuyItem(BoostItem boostItem)
    {
        buyBooster.gameObject.SetActive(true);
        buyBooster.Apppear(GameManager.Instance.CurrentLevel, boostItem, ()=> {
            int add = 1;
            UserInfo.AddItem(boostItem.Key, add, "unlock_boost", "unlock_boost");
            InitBoost();
            onUseItem(boostItem);
            //int value = UserInfo.GetItemByKey(boostItem.Key);
            //boostItem.InitData(value);
            Debug.Log("OnBuyComplete " + boostItem.Key); 
            buyBooster.gameObject.SetActive(false);
        });
        
    }
    public void ShowHandGuideBooster(string keyBooster)
    {
        currentGuideBooster = null;
        transformHandGuideBooster.gameObject.SetActive(false);
        if (keyBooster.IsNullOrEmpty()) return;
        currentGuideBooster = keyBooster;
        BoostItem booster = GetBoostItemByKey(keyBooster);
        if (booster != null)
        {
            transformHandGuideBooster.gameObject.SetActive(true);
            transformHandGuideBooster.transform.position = booster.transform.position;
        }
    }
    public BoostItem GetBoostItemByKey(string key)
    {
        foreach (var item in boostItems)
        {
            if (item.Key == key) return item;
        }
        return null;
    }
    public bool IsHandGuiding()
    {
        return transformHandGuideBooster.gameObject.activeInHierarchy;
    }
    public void OnUseItemComplete(string key)
    {
        Debug.Log("onUseItemComplete " + key);
        UserInfo.UseItem(key);
        BoostItem boostItem = GetBoostItemByKey(key);
        boostItem.InitData(boostItem.Quantity - 1);

        TrackingHelper.PowerUpUsed(GameManager.Instance.CurrentLevel.ToString(),
            TrackingConstant.MType_MAIN,
            key,
            TrackingConstant.MName_MAIN_NAME + GameManager.Instance.CurrentLevel,
            GameUtils.GetMissionDataTracking(GameManager.Instance.CurrentLevel));
    }
    public void ShowReady(string key, bool isGuiding)
    {
        transformContainerBooster.gameObject.SetActive(false);
        imgReadyMain.transform.SetWidth(200);
        cvgReady.gameObject.SetActive(true);
        cvgReady.alpha = 1;
        txtReadyGuide.transform.localScale = new Vector3(0, 1, 1);
        txtReadyGuide.text = LanguageHelper.GetTextByKey($"boost_{key}_guide");
        btnCloseReady.gameObject.SetActive(false);
        txtReadyGuide.transform.DOScaleX(1, 0.2f);
        DOTween.To(_x => {
            imgReadyMain.transform.SetWidth(_x);
        }, 200, 635, 0.2f).OnComplete(() => {
            btnCloseReady.gameObject.SetActive(true);
            if(isGuiding) btnCloseReady.gameObject.SetActive(false);
        });
    }
    public void HideReady()
    {
        transformContainerBooster.gameObject.SetActive(true);
        cvgReady.gameObject.SetActive(false);
    }
}
