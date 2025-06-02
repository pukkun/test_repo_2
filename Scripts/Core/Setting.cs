using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] ButtonScale btnClose = null;
    [SerializeField] ButtonScale btnPolicy = null;
    [SerializeField] ButtonOnOff btnMusic = null;
    [SerializeField] ButtonOnOff btnSound = null;
    [SerializeField] ButtonOnOff btnVibration = null;
    [SerializeField] RectTransform rectMain = null;
    [SerializeField] Transform transInHome = null;
    [SerializeField] Transform transInGame = null;
    [SerializeField] ButtonScale btnContinue = null;
    [SerializeField] ButtonScale btnPrivacy = null;

    [Header("Dev Test")]
    [SerializeField] Transform devFunction = null;
    [SerializeField] InputField inputFieldLevel = null;
    [SerializeField] Button btnPlay = null;
    [SerializeField] Button btnWin = null;
    public System.Action OnClickHome;

    private void Start()
    {
        devFunction.gameObject.SetActive(!GameUtils.IsProduction() && SceneManager.GetActiveScene().name == SceneConstant.SCENE_GAME);
        btnPlay.onClick.AddListener(() =>
        {
            int level = inputFieldLevel.text.ToInt();
            if(level >= 1 && level <= GameStatic.MAX_LEVEL)
            {
                UserInfo.Level = level;
                if (SceneManager.GetActiveScene().name == SceneConstant.SCENE_GAME) GameManager.Instance.NextLevel();
                HideSetting();
            }
        });
        btnWin.onClick.AddListener(() =>
        {
            if (SceneManager.GetActiveScene().name == SceneConstant.SCENE_GAME) GameManager.Instance.DevWin();
            HideSetting();
        });
        btnPolicy.HandleClick = delegate {
            //SoundController.Instance.PlaySoundEffectOneShot("Tap_Sound");
            //GameUtils.DeviceVibrate();
            GameUtils.DelegatActionWithNormalSound(null).Invoke();
            Application.OpenURL(GameStatic.DataLinks.GetString("policy"));
        };
        btnClose.HandleClick = ()=> {
            //SoundController.Instance.PlaySoundEffectOneShot("Tap_Sound");
            //GameUtils.DeviceVibrate();
            
            //HideSetting();
            GameUtils.DelegatActionWithNormalSound(HideSetting).Invoke();
        };
        btnContinue.HandleClick = () =>
        {
            GameUtils.DelegatActionWithNormalSound(onClickContinue).Invoke();
            //SoundController.Instance.PlaySoundEffectOneShot("Tap_Sound");
            //GameUtils.DeviceVibrate();
            onClickContinue();
        };

        btnSound.onClick.AddListener(()=> {
            SoundController.Instance.PlaySoundEffectOneShot("sndButtonSwitch");
            HapticHelper.DeviceVibrate();
            SoundController.Instance.ChangeSoundState();
        });
        btnMusic.onClick.AddListener(()=> {
            SoundController.Instance.PlaySoundEffectOneShot("sndButtonSwitch");
            HapticHelper.DeviceVibrate();
            SoundController.Instance.ChangeSoundState();
        });
        btnVibration.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(() =>
        {
            GameStatic.AllowVirate = btnVibration.IsOn;
            SoundController.Instance.PlaySoundEffectOneShot("sndButtonSwitch");
            HapticHelper.DeviceVibrate();
            GameUtils.SaveDataPref("allow_vibration", (GameStatic.AllowVirate) ? 1 : 0, false);
        }));
    }
    public void OnShow(bool inGamePlay = false)
    {
        gameObject.SetActive(true);
        btnSound.IsOn = SoundController.Instance.StatusSFX == 1;
        btnMusic.IsOn = SoundController.Instance.StatusSoundMusic == 1;
        btnVibration.IsOn = GameStatic.AllowVirate;

        transInGame.gameObject.SetActive(inGamePlay);
        transInHome.gameObject.SetActive(!inGamePlay);
        btnPolicy.gameObject.SetActive(!inGamePlay);
    }

    public void HideSetting()
    {
        gameObject.SetActive(false);
    }
    private void onClickContinue()
    {
        HideSetting();
    }
}
