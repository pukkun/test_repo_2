using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoItem : MonoBehaviour
{
    [SerializeField] private Image imageIcon = null;
    [SerializeField] private Text txtQuan = null;
    [SerializeField] private CanvasGroup canvasGroup = null;


    public void ShowValue(string str)
    {
        txtQuan.text = str;
    }


    public Image Image => imageIcon;
    public Text TextQuan => txtQuan;
    public CanvasGroup CanvasGroup => canvasGroup;
}
