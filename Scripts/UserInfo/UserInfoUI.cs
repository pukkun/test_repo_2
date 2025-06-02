using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup cvgMain = null;
    [SerializeField] private UserInfoItem userGold = null;
    public void ShowMotion()
    {
        cvgMain.alpha = 0;
        cvgMain.transform.localPosition = new Vector3(0, 100);
        cvgMain.DOFade(1, 0.2f).SetEase(Ease.Linear);
        cvgMain.transform.DOLocalMoveY(0, 0.2f).SetEase(Ease.Linear);
    }

    public void HideMotion()
    {
        cvgMain.alpha = 0;
        cvgMain.transform.localPosition = new Vector3(0, -100);
        cvgMain.DOFade(0, 0.2f).SetEase(Ease.Linear);
        cvgMain.transform.DOLocalMoveY(0, 0.2f).SetEase(Ease.Linear);
    }
    public void ShowGold()
    {
        userGold.gameObject.SetActive(true);
        userGold.ShowValue(UserInfo.Gold.ToString());
    }
    public void ShowLife()
    {
        
    }
    public void ShowInfo(params Enums.UserInfoElement[] _params)
    {
        HideAll();
        foreach (var item in _params)
        {
            switch (item)
            {
                case Enums.UserInfoElement.Gold:
                    ShowGold();
                    break;
                default:
                    break;
            }
        }
    }

    

    public void HideAll()
    {
        userGold.gameObject.SetActive(false);
    }

    public UserInfoItem UserGold => userGold;
}
