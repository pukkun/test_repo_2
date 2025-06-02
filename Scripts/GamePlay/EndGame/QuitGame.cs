using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    [SerializeField] private Button btnGiveUp = null;
    [SerializeField] private Button btnClose = null;

    public System.Action OnClose;
    public System.Action OnGiveUp;
    private void Awake()
    {
        btnClose.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(onClose));
        btnGiveUp.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(onGiveUp));
    }
    private void onClose() { OnClose?.Invoke(); }
    private void onGiveUp() { OnGiveUp?.Invoke(); }
}
