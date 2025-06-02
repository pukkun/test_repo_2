#if LION_SDK
using LionStudios.Suite.Ads;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AdsHelper
{
    public static System.Action<bool> OnRewardedStatusChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
#if LION_SDK
        LionAds.OnRewardedStatusChanged += haveRewave =>
        {
            OnRewardedStatusChanged?.Invoke(haveRewave);
        };
#endif
    }

    public static bool HaveReward()
    {
#if UNITY_EDITOR
        //return false;
#endif
#if LION_SDK
        return LionAds.IsRewardedReady;
#endif
        return true;
    }
    public static void ShowReward(string placement, int level, System.Action onSuccess, System.Action onFail, System.Action onClosed = null, List<Dictionary<string, object>> rewardsGift = null, Dictionary<string, object> additionalData = null)
    {

#if LION_SDK && !UNITY_EDITOR
        TrackingHelper.RewardVideoShow(placement, level, additionalData);
        if (HaveReward())
        {
            TrackingHelper.RewardVideoStart(placement, level, additionalData);
            LionAds.TryShowRewarded(placement, onSuccess, () =>
            {
                TrackingHelper.RewardVideoEnd(placement, level, additionalData);
                onClosed?.Invoke();
            }, TrackingHelper.ParseGiftToLionRewardFormat(rewardsGift), additionalData);
        }
        else
        {
            onFail?.Invoke();
        }
#else
        onSuccess.Invoke();
#endif
    }
}
