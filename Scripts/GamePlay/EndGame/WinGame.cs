using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class WinGame : MonoBehaviour
{
    [SerializeField] Sprite[] sprites = null;
    [SerializeField] Button btnNext = null;
    [SerializeField] Button btnAds = null;
    [SerializeField] Image imageGold = null;
    [SerializeField] Image imageLevelCompleteBG = null;
    [SerializeField] Transform fxGold = null;
    [SerializeField] Text txtMaxLevel = null;
    [SerializeField] CanvasGroup cvgButtons = null;
    [Header("New Feature")]
    [SerializeField] Transform transformNewFeature = null;
    [SerializeField] Image imageBlank = null;
    [SerializeField] Image imageFull = null;
    [SerializeField] Image imageProgress = null;
    [SerializeField] Text txtProgress = null;
    [SerializeField] Transform effectFull = null;
    public System.Action OnNext;
    public System.Action OnAdsNext;
    [SerializeField] Transform[] transformsFirework = null;
    private int goldEarn = 20;
    bool allowAction;
    private void Awake()
    {
        btnNext.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(onNext));
        btnAds.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(onAdsNext));
        btnAds.gameObject.SetActive(false);
    }
    //private void OnEnable()
    //{
        
    //}
    void showFireworks()
    {
        foreach (var item in transformsFirework)
        {
            item.gameObject.SetActive(false);
        }
        
        float timeAnimChar = 0.4f;
        this.Wait(timeAnimChar, () => {
            //transformsFirework[0].gameObject.SetActive(true);
            
            this.Wait(0, () =>
            {
                
                transformsFirework[3].gameObject.SetActive(true);
                this.Wait(0.2f, () =>
                {
                    transformsFirework[2].gameObject.SetActive(true);
                    this.Wait(0.2f, () =>
                    {
                        transformsFirework[1].gameObject.SetActive(true);
                        this.Wait(1, showFireworks, true);
                    }, true);
                }, true);
            }, true);
        }, true);
        
    }
    private void onNext()
    {
        if (!allowAction) return;
        allowAction = false;
        imageGold.gameObject.SetActive(false);
        GoldEffectHelper.StartEffGold(MainGameController.Instance.UserInfoUI.UserGold,
                    goldEarn,
                    imageGold.transform.position,
                    MainGameController.Instance.UserInfoUI.transform, () => {
                        MainGameController.Instance.UserInfoUI.HideAll();
                        OnNext?.Invoke();
                    });
        
    }
    private void onAdsNext()
    {
        if (!allowAction) return;
        allowAction = false;
        UserInfo.AddGold(goldEarn);// add them lan nua
        imageGold.gameObject.SetActive(false);
        GoldEffectHelper.StartEffGold(MainGameController.Instance.UserInfoUI.UserGold,
                       goldEarn * 2,
                       imageGold.transform.position,
                       MainGameController.Instance.UserInfoUI.transform, () => {
                           MainGameController.Instance.UserInfoUI.HideAll();
                           OnAdsNext?.Invoke();
                       });
    }
    public void ShowEndGame(int level)
    {
        imageLevelCompleteBG.SetAlpha(0);
        SoundController.Instance.PlaySoundEffectOneShot("Finish_New_Decord_Item");
        allowAction = true;
        cvgButtons.alpha = 0;
        imageGold.transform.localScale = Vector3.zero;
        imageGold.gameObject.SetActive(true);
        fxGold.gameObject.SetActive(false);
        UserInfo.AddGold(goldEarn);
        showFireworks();
        cvgButtons.DOFade(1, 0.3f).SetDelay(2);
        imageGold.transform.DOScale(1, 0.3f).SetDelay(1).OnComplete(()=> {
            fxGold.gameObject.SetActive(true);
        });
        imageLevelCompleteBG.DOFade(1, 0.3f).SetDelay(0.5f);
        txtMaxLevel.gameObject.SetActive(level >= GameStatic.MAX_LEVEL);
        transformNewFeature.gameObject.SetActive(false);

        int nextLevel = level + 1;
        var data = getNextFeature(nextLevel);
        int levelNextFeature = data.Item2;
        if (levelNextFeature > 0)
        {
            effectFull.gameObject.SetActive(false);
            this.Wait(1f, () =>
            {
                transformNewFeature.DOScale(1.1f, 0.1f).OnComplete(() => {
                    transformNewFeature.DOScale(1, 0.1f);
                });
                float firstFill = (float)(nextLevel - 1 - data.Item1) / (data.Item2 - data.Item1);
                int firstPercent = Mathf.FloorToInt(firstFill * 100);
                txtProgress.text = $"{firstPercent}%";
                imageFull.fillAmount = firstFill;
                imageProgress.fillAmount = firstFill;
                transformNewFeature.gameObject.SetActive(true);
                imageBlank.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, $"blank_{(data.Item3 + 1).ToString("0000")}");
                imageFull.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, $"full_{(data.Item3 + 1).ToString("0000")}");

                //var nextLevel = getNextFeature(level + 1);
                var nextFill = (float)(nextLevel - data.Item1) / (data.Item2 - data.Item1);
                imageFull.DOFillAmount(nextFill, 0.3f).SetDelay(0.3f);
                imageProgress.DOFillAmount(nextFill, 0.3f).SetDelay(0.3f);
                int nextPercent = Mathf.FloorToInt(nextFill * 100);
                DOTween.To(_x =>
                {
                    txtProgress.text = $"{Mathf.FloorToInt(_x)}%";
                }, firstPercent, nextPercent, 0.3f).SetDelay(0.3f).OnComplete(() => {
                    if (nextPercent >= 100)
                    {
                        float defaultScale = imageBlank.transform.localScale.x;
                        imageBlank.transform.DOScale(defaultScale * 1.2f, 0.1f).OnComplete(() =>
                        {
                            imageBlank.transform.DOScale(defaultScale, 0.2f).OnComplete(() =>
                            {
                                SoundController.Instance.PlaySoundEffectOneShot("NewFeature_1");
                                effectFull.gameObject.SetActive(true);
                            });
                        });
                    }
                });
            });
            
        }
        else
        {
            transformNewFeature.gameObject.SetActive(false);

        }
    }

    private (int, int, int) getNextFeature(int level)
    {
        int start = 0;
        int end = 0;
        int index = 0;
        try
        {
            var dic = GameStatic.ConfigLevel.GetDictionary("level_unlock").GetDictionary("new_gameplay");
            var sortedDict = dic.OrderBy(pair => pair.Value.ToInt()).ToDictionary(pair => pair.Key, pair => pair.Value);
            for (int i = 0; i < sortedDict.Values.Count; i++)
            {
                int value = sortedDict.Values.ElementAt(i).ToInt();
                if (i > 0)
                {
                    start = sortedDict.Values.ElementAt(i - 1).ToInt();
                }
                if (level <= value)
                {
                    end = value;
                    index = i;
                    break;
                }
            }
        }
        catch (System.Exception)
        {
            start = -1;
            end = -1;
            index = 0;
        }
        return (start, end, index);
    }
}
