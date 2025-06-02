using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LocalizedKey))]
public class Text_ML : UnityEngine.UI.Text
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("CONTEXT/Text/Convert to ML_Text")]
    public static void ReplaceText(UnityEditor.MenuCommand menuCommand)
    {
        var target = menuCommand.context as UnityEngine.UI.Text;
        string _text = target.text;
        FontStyle _fontStyle = target.fontStyle;
        Font _font = target.font;
        int _fontSize = target.fontSize;
        float _lineSpacing = target.lineSpacing;
        bool _richText = target.supportRichText;

        TextAnchor _Alignment = target.alignment;
        bool _AlignByGeomatry = target.alignByGeometry;
        // target.horizontalOverflow = HorizontalWrapMode.Overflow;
        // target.verticalOverflow = VerticalWrapMode.Overflow;
        HorizontalWrapMode _horizontalWrapMode = target.horizontalOverflow;
        VerticalWrapMode _verticalWrapMode = target.verticalOverflow;
        bool _bestFit = target.resizeTextForBestFit;
        int _resizeMinSize = target.resizeTextMinSize;
        int _resizeMaxSize = target.resizeTextMaxSize;
        Color _color = target.color;
        Material _material = target.material;
        bool _RaycastTarget = target.raycastTarget;

        var g = target.gameObject;
        DestroyImmediate(target);
        Text_ML ml = g.AddComponent(typeof(Text_ML)) as Text_ML;
        ml.text = _text;
        ml.font = _font;
        ml.fontStyle = _fontStyle;
        ml.fontSize = _fontSize;
        ml.lineSpacing = _lineSpacing;
        ml.supportRichText = _richText;
        ml.alignment = _Alignment;
        ml.alignByGeometry = _AlignByGeomatry;
        ml.horizontalOverflow = _horizontalWrapMode;
        ml.verticalOverflow = _verticalWrapMode;
        ml.resizeTextForBestFit = _bestFit;
        ml.resizeTextMinSize = _resizeMinSize;
        ml.resizeTextMaxSize = _resizeMaxSize;
        ml.color = _color;
        ml.material = _material;
        ml.raycastTarget = _RaycastTarget;
    }

    [UnityEditor.MenuItem("CONTEXT/Text/Conver to Text")]
    public static void ReplaceMLText(UnityEditor.MenuCommand menuCommand)
    {
        var target = menuCommand.context as Text_ML;
        string _text = target.text;
        FontStyle _fontStyle = target.fontStyle;
        Font _font = target.font;
        int _fontSize = target.fontSize;
        float _lineSpacing = target.lineSpacing;
        bool _richText = target.supportRichText;

        TextAnchor _Alignment = target.alignment;
        bool _AlignByGeomatry = target.alignByGeometry;
        HorizontalWrapMode _horizontalWrapMode = target.horizontalOverflow;
        VerticalWrapMode _verticalWrapMode = target.verticalOverflow;
        bool _bestFit = target.resizeTextForBestFit;
        int _resizeMinSize = target.resizeTextMinSize;
        int _resizeMaxSize = target.resizeTextMaxSize;
        Color _color = target.color;
        Material _material = target.material;
        bool _RaycastTarget = target.raycastTarget;

        var g = target.gameObject;
        DestroyImmediate(target);
        UnityEngine.UI.Text ml = g.AddComponent(typeof(UnityEngine.UI.Text)) as UnityEngine.UI.Text;
        ml.text = _text;
        ml.font = _font;
        ml.fontStyle = _fontStyle;
        ml.fontSize = _fontSize;
        ml.lineSpacing = _lineSpacing;
        ml.supportRichText = _richText;
        ml.alignment = _Alignment;
        ml.alignByGeometry = _AlignByGeomatry;
        ml.horizontalOverflow = _horizontalWrapMode;
        ml.verticalOverflow = _verticalWrapMode;
        ml.resizeTextForBestFit = _bestFit;
        ml.resizeTextMinSize = _resizeMinSize;
        ml.resizeTextMaxSize = _resizeMaxSize;
        ml.color = _color;
        ml.material = _material;
        ml.raycastTarget = _RaycastTarget;
    }

    [UnityEditor.MenuItem("GameObject/Multi-Language/Text", false, 0)]
    public static void CreateMLText()
    {
        if (UnityEditor.Selection.activeGameObject)
        {
            CreateInstanceIn(UnityEditor.Selection.activeTransform);
        }
        else
        {
            var can = FindObjectOfType<Canvas>();
            if (can)
            {
                CreateInstanceIn(can.transform);
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("No canvas found!", "Please create a canvas before adding!", "OK");
            }

        }
    }
    static void CreateInstanceIn(Transform parent)
    {
        GameObject g = new GameObject("Text-ML", typeof(RectTransform), typeof(Text_ML));
        g.transform.SetParent(parent);
        RectTransform rect = g.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector3.zero;
        rect.localPosition = Vector3.zero;
        UnityEditor.Selection.SetActiveObjectWithContext(g, g);
    }
#endif
}
