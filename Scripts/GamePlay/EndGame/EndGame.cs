using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class EndGame : MonoBehaviour
{
    [SerializeField] private WinGame winGame = null;
    [SerializeField] private LoseGame loseGame = null;
    [SerializeField] private QuitGame quitGame = null;
    [SerializeField] private Revive revive = null;

    public System.Action OnShow;
    public System.Action OnHide;
    public void AddEventRevive(System.Action onPlayOn, System.Action onGiveUp)
    {
        revive.OnPlayOn += onPlayOn;
        revive.OnGiveUp += onGiveUp;
    }
    public void AddEventNext(System.Action onNext, System.Action onAdsNext)
    {
        winGame.OnNext += onNext;
        winGame.OnAdsNext += onAdsNext;
        
        winGame.OnNext += HideEndGame;
        winGame.OnAdsNext += HideEndGame;
    }
    public void AddEventReplay(System.Action onReplay)
    {
        loseGame.OnReplay += onReplay;
        loseGame.OnReplay += HideEndGame;
    }
    public void HideEndGame()
    {
        winGame.gameObject.SetActive(false);
        loseGame.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);
        revive.gameObject.SetActive(false);
        OnHide?.Invoke();
    }
    public void ShowRevive(Enums.TypeLoseGame typeLoseGame)
    {
        OnShow?.Invoke();
        winGame.gameObject.SetActive(false);
        loseGame.gameObject.SetActive(false);
        revive.gameObject.SetActive(true);
        revive.ShowRevive(typeLoseGame);
    }
    public void ShowQuitGame(System.Action onGiveUp, System.Action onClose) 
    {
        OnShow?.Invoke();
        quitGame.gameObject.SetActive(true);
        quitGame.OnGiveUp = onGiveUp;
        quitGame.OnClose = onClose;
    }
    public void ShowEndGame(bool isWin, int level, TypeLoseGame typeLoseGame)
    {
        OnShow?.Invoke();
        winGame.gameObject.SetActive(false);
        loseGame.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);
        if (isWin)
        {
            winGame.gameObject.SetActive(true);
            winGame.ShowEndGame(level);
        }
        else
        {

            loseGame.gameObject.SetActive(true);
            loseGame.ShowEndGame(level);

            if (typeLoseGame != TypeLoseGame.None)
            {
                GameUtils.SaveDataPref(PrefConstant.CACHE_MISSION_FAIL, string.Empty);
                TrackingHelper.MissionFailed(level == 1,
                TrackingConstant.MType_MAIN,
                TrackingConstant.MName_MAIN_NAME + level,
                level.ToString(),
                GameManager.Instance.Score,
                    GameUtils.GetMissionDataTracking(level),
                typeLoseGame.ToString()

                );
            }
        }
    }
}
