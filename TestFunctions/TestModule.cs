using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TestModule : Singleton<TestModule>
{
    [SerializeField] Text txtFPS = null;
    private int[] frameRateSamples;
    private int averageFromAmount = 30;
    private int averageCounter = 0;
    private int currentAveraged;
    // Start is called before the first frame update
    void Start()
    {
        frameRateSamples = new int[averageFromAmount];
    }

    // Update is called once per frame
    void Update()
    {
        var currentFrame = 0; 
        if(Time.smoothDeltaTime != 0) currentFrame = (int)Math.Round(1f / Time.smoothDeltaTime);
        frameRateSamples[averageCounter] = currentFrame;

        var average = 0f;
        foreach (var frameRate in frameRateSamples)
        {
            average += frameRate;
        }
        currentAveraged = (int)Math.Round(average / averageFromAmount);
        averageCounter = (averageCounter + 1) % averageFromAmount;
        if(txtFPS != null) txtFPS.text = Mathf.Ceil(currentAveraged).ToString();

        if(Input.GetKey(KeyCode.Alpha1))
        {
            
        }
    }
}
