using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
[SelectionBase]
public class TargetBlock : FxPoolElement
{
    [SerializeField] private SpriteRenderer sprCube = null;
    [SerializeField] private SpriteRenderer sprCircle = null;
    [SerializeField] private MeshRenderer meshRenderer = null;
    [SerializeField] private MeshRenderer meshRendererCircle = null;
    [SerializeField] private Transform transformContainer = null;

    [SerializeField] public Enums.BlockColor BlockColor;
    [SerializeField] public Rigidbody rigidbody = null;
    [SerializeField] private SpriteRenderer spriteRendererBlurEffect = null;
    private Tween moveTween;
    public bool IsReady;
    GameManager gameManager;

    private int width;
    private int height;


    protected override void Awake()
    {
        spriteRendererBlurEffect.gameObject.SetActive(false);
    }
    public void ShowBlur(float delay, float time)
    {
        DOVirtual.DelayedCall(delay, () => {
            spriteRendererBlurEffect.gameObject.SetActive(true);
            spriteRendererBlurEffect.material = new Material(spriteRendererBlurEffect.material);
            spriteRendererBlurEffect.material.SetFloat("_Size", 3);
            spriteRendererBlurEffect.material.DOFloat(0, "_Size", time).OnComplete(()=> { spriteRendererBlurEffect.gameObject.SetActive(false); });
        });
        
        
    }
    public void InitCoord(int w, int h, int l)
    {
        width = w;
        height = h;
    }
    public void InitData(GameManager gameManager, Enums.BlockColor blockColor, Sprite cube, Sprite circle)
    {
        this.gameManager = gameManager;
        IsReady = true;
        this.BlockColor = blockColor;

        sprCube.sprite = cube;
        sprCircle.sprite = circle;

        sprCircle.gameObject.SetActive(false);
        sprCube.transform.localScale = Vector3.one;
        sprCircle.transform.localScale = Vector3.zero;

        meshRenderer.gameObject.SetActive(false);
        meshRendererCircle.gameObject.SetActive(false);
        //meshRenderer.gameObject.transform.localScale = Vector3.one * 140;
        //meshRenderer.material = material;
        //meshRendererCircle.gameObject.SetActive(false);
        //meshRendererCircle.material = material;
        //meshRendererCircle.gameObject.transform.localScale = Vector3.zero;
    }

    public void ShowEffectAfterDrop()
    {
        float bounce = 0.05f;
        float time = 0.1f;
        transformContainer.DOLocalMoveY(-bounce, time / 2).OnComplete(() => {
            transformContainer.DOLocalMoveY(bounce / 2, time).OnComplete(() => {
                transformContainer.DOLocalMoveY(0, time);
            });
        });
        //Debug.LogError("ShowEffectAfterDrop " + gameObject.name);
    }
    public void MoveDrop(float delay, float h, float spacingH, float spacingL)
    {
        //if (gameObject.name == "2 - 1 - 0")
        //{
        //    Debug.LogError("droppppp");
        //}
        DOTween.Kill(transform, true);
        if (moveTween != null) DOTween.Kill(moveTween, true);
        IsReady = false; //kieu ban nen ko can cho` nua
        float time = 0.2f;
        int l = 0;
        float newY = h * spacingH + (spacingL * l);
        moveTween = transform.DOLocalMoveY(newY, time).SetDelay(delay).SetEase(Ease.Linear).OnUpdate(() =>
        {
            float progress = moveTween.ElapsedPercentage();
            if(progress >= 0.4f) IsReady = true;
        }).OnComplete(() =>
        {
            IsReady = true;
            //Debug.LogError("ready to check");
            ShowEffectAfterDrop();
        });
    }
    //private void Update()
    //{
    //    if (gameObject.name == "2 - 6 - 0")
    //    {
    //        Debug.LogError(transform.localPosition.y);
    //    }
    //}
    public void MoveToSnake(TargetBlock left, TargetBlock right, float delay, Slot slot)
    {
        //Debug.LogError("MoveToSnake "+ delay);
        //if (gameObject.name == "2 - 1 - 0")
        //{
        //    Debug.LogError("shake");
        //}
        sprCube.transform.DOScale(0, 0.01f).OnComplete(() =>
        {
            
        });
        DOVirtual.DelayedCall(0.005f, () => {
            sprCircle.gameObject.SetActive(true);
            sprCircle.transform.DOScale(1, 0.01f).OnComplete(() =>
            {
                if (left != null) left.DoExplosion(rigidbody, Vector3.forward);
                if (right != null) right.DoExplosion(rigidbody, Vector3.back);
                float time = 0.2f;
                DOTween.Kill(transformContainer, true);
                DOTween.Kill(transform, true);
                if (moveTween != null) DOTween.Kill(moveTween);
                //Debug.LogError("MoveToSnake " + gameObject.name);
                transform.DOScale(0.4f, time).SetDelay(delay);

                slot.Snake.Look(transform);
                transform.DOMove(slot.Snake.GetTargetMove().position, time).SetDelay(delay).OnComplete(() => {

                    showFxCollect();
                    HapticHelper.DeviceVibrate();
                    SoundController.Instance.PlaySoundEffectOneShot("SFX_Letter_Press_Soft_Bubble_Tier_05");
                    //transform.gameObject.SetActive(false);
                    this.ReturnToPool();
                    slot.Snake.DoShakeScale(1.08f);
                    slot.Snake.CollectView();
                    slot.ShowRemain();
                    if (slot.Snake.GetRemainView() == 0)
                    {
                        Snake snake = slot.Snake;
                        GameManager.Instance.AddButterToQueue(snake, slot);

                        //snake.DoDisAppear(slot.transform.position, () => {
                        //    GameManager.Instance.AddButterToQueue(snake.SnakeData.SelectedColor, slot);
                        //});
                        //slot.ClearSnake();
                        //slot.ShowEffectComplete();

                    }
                });
            });
        });
    }

