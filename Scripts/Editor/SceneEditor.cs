using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
# if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class SceneEditor : MonoBehaviour
{
# if UNITY_EDITOR
    [MenuItem("Helper/ClearPref", false, 1)]
    static void clearPref()
    {
        PlayerPrefs.DeleteAll();
    }
    //[MenuItem("Scene/Startup", false, 0)]
    //static void ChangeSceneStartup()
    //{
    //    EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_STARTUP + ".unity");
    //}

    [MenuItem("Scene/Init", false, 0)]
    static void ChangeSceneInit()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_INIT + ".unity");
    }

    [MenuItem("Scene/Home", false, 1)]
    static void ChangeSceneHome()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_HOME + ".unity");
    }

    [MenuItem("Scene/Game", false, 2)]
    static void ChangeSceneGame()
    {
        Debug.LogError(DateTime.Now);
        EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_GAME + ".unity", OpenSceneMode.Single);
        Debug.LogError(DateTime.Now);
    }

    [MenuItem("Scene/GameAdd", false, 2)]
    static void AddSceneGame()
    {
        Debug.LogError(DateTime.Now);
        EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_GAME + ".unity", OpenSceneMode.Additive);
        Debug.LogError(DateTime.Now);
    }

    [MenuItem("Scene/Empty", false, 3)]

    static void ChangeSceneEmpty()
    {
        Debug.LogError(DateTime.Now);
        EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_EMPTY + ".unity");
        Debug.LogError(DateTime.Now);
    }
#endif
#if TOOL_EDITOR
    [MenuItem("Scene/Tool", false, 4)]
    static void AddSceneTool()
    {
        EditorSceneManager.OpenScene("Assets/ToolMap/Scenes/" + "ToolMap" + ".unity");
    }
#endif
}
