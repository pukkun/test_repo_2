using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerHelper : MonoBehaviour
{
    float defaultScaleFactor;
    private Vector2 lastResolution;
    [SerializeField] bool useCustomValueForSafeArea = false;//dung cho truong hop tai tho
    [SerializeField] float customValue;
    IEnumerator Start()
    {
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        defaultScaleFactor = canvasScaler.matchWidthOrHeight;
        setMatchWidthOrHeight();
        ResolutionChangeListener.OnResolutionChange += setMatchWidthOrHeight;
        yield return null;
    }
    private void OnDestroy()
    {
        ResolutionChangeListener.OnResolutionChange -= setMatchWidthOrHeight;
    }
    private void setMatchWidthOrHeight()
    {
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        float ratio = (float)Screen.height / Screen.width;
        Rect safe = Screen.safeArea;
        
        if (ratio >= 2.4f)// resolution 23:9
        {
            canvasScaler.matchWidthOrHeight = defaultScaleFactor * 0.8f;
        }
        else if (ratio == 1f)
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
        else if (safe.height < Screen.height && useCustomValueForSafeArea)
        {
            canvasScaler.matchWidthOrHeight = customValue;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = defaultScaleFactor;
        }
        lastResolution = new Vector2(Screen.width, Screen.height);
    }
    public static bool IsCutScreen()
    {
        return Screen.safeArea.height < Screen.height;
    }
}
