using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HapticHelper : MonoBehaviour
{
    private static bool checkIntervalHaptic = true;
    public static void DeviceVibrate(int week = 50)
    {
        //Debug.LogError($"AllowVirate {GameStatic.AllowVirate} -- check interval: {checkIntervalHaptic}");
        if (!GameStatic.AllowVirate) return;
        if (!checkIntervalHaptic) return;
        checkIntervalHaptic = false;
        DOVirtual.DelayedCall(0.5f, () => {
            checkIntervalHaptic = true;
        });
#if UNITY_EDITOR
        return;
#endif
        //Handheld.Vibrate();
#if UNITY_ANDROID
        Vibration.VibratePop();
#elif UNITY_IOS
        Vibration.VibratePop();
        // Vibration.Vibrate();
#endif
    }
}
