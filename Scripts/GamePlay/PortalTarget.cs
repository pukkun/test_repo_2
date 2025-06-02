using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Enums;

[System.Serializable]
public class PortalTarget : MonoBehaviour
{
    public PortalData PortalData;
    [SerializeField] private SpriteRenderer sprMain = null;
    [SerializeField] private SpriteRenderer sprBlank = null;
    [SerializeField] private SpriteRenderer sprSecret = null;
    [SerializeField] private SpriteRenderer sprCovered = null;
    [SerializeField] private TMPro.TextMeshPro textCoveredTurn = null;
    [SerializeField] private MeshRenderer meshLevel_1 = null;
    [SerializeField] private MeshRenderer meshLevel_2 = null;
    [SerializeField] private Transform transformFxPortal = null;
    [SerializeField] private ParticleSystem psCircle = null;
    [SerializeField] private ParticleSystem psSpark = null;
    [SerializeField] private ParticleSystem psCantMove = null;

    [SerializeField] GameObject fxSnakeAppear = null;

    float timeExpand = 0.15f;
    public bool IsClose;
    public bool IsTempClose;// tam check logic
    float defaultNormal = 0.39f;
    float defaultExpand = 0.44f;
    Vector3 expand = new Vector3(0, 0.047f, -0.004f);
    int turn;
    private void Awake()
    {
        transform.localScale = Vector3.one * 1.2f;
        turn = PortalData.Turn;
        
        ShowLevelPortal();
    }
    public void SetSprite(Sprite spriteMain, Sprite spriteBlank)
    {
        sprMain.sprite = spriteMain;
        sprBlank.sprite = spriteBlank;
        sprBlank.gameObject.SetActive(true);
        sprMain.gameObject.SetActive(true);
    }
    public void ShowAnimAppear()
    {
        float timeScale = 0.15f;
        sprBlank.transform.localScale = Vector3.one * 0.875f * defaultNormal;
        sprMain.transform.localScale = Vector3.one * 0.875f * defaultNormal;

        sprBlank.transform.DOScale(1.1f * defaultNormal, timeScale).OnComplete(() => {
            sprBlank.transform.DOScale(0.95f * defaultNormal, timeScale/2).OnComplete(() => {
                sprBlank.transform.DOScale(1 * defaultNormal, timeScale/2).OnComplete(() => {

                });
            });
        });
        sprMain.transform.DOScale(1.1f * defaultNormal, timeScale).OnComplete(() => {
            sprMain.transform.DOScale(0.95f * defaultNormal, timeScale/2).OnComplete(() => {
                sprMain.transform.DOScale(1 * defaultNormal, timeScale/2).OnComplete(() => {

                });
            });
        });
    }
    public void HideColorHole()
    {
        sprBlank.gameObject.SetActive(false);
        sprMain.gameObject.SetActive(false);
    }
    public void SetMaterial(int turn, GameDataSO.DataMaterial dataMaterial)
    {
        this.turn = turn;
        //meshLevel_1.material = material;
        //meshLevel_2.material = material;
        if (turn == 1)
        {
            SetSprite(dataMaterial.SpritePortal_1, dataMaterial.SpriteBlank);
        }
        else if (turn > 1)
        {
            SetSprite(dataMaterial.SpritePortal_1, dataMaterial.SpriteBlank);
        }
        
        ShowLevelPortal();
    }
    public void ShowLevelPortal()
    {
        sprBlank.gameObject.SetActive(true);
        sprMain.gameObject.SetActive(true);
        meshLevel_1.gameObject.SetActive(false);
        meshLevel_2.gameObject.SetActive(false);
        if (turn == 1)
        {
            meshLevel_1.gameObject.SetActive(true);
        }
        else if (turn > 1)
        {
            meshLevel_2.gameObject.SetActive(true);
        }
        if (PortalData.CoveredTurn > 0)
        {
            sprSecret.gameObject.SetActive(true);
            sprCovered.gameObject.SetActive(true);
            textCoveredTurn.text = PortalData.CoveredTurn.ToString();
        }
    }

