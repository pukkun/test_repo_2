using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewFeatureGamePlay : MonoBehaviour
{
    [SerializeField] private Text textComing = null;
    [SerializeField] private Text textDes1 = null;
    [SerializeField] private Text textDes2 = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Button btnConinue = null;

    [SerializeField] Sprite[] sprites = null;

    System.Action onClose;
    private void Awake()
    {
        btnConinue.onClick.AddListener(GameUtils.DelegatActionWithNormalSound(() => {
            gameObject.SetActive(false);
            onClose?.Invoke();
        }));
    }
    public void InitData(string feature, System.Action onClose)
    {
        this.onClose = onClose;
        textComing.text = LanguageHelper.GetTextByKey($"new_feature_coming_{feature}");
        textDes1.text = LanguageHelper.GetTextByKey($"new_feature_des1_{feature}");
        textDes2.text = LanguageHelper.GetTextByKey($"new_feature_des2_{feature}");
        image.sprite = SpriteAtlasHelper.GetSpriteByName(sprites, feature);
        image.SetNativeSize();
    }
}
