using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text txtQuan = null;
    [SerializeField] private Text txtLevel = null;

    [SerializeField] private ButtonScale btnPlus;
    [SerializeField] private Transform transLock;
    [SerializeField] private Image mainImageBooster;
    public System.Action<BoostItem> OnClick;
    public System.Action<BoostItem> OnBuyItem;

    public string Key;
    private int quan;
    private bool isLock;
    public bool AllowClick = true;
    public int Quantity => quan;
    public bool IsLock => isLock;

    private void Start()
    {
        btnPlus.HandleClick = onClickButtonPlus;

    }
    public void InitData(int quan)
    {
        this.quan = quan;
        int levelRequired = 0;
        UpdateGUI();
        levelRequired = GameUtils.GetRequireLevelBooster(Key);
        // switch (Key)
        // {
        //     case "add_hole":
        //         levelRequired = GameUtils.GetRequireLevelBooster();
        //         break;
        //     case "hammer":
        //         levelRequired = GameStatic.LEVEL_BOOST_HAMMER;
        //         break;
        //     case "time":
        //         levelRequired = GameStatic.LEVEL_BOOST_TIME;
        //         break;
        //     case "magnet":
        //         levelRequired = GameStatic.LEVEL_BOOST_MAGNET;
        //         break;
        //     case "double_box":
        //         levelRequired = GameStatic.LEVEL_BOOST_BOX;
        //         break;
        // }
        CheckLevelRequired(levelRequired);
    }

    public void CheckLevelRequired(int levelRequired)
    {
        if (UserInfo.Level < levelRequired && Key != "time")
        {
            isLock = true;
            btnPlus.gameObject.SetActive(false);
            transLock.gameObject.SetActive(true);
            mainImageBooster.gameObject.SetActive(false);
            txtQuan.transform.parent.gameObject.SetActive(false);
            if (levelRequired > 0) txtLevel.text = $"Lvl {levelRequired}";
        }
        else
        {
            isLock = false;
            transLock.gameObject.SetActive(false);
            mainImageBooster.gameObject.SetActive(true);
        }

    }

    public void UpdateGUI()
    {
        txtQuan.text = quan.ToString();
        updateImgQuan();
    }
    private void updateImgQuan()
    {
        btnPlus.gameObject.SetActive(false);
        transLock.gameObject.SetActive(false);
        txtQuan.transform.parent.gameObject.SetActive(true);
        if (quan <= 0)
        {
            btnPlus.gameObject.SetActive(true);
            txtQuan.transform.parent.gameObject.SetActive(false);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!AllowClick) return;
        if (isLock) return;
        if (quan <= 0)
        {
            OnBuyItem?.Invoke(this);
        }
        else OnClick?.Invoke(this);
    }
    private void onClickButtonPlus()
    {
        if (!AllowClick) return;
        OnBuyItem?.Invoke(this);
    }
    public void NotifyBooster(string name)
    {
        if (isLock) return;
        //Animator animator = GetComponent<Animator>();
        //animator.enabled = true;
        //animator.Play(name);
    }
    public void UnNotifyBooster()
    {
        //Animator animator = GetComponent<Animator>();
        //if (animator == null) return;
        //animator.StopPlayback();
        //animator.enabled = false;
    }
}
