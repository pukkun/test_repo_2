using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts.Editor;
using UnityEngine;

public class LocalizeEditor : MonoBehaviour
{
    #if UNITY_EDITOR
    //[UnityEditor.CustomEditor(typeof(LocalizedKey))]
    //public class LocalizedTextEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        DrawDefaultInspector();

    //        if (GUILayout.Button("Localization Editor"))
    //        {
    //            var component = (LocalizedKey) target;
    //            LocalizationEditorWindow.OpenWithFilter(component.GetKey);
    //        }
    //    }
    //}
    #endif
}
