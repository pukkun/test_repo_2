using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BoosterUnlock : MonoBehaviour
{
    [SerializeField] private Transform transformMain = null;
    [SerializeField] private Image imageItem = null;
    [SerializeField] private CanvasGroup imageDark = null;
    [SerializeField] private Transform transformEffect_1 = null;
    [SerializeField] private Transform transformEffect_2 = null;
    [SerializeField] private Button btnClaim = null;
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] private Image imgMainBG = null;
    [SerializeField] private Text txtNameBoost = null;
    [SerializeField] private Text txtDescription = null;
    [SerializeField] private Text txtQuantity = null;
    BoostItem boostItem;
    System.Action actionClose;
    int quan = 2;
    private void Awake()
    {
        btnClaim.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(startAnim));
    }
    public void Apppear(BoostItem boostItem, System.Action actionClose)
    {
        string keyBoost = boostItem.Key;
        this.boostItem = boostItem;
        this.actionClose = actionClose;
        ButtonScale btnScale = btnClaim.GetComponent<ButtonScale>();
        btnScale.EffIdle = true;
        btnScale.AllowPointer = true;
        imageDark.alpha = 0.9f;
        transformMain.localScale = Vector3.one;
        imageItem.transform.localPosition = new Vector3(0, 70);
        imageItem.transform.localScale = Vector3.zero;
        string keyItem = keyBoost;
        imageItem.sprite = ItemUtils.GetSpriteItemByData(new Dictionary<string, object>() { { "item", keyItem } });
        transformEffect_1.gameObject.SetActive(false);
        imgMainBG.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, $"img_{keyBoost}");
        txtNameBoost.text = LanguageHelper.GetTextByKey($"boost_{keyBoost}_name");
        txtDescription.text = LanguageHelper.GetTextByKey($"boost_{keyBoost}_des");
        txtQuantity.text = $"x{quan}";
    }
    private void startAnim()
    {
        //SoundController.Instance.PlaySoundEffectOneShot("BonusAds");
        ButtonScale btnScale = btnClaim.GetComponent<ButtonScale>();
        btnScale.EffIdle = false;
        btnScale.AllowPointer = false;
        transformMain.DOScale(0.682f, 0.15f).SetDelay(0.1f).SetEase(Ease.Linear).OnComplete(() => {
            transformMain.localScale = Vector3.zero;
            imageItem.transform.localScale = Vector3.one * 0.6f;
            this.Wait(0.1f, () =>
            {
                transformEffect_1.gameObject.SetActive(true);
            });
            imageItem.transform.DOScale(1.3f, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
            {
                imageItem.transform.DOScale(1, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    imageDark.DOFade(0, 0.3f).SetEase(Ease.Linear).SetDelay(0.1f).OnComplete(() => {

                    });
                    this.Wait(0.2f, onMoving);
                });
            });
        });
    }
    private void onMoving()
    {
        transformEffect_1.gameObject.SetActive(false);
        imageItem.transform.DOScale(0.45f, 0.2f);
        transformEffect_2.position = boostItem.transform.position;
        imageItem.transform.DOMove(boostItem.transform.position, 0.2f).OnComplete(() => {
            //SoundController.Instance.PlaySoundEffectOneShot("ItemOnButton");
            imageItem.transform.localScale = Vector3.zero;
            transformEffect_2.gameObject.SetActive(true);
            this.Wait(0.3f, () => {
                UserInfo.AddItem(boostItem.Key, quan, "unlock_boost", "unlock_boost");
                GameUtils.SaveDataPref($"earn_booster_free_at_level_{UserInfo.Level}", 1);
                actionClose?.Invoke();
                gameObject.SetActive(false);
                transformEffect_2.gameObject.SetActive(false);
            });
        });
    }
}
