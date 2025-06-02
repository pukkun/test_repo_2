using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteSheetHelper : MonoBehaviour
{
    [Tooltip("Path asset trong resource, dùng cách này khi nhiều bundle xài chung asset")]
    [SerializeField] private string pathRes = null;

    [SerializeField] private Sprite[] sprites = null;

    [SerializeField] private UnityEngine.UI.Image image = null;

    [SerializeField] private SpriteRenderer spriteRenderer = null;

    private int frame;
    bool isNext;

    [SerializeField] private int currentFrame = 0;

    [SerializeField] private TypeAnim typeAnim = TypeAnim.Loop;

    [Range(1, 30)] [SerializeField] int speed = 1;

    [SerializeField] private bool isRunning;
    [SerializeField] private bool nonStop;
    [SerializeField] private bool keepWhenFinish;
    [SerializeField] private float timeDelayLoop = 0;

    [SerializeField] private bool resetWhenDisable;

    public bool IsAutoHide;
    public bool IsAutoRun = true;

    public System.Action HandleEnd;
    public bool IsRunning=>isRunning;
    public enum TypeAnim
    {
        PingPong,
        Once,
        Once_Reverse,
        Loop
    }
    private void Awake()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
        if (!pathRes.IsNullOrEmpty())
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.SetAlpha(0);
            }
            if (image != null)
            {
                image.SetAlpha(0);
            }
            sprites = Resources.LoadAll<Sprite>(pathRes);
            if (spriteRenderer != null)
            {
                spriteRenderer.SetAlpha(1);
                spriteRenderer.sprite = sprites[0];
            }
            if (image != null)
            {
                image.SetAlpha(1);
                image.sprite = sprites[0];
            }
        }
    }

    public void ShowFirstImage()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.SetAlpha(1);
            spriteRenderer.sprite = sprites[0];
            spriteRenderer.gameObject.SetActive(true);
        }
        if (image != null)
        {
            image.SetAlpha(1);
            image.sprite = sprites[0];
            image.gameObject.SetActive(true);
        }
    }
    private void resetState()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.SetAlpha(1);
            spriteRenderer.sprite = sprites[0];
        }
        if (image != null)
        {
            image.SetAlpha(1);
            image.sprite = sprites[0];
        }
    }

    private void OnEnable()
    {
        if (IsAutoRun) StartAnim();
    }
    private void OnDisable()
    {
        if (!nonStop && !gameObject.activeSelf)
        {
            stopAnim();
        }
        if (resetWhenDisable)
        {
            resetState();
        }
    }



    void Update()
    {
        if (!isRunning) return;
        frame++;
        if (frame >= speed)
        {
            frame = 0;
            if (typeAnim == TypeAnim.Once_Reverse)
            {
                isNext = false;
                currentFrame--;
                if (currentFrame <= 1)
                {
                    currentFrame = 1;
                    if (!keepWhenFinish)
                    {
                        stopAnim(true);
                    }
                }
            }
            else if (typeAnim == TypeAnim.PingPong)
            {
                if (currentFrame == sprites.Length)
                {
                    isNext = false;
                }
                else if (currentFrame == 1)
                {
                    isNext = true;
                }

                if (isNext)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame--;
                }
            }
            else
            {
                if (currentFrame == sprites.Length)
                {
                    if (typeAnim == TypeAnim.Once && !keepWhenFinish)
                    {
                        stopAnim(true);
                        return;
                    }
                    else if (typeAnim == TypeAnim.Loop)
                    {
                        this.Wait(timeDelayLoop, () =>
                        {
                            if (gameObject.activeInHierarchy) StartAnim();
                        });
                    }
                }
                else
                {
                    currentFrame++;
                }
            }
            Sprite sprite = sprites[currentFrame - 1];
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
            if (image != null)
            {
                image.sprite = sprite;
            }
        }
    }

    public void StopAnim(bool isEnd = false)
    {
        stopAnim(isEnd);
    }
    private void stopAnim(bool isEnd = false)
    {
        isRunning = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
            spriteRenderer.gameObject.SetActive(false);
        }
        if (image != null)
        {
            image.gameObject.SetActive(false);
        }
        if (isEnd)
        {
            HandleEnd?.Invoke();
        }
        if (IsAutoHide)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartAnim()
    {
        currentFrame = 1;
        frame = 0;
        isRunning = true;
        if (typeAnim == TypeAnim.Once_Reverse)
        {
            currentFrame = sprites.Length - 1;
        }
        if (spriteRenderer != null)
        {
            Sprite sprite = sprites[currentFrame - 1];
            spriteRenderer.sprite = sprite;
            spriteRenderer.gameObject.SetActive(true);
        }
        if (image != null)
        {
            image.gameObject.SetActive(true);
        }
    }

    public void SetOrder(int order)
    {
        if(spriteRenderer != null) spriteRenderer.sortingOrder = order;
    }
    public void SetLayer(string layer)
    {
        if (spriteRenderer != null) spriteRenderer.sortingLayerName = layer;
    }
}