    public Rigidbody Rigidbody => rigidbody;
    private void showFxCollect()
    {
        FxCollectItem fxCollectItem = PoolManager.Instance.GetFxCollectItem();
        Vector3 pos = transform.position;
        //pos.z = -1f;
        fxCollectItem.transform.position = pos;
        ParticleSystem[] pss = fxCollectItem.transform.GetComponentsInChildren<ParticleSystem>();
        Color color = GameDataSO.Instance.GetDataMaterial(BlockColor).MainColor;
        foreach (var item in pss)
        {
            var mainSpark = item.main;
            mainSpark.startColor = color;
        }
        DOVirtual.DelayedCall(1, () =>
        {
            fxCollectItem.ReturnToPool();
        });
    }
    public void DoExplosion(Rigidbody middleRb, Vector3 rotationForce)
    {
        if (!IsReady) return; 
        if (hasBeenPushed) return;
        if (DOTween.IsTweening(transform)) return;
        if (moveTween != null && DOTween.IsTweening(moveTween)) return;
        if (gameObject.name == "2 - 1 - 0")
        {
            Debug.LogError("DoExplosion");
            //return;
        }
        float explosionForce = 1;
        float explosionRadius = 5;
        initialPosition = rigidbody.transform.position;
        initialRotation = rigidbody.transform.rotation;

        hasBeenPushed = true;
        Vector3 horizontalDirection = (transform.position - middleRb.transform.position).normalized;
        Vector3 finalExplosionDirection = (horizontalDirection + explosionDirectionOffset).normalized;

        rigidbody.AddExplosionForce(explosionForce, middleRb.transform.position, explosionRadius, 0f, ForceMode.Impulse);
        rigidbody.AddForce(finalExplosionDirection * explosionForce, ForceMode.Impulse);
        rigidbody.AddTorque(rotationForce, ForceMode.Impulse);

        Invoke("ReturnToInitialState", 0.1f);
    }
    Vector3 explosionDirectionOffset = Vector3.up;

    public TargetBlock left;
    public TargetBlock right;
    public void Test()
    {
        left.DoExplosion(rigidbody, Vector3.right);
        right.DoExplosion(rigidbody, Vector3.back);
    }

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool hasBeenPushed = false;
    void ReturnToInitialState()
    {
        float returnSpeed = 50;
        if (rigidbody != null)
        {
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.angularVelocity = Vector3.zero;
            rigidbody.isKinematic = true;

            transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * returnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * returnSpeed);

            if (Vector3.Distance(transform.position, initialPosition) < 0.01f && Quaternion.Angle(transform.rotation, initialRotation) < 1f)
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
                rigidbody.isKinematic = false;
                hasBeenPushed = false;
                CancelInvoke("ReturnToInitialState");
            }
            else
            {
                Invoke("ReturnToInitialState", Time.deltaTime);
            }
        }
        
    }
    //private void OnDestroy()
    //{
    //    Debug.LogError("OnDestroy "+gameObject.name);
    //}
#if UNITY_EDITOR
    void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.fontStyle = FontStyle.BoldAndItalic;
        gUIStyle.fontSize = 40;
        gUIStyle.alignment = TextAnchor.MiddleCenter;
        gUIStyle.normal.textColor = Color.white;
        string content = $"{width}-{height}";
        Handles.Label(transform.position, content, gUIStyle);
    }
    [CustomEditor(typeof(TargetBlock))]
    public class TargetBlockEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Move AHead"))
            {
                var component = (TargetBlock)target;
                component.Test();
            }
        }
    }
#endif
    //void OnBecameVisible()
    //{
    //    meshRenderer.enabled = true;
    //}

    //void OnBecameInvisible()
    //{
    //    meshRenderer.enabled = false;
    //}
}
