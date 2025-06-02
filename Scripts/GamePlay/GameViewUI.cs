using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameViewUI : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] EndGame endGame;
    [SerializeField] Text txtLevel;
    [SerializeField] Transform transformTimer;
    [SerializeField] Image imageTimerBG;
    [SerializeField] Image imageDecorBG;
    [SerializeField] Text txtTimer;
    [SerializeField] Button btnSetting = null;
    [SerializeField] Button btnReplay = null;
    [SerializeField] GameManager gameManager = null;
    [SerializeField] private BoosterUnlock boosterUnlock = null;
    [SerializeField] private NewFeatureGamePlay newFeatureGamePlay = null;
    [SerializeField] private Transform transformLevelHard = null;
    [SerializeField] private Text txtHelpGuide = null;

    [SerializeField] CanvasGroup cgFreeze = null;
    [SerializeField] Image imgFreezeTimer = null;
    [SerializeField] ParticleSystem particleSystemSnow = null;
    [SerializeField] ClockwiseAnimation clockwiseAnimation = null;
    [SerializeField] Transform transformHandGuide = null;
    TimerController timerController;
    Color timeUpColor;
    bool isReviving;
    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#FF9966", out timeUpColor);
        btnReplay.onClick.AddListener(onQuit);
        btnSetting.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(()=> {
            if (!gameManager.AllowAction) return;
            MainGameController.Instance.ShowSetting(true);
        }));
        ResetView();
        endGame.OnHide += () => { gameManager.SetDepthCameraButterFly(0); };
        endGame.OnShow += () => { gameManager.SetDepthCameraButterFly(-2); };
        endGame.AddEventRevive(()=> {
            AdsHelper.ShowReward("revive", gameManager.CurrentLevel, () =>
            {
                isReviving = true;
                endGame.HideEndGame();
                gameManager.OnRevive();
            }, () => {
                MainGameController.Instance.ShowBubbleAlertNoAds();
            }, null, null, GameUtils.GetMissionDataTracking(gameManager.CurrentLevel));
            
        }, () =>
        {
            endGame.HideEndGame();
            endGame.ShowEndGame(false, gameManager.CurrentLevel, gameManager.TypeLoseGame);
        });
        endGame.AddEventNext(onNextLevel, onAdsNextLevel);
        endGame.AddEventReplay(onReplay);
    }
    private void onNextLevel()
    {
        gameManager.NextLevel();
    }
    private void onAdsNextLevel()
    {
        gameManager.NextLevel();
    }
    private void onQuit()
    {
        if (!gameManager.AllowAction) return;
        endGame.gameObject.SetActive(true);
        endGame.ShowQuitGame(() =>
        {
            gameManager.QuitGame();
            PauseTime();
            endGame.HideEndGame();
            endGame.ShowEndGame(false, gameManager.CurrentLevel, Enums.TypeLoseGame.None);
        },
        () => {
            endGame.HideEndGame();
        });
    }
    private void onReplay() 
    {
        gameManager.ReplayGame();
    }
    public void ShowEndGame(bool isWin, int level, Enums.TypeLoseGame typeLoseGame)
    {
        particleSystemSnow.gameObject.SetActive(false);
        if (timerController != null) timerController.PauseTimer();
        endGame.gameObject.SetActive(true);

        endGame.ShowEndGame(isWin, level, typeLoseGame);
    }
    public void ShowRevive(int level, Enums.TypeLoseGame typeLoseGame)
    {
        Dictionary<string, object> dataCache = new Dictionary<string, object>();
        dataCache.Add("lv", level);
        dataCache.Add("score", GameManager.Instance.Score);
        dataCache.Add("type_lose", (int)typeLoseGame);
        GameUtils.SaveDataPref(PrefConstant.CACHE_MISSION_FAIL, dataCache.ToJson());
        PauseTime();
        endGame.gameObject.SetActive(true);
        endGame.ShowRevive(typeLoseGame);
    }
    public void ShowLevel(int level)
    {
        txtLevel.text = $"Level {level}";
        
    }
    public void InitTimer(int timer, bool isRun)
    {
        if (timer < 0) timer = 200;
#if UNITY_EDITOR
        //if (timer < 0) timer = 200;
#endif

        txtTimer.text = GameUtils.ConvertSecondToMSS(timer);
        transformTimer.gameObject.SetActive(true);
        if (timer > 0 && isRun)
        {
            clockwiseAnimation.Play();
            if (timerController != null) timerController.DestroyTimer();
            timerController = GameUtils.CreateTimer(transform);
            timerController.InitTimer(timer, onTimer, onTimeEnd);
        }
    }
    private void onTimer(int time)
    {
        if (time <= 20)
        {
            txtTimer.color = timeUpColor;
            if (!isReviving) SoundController.Instance.PlaySoundEffectOneShot("TimerCountdown");
        }
        txtTimer.text = GameUtils.ConvertSecondToMSS(time);
    }
    private void onTimeEnd()
    {
        clockwiseAnimation.Pause();
        gameManager.TimeOver();
    }
    public int GetCurrentTime()
    {
        if (timerController != null) return timerController.CurrentTime;
        return 0;
    }
    public void PauseTime()
    {
        clockwiseAnimation.Pause();
        if (timerController != null) timerController.PauseTimer();
    }
    public void ResumeTime()
    {
        clockwiseAnimation.Resume();
        if (timerController != null) timerController.ResumeTimer();
    }
    public void ResetView()
    {
        isReviving = false;
        txtTimer.color = Color.white;
        transformTimer.gameObject.SetActive(false);
        cgFreeze.gameObject.SetActive(false);
        imgFreezeTimer.gameObject.SetActive(false);
    }
    public void ShowBoosterUnlock(BoostItem boostItem, System.Action actionClose)
    {
        boosterUnlock.gameObject.SetActive(true);
        boosterUnlock.Apppear(boostItem, actionClose);
    }
    public void ShowNewFeature(string newFeature, System.Action onClose)
    {
        newFeatureGamePlay.gameObject.SetActive(true);
        newFeatureGamePlay.InitData(newFeature, onClose);
    }
    public void FreezeTimer(int time)
    {
        showFreezeTime();
        PauseTime();
        this.Wait(time, EndFreezeTimer);
    }
    public void EndFreezeTimer()
    {
        
        ResumeTime();
        hideFreezeTime();
    }
    private void showFreezeTime()
    {
        particleSystemSnow.gameObject.SetActive(true);
        particleSystemSnow.Play();
        cgFreeze.gameObject.SetActive(true);
        imgFreezeTimer.gameObject.SetActive(true);
        cgFreeze.alpha = 0.0f;
        cgFreeze.DOFade(1.0f, 1.0f);
        imgFreezeTimer.SetAlpha(0.0f);
        imgFreezeTimer.DOFade(1.0f, 1.0f);
    }
    private void hideFreezeTime()
    {
        particleSystemSnow.Stop();
        cgFreeze.DOFade(0.0f, 1.0f).OnComplete(() => {
            cgFreeze.gameObject.SetActive(false);
        });
        imgFreezeTimer.DOFade(0.0f, 1.0f).OnComplete(() => {
            imgFreezeTimer.gameObject.SetActive(false);
        });
    }

    public void ShowLevelHard(bool isLevelHard, System.Action onClose)
    {
        Outline[] outlines = txtLevel.GetComponents<Outline>();
        if (isLevelHard)
        {
            Color colorHard = ColorHelper.HexToColor("#250000");
            outlines.ForEach(s => s.effectColor = colorHard);
            SoundController.Instance.PlaySoundEffectOneShot("rescue_alarm");
            imageTimerBG.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, "bgTimer_hard");
            imageDecorBG.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, "decor_hard");
            transformLevelHard.gameObject.SetActive(true);
            this.Wait(1.2f, onClose);
        }
        else
        {
            imageTimerBG.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, "bgTimer");
            imageDecorBG.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, "decor");
            transformLevelHard.gameObject.SetActive(false);

            Color colorNormal = ColorHelper.HexToColor("#0E480A");
            outlines.ForEach(s => s.effectColor = colorNormal);
        }
    }
    public bool IsHandGuide()
    {
        return transformHandGuide.gameObject.activeInHierarchy;
    }
    public void ShowHandGuide(Vector3 pos)
    {
        pos.z = 0;
        transformHandGuide.position = pos;
        transformHandGuide.gameObject.SetActive(true);
    }
    public void HideHandGuide()
    {
        transformHandGuide.gameObject.SetActive(false);
    }
    public void ShowHelpGuide(string text)
    {
        txtHelpGuide.text = text;
        txtHelpGuide.transform.parent.gameObject.SetActive(true);
    }
    public void HideHelpGuide()
    {
        txtHelpGuide.text = string.Empty;
        txtHelpGuide.transform.parent.gameObject.SetActive(false);
    }
    public void VisibleButtonSetting(bool show)
    {
        btnSetting.gameObject.SetActive(show);
    }
}
