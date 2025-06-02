using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public System.Action OnClick;
    public bool IsFull;
    public int IndexSlot;
    Snake snake;
    public Snake Snake => snake;
    public System.Action<Slot, float> OnRelease;//not new
    [SerializeField] TMPro.TextMeshPro txtRemain = null;
    [SerializeField] MeshRenderer meshRenderer = null;
    [SerializeField] Material defaultMaterial = null;
    [SerializeField] PortalTarget portalTarget = null;
    [SerializeField] GameObject fxSnakeAppear = null;
    [SerializeField] SpriteRenderer spr2Light = null;
    [SerializeField] SpriteRenderer iconAds = null;
    [SerializeField] BoxCollider2D boxCollider2D = null;

    public float DelayBlock;
    private void Start()
    {
        //iconAds.gameObject.SetActive(false);
        //boxCollider2D.enabled = false;
        portalTarget.HideColorHole();
        portalTarget.transform.localScale = Vector3.one * 1.3f;
    }
    public void ShowSlotAds()
    {
        iconAds.gameObject.SetActive(true);
        boxCollider2D.enabled = true;
    }
    public void SetSnake(Snake snake)
    {
        //meshRenderer.material = GameDataSO.Instance.GetMaterial(snake.SnakeData.SelectedColor);
        
        this.snake = snake;
        ShowRemain();
        
    }
    public void ShowSlot(Enums.BlockColor blockColor)
    {
        GameDataSO.DataMaterial DataMaterial = GameDataSO.Instance.GetDataMaterial(blockColor);
        portalTarget.SetSprite(DataMaterial.SpritePortal_1, DataMaterial.SpriteBlank);
        portalTarget.ShowAnimAppear();
        //this.Wait(0.15f, () =>
        //{
        //    fxSnakeAppear.gameObject.SetActive(true);
        //});
    }
    public void ShowFxAppear()
    {
        fxSnakeAppear.gameObject.SetActive(true);
    }
    public void ShowSlotGray()
    {
        portalTarget.HideColorHole();
        //meshRenderer.material = defaultMaterial;
    }
    public void ClearSnake(float waitRelease = 0)
    {
        snake = null;
        IsFull = false;
        txtRemain.text = string.Empty;
        OnRelease?.Invoke(this, waitRelease);
    }
    public void ShowRemain()
    {
        int remain = snake.GetRemainView();
        txtRemain.text = remain.ToString();
        if (remain == 0) txtRemain.text = string.Empty;
    }

    public void ShowEffectComplete()
    {
        GameObject go = Instantiate(GameManager.Instance.GameDataSO.PrefabFxAppearButterFly, transform);
        go.transform.localPosition = Vector3.zero * 5;//Vector3.back * 10;
        go.transform.localScale = Vector3.one * 0.5f;
        Destroy(go.gameObject, 1);
    }
    public void Show2Light()
    {
        spr2Light.gameObject.SetActive(true);
    }
    public void Hide2Light()
    {
        spr2Light.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDownOnMouseDownOnMouseDownOnMouseDown");
        OnClick?.Invoke();
    }
}
