using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    public System.Action HandleClick;
    public System.Action<PointerEventData> HandleDown;
    [SerializeField]
    private float oldScale;
    private bool isPointerDown;
    public bool AllowPointer = true;
    private static long lastTime;

    public bool EffIdle = false;
    [SerializeField] TypeIdle typeIdle = TypeIdle.ScaleContinuous;
    [Range(0, 10)]
    [SerializeField] float timeSpace = 4.0f;

    public enum TypeIdle
    {
        ScaleSometime,
        ScaleContinuous
    }
    public void SetTypeIdle(TypeIdle typeIdle) 
    {
        this.typeIdle = typeIdle;
    }

    Sequence sequenceIdle = null;
    private void Awake()
    {
        oldScale = transform.localScale.x;
        isPointerDown = false;
    }


    public void RunIdleScale()
    {

        stopIdleScale();
        if (!AllowPointer) return;
        if (!EffIdle || !gameObject.activeInHierarchy) return;
        sequenceIdle = DOTween.Sequence();

        if (typeIdle == TypeIdle.ScaleContinuous)
        {
            sequenceIdle.Append(transform.DOScaleX(oldScale * 1.05f, 0.5f).SetEase(Ease.OutQuad));
            sequenceIdle.Join(transform.DOScaleY(oldScale * 1.05f, 0.5f).SetEase(Ease.InCubic));
            sequenceIdle.Append(transform.DOScaleX(oldScale * 1f, 0.5f).SetEase(Ease.OutQuad));
            sequenceIdle.Join(transform.DOScaleY(oldScale * 1f, 0.5f).SetEase(Ease.InCubic));
            sequenceIdle.AppendCallback(RunIdleScale);
        } else
        {
            sequenceIdle.SetDelay(timeSpace);
            sequenceIdle.Append(transform.DOScale(oldScale * 0.9f, 0.25f));
            sequenceIdle.Append(transform.DOScale(oldScale * 1.05f, 0.125f));
            sequenceIdle.Append(transform.DOScale(oldScale * 1.0f, 0.2f));
            sequenceIdle.AppendCallback(RunIdleScale);
        }
    } 
    public void stopIdleScale()
    {
        if (sequenceIdle != null) {
            sequenceIdle.Kill();
            sequenceIdle = null;
        }
    }


    public void RunEffectPointDown(float scale = 0.9f, float time = 0.1f)
    {
        if (!AllowPointer) return;
        stopIdleScale();
        transform.DOScale(oldScale * scale, time);
    }

    public void RunEffectPointUp(float time = 0.1f)
    {
        if (!AllowPointer) return;
        transform.DOScale(oldScale, time).OnComplete(RunIdleScale);

    }

    public void RunEffectPointClick(float scale = 0.9f, float time = 0.1f)
    {
        if (!AllowPointer) return;
     //   stopIdleScale();
        transform.DOScale(oldScale * scale, time).SetEase(Ease.OutExpo).OnComplete(() => {
            transform.DOScale(oldScale, time);
        });
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (!AllowPointer) return;
        long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (lastTime + 10 > now) return;
        lastTime = now;
        RunEffectPointUp();
        HandleClick?.Invoke();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (!AllowPointer) return;
        isPointerDown = false;
        RunEffectPointUp();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!AllowPointer)
        {
            HandleDown?.Invoke(eventData);
            return;
        }
        isPointerDown = true;
        RunEffectPointDown();
        HandleDown?.Invoke(eventData);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (isPointerDown) transform.DOScale(oldScale, 0.1f).SetEase(Ease.OutBounce);

    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (isPointerDown) RunEffectPointDown();

    }

    public void SetOldScale(float scale)
    {
        oldScale = scale;
    }

    private void OnDisable()
    {
        isPointerDown = false;
        stopIdleScale();
    }
    private void OnEnable()
    {
        transform.localScale = Vector3.one * oldScale;
        RunIdleScale();
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    public class UIEditor : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Button With Scale", false, 0)]
        public static void CreateButton()
        {
            if (UnityEditor.Selection.activeGameObject)
            {
                createInstanceIn(UnityEditor.Selection.activeTransform);
            }
            else
            {
                var can = FindObjectOfType<Canvas>();
                if (can)
                {
                    createInstanceIn(can.transform);
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("No canvas found!", "Please create a canvas before adding!", "OK");
                }

            }
        }
        static void createInstanceIn(Transform parent)
        {
            GameObject g = ObjectFactory.CreateGameObject("Button", typeof(RectTransform), typeof(ButtonScale));
            g.transform.SetParent(parent);
            g.transform.localPosition = Vector3.zero;
            g.GetComponent<RectTransform>().localScale = Vector3.one;
            g.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            g.AddComponent<UnityEngine.UI.Image>();
            g.AddComponent<UnityEngine.UI.Button>();
            UnityEditor.Selection.SetActiveObjectWithContext(g, g);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = g;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
#endif
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(ButtonScale))]
    public class ButtonScaleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ButtonScale buttonScale = (ButtonScale)target;
            buttonScale.EffIdle = EditorGUILayout.Toggle("EffIdle", buttonScale.EffIdle);

            if (buttonScale.EffIdle)
            {
                buttonScale.typeIdle = (ButtonScale.TypeIdle)EditorGUILayout.EnumPopup("Type Idle", buttonScale.typeIdle);

                if (buttonScale.typeIdle == ButtonScale.TypeIdle.ScaleSometime)
                {
                    buttonScale.timeSpace = EditorGUILayout.Slider("Time Space", buttonScale.timeSpace, 0, 10);
                }
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
#endif
}

