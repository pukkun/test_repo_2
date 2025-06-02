#if LION_SDK
using LionStudios.Suite.Analytics;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public static class TrackingHelper
{
    #region Misson Event
    /// <summary>
    /// Gọi khi bắt đầu chơi gameplay, nếu có tính năng level chest thì gọi 2 event
    /// </summary>
    /// <param name="missionType"> "main" or "level_chest" or mode chơi gì đó</param>
    /// <param name="missionName">main_<level_hiện_tại></param>
    /// <param name="missionID">level hiện tại</param>
    /// <param name="isTutorial">phải hướng dẫn hay ko?</param>
    /// <param name="additionalData"></param>
    public static void MissionStarted(bool isTutorial, string missionType, string missionName, string missionID, Dictionary<string, object> additionalData = null)
    {
#if LION_SDK
        int missionAttempt = GameUtils.GetIntPref($"count_play_{missionID}", 0);
        missionAttempt++;
        GameUtils.SaveDataPref($"count_play_{missionID}", missionAttempt);
        LionAnalytics.MissionStarted(isTutorial, missionType, missionName, missionID, missionAttempt, additionalData);
#endif
    }
    /// <summary>
    /// gọi khi bấm revive gameplay, nếu hoàn tất chest level thì missionType = level_chest
    /// các param tương tự MissionStarted
    /// </summary>
    /// <param name="isTutorial"></param>
    /// <param name="missionType"></param>
    /// <param name="missionName"></param>
    /// <param name="missionID"></param>
    /// <param name="userScore">điểm hiện tại</param>
    /// <param name="additionalData"></param>
    /// <param name="reward"></param>
    /// <param name="stepName">revive</param>
    public static void MissionStep(bool isTutorial, string missionType, string missionName, string missionID, int userScore, Dictionary<string, object> additionalData = null, string stepName = null)
    {
        Debug.LogError("MissionStep");
#if LION_SDK
        int missionAttempt = GameUtils.GetIntPref($"count_play_{missionID}", 0);
        if(missionAttempt == 0)
        {
            Debug.LogError("PHAI GOI MissionStarted TRUOC");
        }
        LionAnalytics.MissionStep(isTutorial, missionType, missionName, missionID, userScore,
            missionAttempt,
            additionalData, 
            null, 
            stepName);
#endif
    }


    /// <summary>
    /// gọi khi win level
    /// </summary>
    /// <param name="isTutorial"></param>
    /// <param name="missionType"></param>
    /// <param name="missionName"></param>
    /// <param name="missionID"></param>
    /// <param name="userScore"></param>
    /// <param name="additionalData"></param>
    /// <param name="reward"></param>
    public static void MissionCompleted(bool isTutorial, string missionType, string missionName, string missionID, int userScore, Dictionary<string, object> additionalData = null, List<Dictionary<string, object>> rewardGift = null)
    {
        Debug.LogError("MissionCompleted");
#if LION_SDK
        int missionAttempt = GameUtils.GetIntPref($"count_play_{missionID}", 0);
        if (missionAttempt == 0)
        {
            Debug.LogError("PHAI GOI MissionStarted TRUOC");
        }
        LionAnalytics.MissionCompleted(isTutorial, missionType, missionName, missionID, userScore,
            missionAttempt,
            additionalData
            //remove ,ParseGiftToLionRewardFormat(rewardGift)
            );
#endif
    }

    /// <summary>
    /// gọi khi user bấm quit / bấm restart
    /// </summary>
    /// <param name="isTutorial"></param>
    /// <param name="missionType"></param>
    /// <param name="missionName"></param>
    /// <param name="missionID"></param>
    /// <param name="userScore"></param>
    /// <param name="additionalData"></param>
    public static void MissionAbandoned(bool isTutorial, string missionType, string missionName, string missionID, int userScore, Dictionary<string, object> additionalData = null)
    {
        Debug.LogError("MissionAbandoned");
#if LION_SDK
        int missionAttempt = GameUtils.GetIntPref($"count_play_{missionID}", 0);
        if (missionAttempt == 0)
        {
            Debug.LogError("PHAI GOI MissionStarted TRUOC");
        }
        LionAnalytics.MissionAbandoned(isTutorial, missionType, missionName, missionID, userScore,
            missionAttempt,
            additionalData
            );
#endif
    }



    /// <summary>
    /// gọi khi user thua mà ko chọn revive
    /// nếu ở màn hình revive user quit app --> cache lại gọi khi mở game
    /// </summary>
    /// <param name="isTutorial"></param>
    /// <param name="missionType"></param>
    /// <param name="missionName"></param>
    /// <param name="missionID"></param>
    /// <param name="userScore"></param>
    /// <param name="missionAttempt"></param>
    /// <param name="additionalData"></param>
    /// <param name="failReason">timeover</param>
    public static void MissionFailed(bool isTutorial, string missionType, string missionName, string missionID, int userScore, Dictionary<string, object> additionalData = null, string failReason = null)
    {
        Debug.LogError("MissionFailed");
#if LION_SDK
        int missionAttempt = GameUtils.GetIntPref($"count_play_{missionID}", 0);
        if (missionAttempt == 0)
        {
            Debug.LogError("PHAI GOI MissionStarted TRUOC");
        }
        LionAnalytics.MissionFailed(isTutorial, missionType, missionName, missionID, userScore,
            missionAttempt,
            additionalData,
            failReason
            );
#endif
    }

#if LION_SDK
    public static Reward ParseGiftToLionRewardFormat(List<Dictionary<string, object>> rewardsGift)
    {
        if (rewardsGift.IsNullOrEmpty()) return null;
        Reward reward = new Reward(ParseGiftToLionProductFormat(rewardsGift));
        return reward;
    }
    public static Product ParseGiftToLionProductFormat(List<Dictionary<string, object>> rewardsGift)
    {
        if (rewardsGift.IsNullOrEmpty()) return null;
        Product product = new Product();
        List<VirtualCurrency> virtualCurrencies = new List<VirtualCurrency>();
        List<Item> items = new List<Item>();
        foreach (var entry in rewardsGift)
        {
            Dictionary<string, object> gift = entry as Dictionary<string, object>;
            string item = gift.GetString("item");
            int quantity = gift.GetInt("q");
            if (item == "gold")
            {
                virtualCurrencies.Add(new VirtualCurrency("coins", "soft", quantity));
            }
            else
            {
                items.Add(new Item(item, "power_up", quantity));
            }
        }
        product.virtualCurrencies = virtualCurrencies;
        product.items = items;
        return product;
    }
#endif
    #endregion


    #region Reward Video Event

    /// <summary>
    /// Gọi khi user bấm nút xem
    /// </summary>
    /// <param name="placement"></param>
    /// <param name="level"></param>
    /// <param name="additionalData">{"mission_data":{"mission_type":"str","mission_name":"str","mission_id":"int","mission_attempt":"int"}}</param>
    public static void RewardVideoShow(string placement, int level, Dictionary<string, object> additionalData)
    {
#if LION_SDK
        LionAnalytics.RewardVideoShow(placement, "applovin", level, additionalData);
#endif
    }

    public static void RewardVideoStart(string placement, int level, Dictionary<string, object> additionalData)
    {
#if LION_SDK
        LionAnalytics.RewardVideoStart(placement, "applovin", level, additionalData);
#endif
    }

    public static void RewardVideoEnd(string placement, int level, Dictionary<string, object> additionalData)
    {
#if LION_SDK
        LionAnalytics.RewardVideoEnd(placement, "applovin", level, additionalData);
#endif
    }

    public static void RewardVideoCollect(string placement, int level, Dictionary<string, object> additionalData, List<Dictionary<string, object>> rewardGift)
    {
#if LION_SDK
        LionAnalytics.RewardVideoCollect(placement, ParseGiftToLionRewardFormat(rewardGift), level, additionalData);

#endif
    }
    #endregion

    #region Resource Event
    /// <summary>
    /// gọi khi xài booster
    /// </summary>
    /// <param name="missionID"></param>
    /// <param name="missionType"></param>
    /// <param name="missionAttempt"></param>
    /// <param name="powerUpName"></param>
    /// <param name="additionalData"></param>
    public static void PowerUpUsed(string missionID, string missionType, string powerUpName, string missionName, Dictionary<string, object> additionalData = null)
    {
#if LION_SDK
        int missionAttempt = GameUtils.GetIntPref($"count_play_{missionID}", 0);
        if (missionAttempt == 0)
        {
            Debug.LogError("PHAI GOI MissionStarted TRUOC");
        }
        LionAnalytics.PowerUpUsed(missionID, missionType, missionAttempt, powerUpName, missionName, additionalData);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="purchaseName"></param>
    /// <param name="cost"></param>
    /// <param name="received"></param>
    /// <param name="purchaseLocation"></param>
    /// <param name="additionalData"></param>
    public static void EconomyEventBuyBoosterByGold(
        List<Dictionary<string, object>> cost,
        List<Dictionary<string, object>> received, 
        string purchaseLocation = "General", 
         Dictionary<string, object> additionalData = null)
    {
#if LION_SDK 
        //LionStudios.Suite.Analytics.Events.ReceiptStatus status = (LionStudios.Suite.Analytics.Events.ReceiptStatus)((int)receiptStatus);
        LionAnalytics.EconomyEvent("coin_buy_booster", ParseGiftToLionProductFormat(cost),
            ParseGiftToLionProductFormat(received),
            "main", null, null, additionalData
            ); 
#endif
    }
    #endregion
}
