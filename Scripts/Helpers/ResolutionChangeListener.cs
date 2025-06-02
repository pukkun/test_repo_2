using System;
using UnityEngine;

public class ResolutionChangeListener:Singleton<ResolutionChangeListener>
{
    public static Action OnResolutionChange;
    private int lastWidth;
    private int lastHeight;

    private void StopListening()
    {
        Application.onBeforeRender -= CheckResolution; 
        Application.quitting -= StopListening; 
    }
    public void Initialize()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        Application.quitting += StopListening;
        Application.onBeforeRender += CheckResolution;
    }
    private void CheckResolution()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            OnResolutionChange?.Invoke(); 
        }
    }
}
