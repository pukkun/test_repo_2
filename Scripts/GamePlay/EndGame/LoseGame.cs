using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseGame : MonoBehaviour
{
    [SerializeField] private Text txtTitle = null;
    [SerializeField] private Button btnReplay = null;

    public System.Action OnReplay;

    private void Awake()
    {
        btnReplay.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(onReplay));
    }

    public void ShowEndGame(int level)
    {
        txtTitle.text = $"Level {level}";
        SoundController.Instance.PlaySoundEffectOneShot("STGR_Lose_Puzzle");
    }
    private void onReplay()
    {
        OnReplay?.Invoke();
    }
}