    public void ChangeSecretToNormal()
    {
        sprCovered.DOFade(0, 0.15f);
        sprSecret.transform.DOScale(defaultExpand, 0.15f).OnComplete(() =>
        {
            ShowFxAppear();
            sprSecret.DOFade(0, 0.2f);
            sprSecret.transform.DOScale(defaultNormal, 0.2f).OnComplete(() =>
            {


            });
        });
    }

    public void SetColor(Color color)
    {
        Color newColor = color;
        Color newColor2 = color;
        newColor.a = 0.2f;
        var mainCircle = psCircle.main;
        mainCircle.startColor = newColor;

        
        var mainSpark = psSpark.main;
        mainSpark.startColor = newColor2;
    }
    public void ShowFxHole(float levelScale)
    {
        FxHoleItem fxHoleItem = PoolManager.Instance.GetFxHoleItem();
        Vector3 pos = transform.position;
        pos.z = 0.5f;
        fxHoleItem.transform.position = pos;
        fxHoleItem.transform.localScale = Vector3.one * 0.5f * levelScale;
        ParticleSystem[] pss = fxHoleItem.transform.GetComponentsInChildren<ParticleSystem>();
        Color color = GameDataSO.Instance.GetDataMaterial(PortalData.SelectedColor).MainColor;
        foreach (var item in pss)
        {
            var mainSpark = item.main;
            mainSpark.startColor = color;
        }
        SoundController.Instance.PlaySoundEffectOneShot("GetInHole");
        this.Wait(1, () =>
        {
            fxHoleItem.ReturnToPool();
        });
    }
    public void DoExpand()
    {
        transformFxPortal.transform.DOLocalMove(expand, 0.1f);
        sprBlank.transform.DOScale(Vector3.one * defaultExpand, timeExpand);
    }
    public void DoNormal(float delay = 0)
    {
        transformFxPortal.transform.DOLocalMove(Vector3.zero, 0.1f).SetDelay(delay);
        sprBlank.transform.DOScale(Vector3.one * defaultNormal, timeExpand).SetDelay(delay);
    }
    public void RemovePortal()
    {
        transform.DOScale(0, 0.3f);
    }
    public void ActiveTurn()
    {
        textCoveredTurn.text = string.Empty;
        if (PortalData.CoveredTurn > 0)
        {
            textCoveredTurn.text = PortalData.CoveredTurn.ToString();
            PortalData.CoveredTurn--;
            textCoveredTurn.transform.DOScale(1.2f, 0.1f).OnComplete(() => {
                textCoveredTurn.text = PortalData.CoveredTurn.ToString();
                if (PortalData.CoveredTurn == 0)
                {
                    textCoveredTurn.text = string.Empty;
                }
                textCoveredTurn.transform.DOScale(1, 0.1f).OnComplete(() => {

                });
            });
            //textCoveredTurn.text = PortalData.CoveredTurn.ToString();
            if (PortalData.CoveredTurn == 0)
            {
                textCoveredTurn.text = string.Empty;
                ChangeSecretToNormal();
            }
        }
    }

    public bool TruePortal(BlockColor blockColor)
    {
        return blockColor == PortalData.SelectedColor && PortalData.CoveredTurn <= 0;
    }
    public void ShowFxCantMoveToPortal(Color color)
    {
        DoExpand();
        DoNormal(timeExpand);
        psCantMove.gameObject.SetActive(true);
        var mainCircle = psCantMove.main;
        mainCircle.startColor = color;
        float str = 0.1f;
        Vector3 originalPosition = transform.position;
        transform.DOShakePosition(0.1f, new Vector3(str, str, 0f), 10, 90, false, false)
            .OnUpdate(() =>
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, originalPosition.z);
            })
            .OnComplete(() =>
            {
                transform.position = originalPosition;
            });
    }
    public void ShowFxAppear()
    {
        fxSnakeAppear.gameObject.SetActive(true);
    }
}
