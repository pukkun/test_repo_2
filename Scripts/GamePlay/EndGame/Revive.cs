using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Revive : MonoBehaviour
{
    [SerializeField] private Button btnPlayOn = null;
    [SerializeField] private Button btnGiveUp = null;
    [SerializeField] private Text txtTitle = null;
    [SerializeField] private Text txtDesc = null;
    [SerializeField] private Image[] imageContent = null;

    public System.Action OnPlayOn;
    public System.Action OnGiveUp;
    private void Awake()
    {
        btnPlayOn.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(()=> OnPlayOn?.Invoke()));
        btnGiveUp.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(() => OnGiveUp?.Invoke()));
    }

    public void ShowRevive(Enums.TypeLoseGame typeLoseGame)
    {
        SoundController.Instance.PlaySoundEffectOneShot("Alert_Revive");
        string type = getStringType(typeLoseGame);
        txtTitle.text = LanguageHelper.GetTextByKey(type);
        txtDesc.text = LanguageHelper.GetTextByKey($"{type}_content");
        foreach (var item in imageContent)
        {
            item.gameObject.SetActive(item.name == type);
        }
    }

    private string getStringType(Enums.TypeLoseGame typeLoseGame)
    {
        if (typeLoseGame == Enums.TypeLoseGame.TimeUp) return "time_up";
        if (typeLoseGame == Enums.TypeLoseGame.NoSpace) return "out_of_space";
        if (typeLoseGame == Enums.TypeLoseGame.Bomb) return "bomb";
        return string.Empty;
    }
}
