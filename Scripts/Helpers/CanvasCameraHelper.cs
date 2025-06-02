using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraHelper : MonoBehaviour
{
    [SerializeField] private string specialTagCamera;
    [SerializeField] private TypeCamera specialTypeCamera;

    public enum TypeCamera
    {
        None,
        UICamera,
        MainCamera
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall -= _onValidate;
        EditorApplication.delayCall += _onValidate;
#endif
    }
    private void _onValidate()
    {
        if (!Application.isPlaying) initCamera();
    }
    //private void Awake()
    //{
    //    initCamera();
    //}
    private void OnEnable()
    {
        initCamera();
        SceneManager.sceneLoaded += onSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }
    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneConstant.SCENE_INIT)
        {
            initCamera();
        }
    }
    private void initCamera()
    {
        if (this == null) return;
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        if (specialTypeCamera == TypeCamera.None || specialTypeCamera == TypeCamera.None)
        {
            canvas.worldCamera = Camera.main;
        }
        else
        {
            GameObject goUICamera = GameObject.FindGameObjectWithTag(specialTypeCamera.ToString());
            if (goUICamera != null) canvas.worldCamera = goUICamera.GetComponent<Camera>();
        }
        if (canvas.worldCamera == null)
        {
            // throw new System.Exception(gameObject.name + " need main camera");
        }
    }
}
