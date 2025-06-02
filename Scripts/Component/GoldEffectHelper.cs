using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GoldEffectHelper
{
    public static void StartEffGold(UserInfoItem userInfoItem, int quantity, Vector3 vtStart, Transform transformParent, System.Action onComplete)
    {
        int numGoldItem = quantity;
        numGoldItem = (numGoldItem > 10) ? 10 : numGoldItem;
        float delay = 0.1f;
        int numComplete = 0;

        int startGold = UserInfo.Gold - quantity;
        userInfoItem.TextQuan.text = GameUtils.ShortCutNumber(startGold);
        float valueBall = startGold;
        float step = quantity * 1.0f / numGoldItem;

        bool isSound = false;
        userInfoItem.CanvasGroup.gameObject.SetActive(true);
        bool appearTarget = false;
        if (!userInfoItem.gameObject.activeInHierarchy)
        {
            appearTarget = true;
        }
        if (appearTarget)
        {
            if (!DOTween.IsTweening(userInfoItem.CanvasGroup))
            {
                userInfoItem.CanvasGroup.alpha = 0;
            }
            else DOTween.Kill(userInfoItem.CanvasGroup);
            userInfoItem.CanvasGroup.DOFade(1, 0.3f);
        }
        float timeMoveToTarget = 0.5f;
        for (int i = 0; i < numGoldItem; i++)
        {
            int index = i;
            MainGameController.Instance.Wait(index * 0.01f, () =>
            {
                float delayPer = index * 0.02f + delay;
                GoldItem goGold = PoolManager.Instance.GetGoldItem();
                goGold.transform.localScale = Vector3.one * 0f;
                goGold.transform.position = new Vector3(vtStart.x + GameUtils.RandomRange(-1.1f, 1.1f), vtStart.y + GameUtils.RandomRange(-1.0f, 1.0f), 0);
                //goGold.transform.DOMove(, 0);
                goGold.transform.DOScale(1f, 0.3f).SetEase(Ease.Linear).OnComplete(() => {
                    //Debug.LogError("delayPerdelayPer " + delayPer);
                    goGold.transform.DOScale(0.2f, timeMoveToTarget).SetEase(Ease.InQuad).SetDelay(delayPer);
                    //goGold.transform.DOMoveX(userInfoItem.Image.transform.position.x, 0.3f).SetEase(Ease.InQuad).SetDelay(i * 0.05f + delay);
                    goGold.transform.DOMove(userInfoItem.Image.transform.position, timeMoveToTarget).SetEase(Ease.OutQuad).SetDelay(delayPer).OnComplete(() => {
                        userInfoItem.Image.transform.SquishItem();
                        goGold.ReturnToPool();
                        numComplete++;
                        if (numGoldItem == numComplete)
                        {
                            userInfoItem.TextQuan.text = GameUtils.ShortCutNumber(UserInfo.Gold);
                            DOTween.Kill(userInfoItem.CanvasGroup);
                            if (appearTarget)
                            {
                                userInfoItem.CanvasGroup.DOFade(0, 0.3f).SetDelay(0.8f).OnComplete(() =>
                                {
                                    userInfoItem.CanvasGroup.gameObject.SetActive(true);
                                });
                            }
                            MainGameController.Instance.Wait(0.1f, () => { onComplete?.Invoke(); });
                        }
                        valueBall += step;
                        userInfoItem.TextQuan.text = GameUtils.ShortCutNumber(Mathf.RoundToInt(valueBall));
                        //SoundController.Instance.PlaySoundEffectOneShot("Coin_Collect");
                        if (!isSound)
                        {
                            isSound = true;
                        }
                    });
                });


                
            });
        }
    } 
    
    public static void StartEffGoldCustom(UserInfoItem userInfoItem, int start, int quantity, Vector3 vtStart,  System.Action onComplete)
    {
        int numGoldItem = quantity;
        numGoldItem = (numGoldItem > 10) ? 10 : numGoldItem;
        float delay = 0.6f;
        int numComplete = 0;

        int startGold = start ;
        userInfoItem.TextQuan.text = startGold.ToString();
        float valueBall = startGold;
        float step = quantity * 1.0f / numGoldItem;

        bool isSound = false;

        userInfoItem.CanvasGroup.gameObject.SetActive(true);
        bool appearTarget = false;
        if (!userInfoItem.gameObject.activeInHierarchy)
        {
            appearTarget = true;
        }
        if (appearTarget)
        {
            if (!DOTween.IsTweening(userInfoItem.CanvasGroup))
            {
                userInfoItem.CanvasGroup.alpha = 0;
            }
            else DOTween.Kill(userInfoItem.CanvasGroup);
            userInfoItem.CanvasGroup.DOFade(1, 0.3f);
        }
        for (int i = 0; i < numGoldItem; i++)
        {
            MainGameController.Instance.Wait(i * 0.05f, () =>
            {
                GoldItem goGold = PoolManager.Instance.GetGoldItem();
                goGold.transform.localScale = Vector3.one * 0.6f;
                goGold.transform.position = vtStart;
                goGold.transform.DOMove(new Vector3(vtStart.x + GameUtils.RandomRange(-1.6f, 1.6f), vtStart.y + GameUtils.RandomRange(-1.0f, 1.0f), 0), 0.3f);
                goGold.transform.DOScale(0.3f, 0.3f).SetEase(Ease.InQuad).SetDelay(i * 0.05f + delay);
                goGold.transform.DOMoveX(userInfoItem.Image.transform.position.x, 0.3f).SetEase(Ease.InQuad).SetDelay(i * 0.05f + delay);
                goGold.transform.DOMoveY(userInfoItem.Image.transform.position.y, 0.3f).SetEase(Ease.OutQuad).SetDelay(i * 0.05f + delay).OnComplete(() => {
                    userInfoItem.Image.transform.SquishItem();
                    goGold.ReturnToPool();
                    numComplete++;
                    if (numGoldItem == numComplete) {
                        userInfoItem.TextQuan.text = UserInfo.Gold.ToString();
                        DOTween.Kill(userInfoItem.CanvasGroup);
                        if (appearTarget)
                        {
                            userInfoItem.CanvasGroup.DOFade(0, 0.3f).SetDelay(0.8f).OnComplete(() =>
                            {
                                userInfoItem.CanvasGroup.gameObject.SetActive(true);
                            });
                        }
                        onComplete?.Invoke();
                    }
                    valueBall += step;
                    userInfoItem.TextQuan.text = Mathf.RoundToInt(valueBall).ToString();
                    //SoundController.Instance.PlaySoundEffectOneShot("Coin_Collect");
                    if (!isSound)
                    {
                        isSound = true;
                    }
                });
            });
        }
    }

}
