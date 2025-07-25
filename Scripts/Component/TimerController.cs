﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    private System.Action handleEnd;
    private System.Action<int> handleTimetick;
    [SerializeField] private int timer = 0;
    public int CurrentTime=> timer;
    private IEnumerator enumerator;
    private System.DateTime timePause;
    private readonly WaitForSecondsRealtime waitReal1Second = new WaitForSecondsRealtime(1);
    private int maxTime;

    public void InitTimer(int remainTime, System.Action<int> timeTick, System.Action end, bool isActive = true)
    {
        maxTime = remainTime;
        handleTimetick = timeTick;
        handleEnd = end;
        timer = remainTime;
        destroyRunner();
        handleTimetick?.Invoke(Mathf.Max(timer, 0));
        enumerator = runTimer();
        if (isActive && gameObject.activeInHierarchy)
        {
            StartCoroutine(enumerator);
        } else
        {
            timePause = System.DateTime.Now;
        }
      
    }

    public void ResetTime()
    {
        timer = maxTime;
    }

    private void destroyRunner()
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
            enumerator = null;
        }
    }
    
    private IEnumerator runTimer()
    {
        //yield return new WaitForSecondsRealtime(1);
        //timer--;
        //if (timer <= 0)
        //{
        //    handleTimetick?.Invoke(0);
        //    handleEnd?.Invoke();
        //}
        //else
        //{
        //    handleTimetick?.Invoke(Mathf.Max(timer, 0));
        //    enumerator = runTimer();
        //    StartCoroutine(enumerator);
        //}
        bool loop = true;
        while (loop)
        {
            yield return waitReal1Second;
            timer--;
            if (timer <= 0)
            {
                loop = false;
                handleTimetick?.Invoke(0);
                handleEnd?.Invoke();
            }
            else
            {
                handleTimetick?.Invoke(Mathf.Max(timer, 0));
            }
        }
    }
    public void DestroyTimer()
    {
        destroyRunner();
        Destroy(gameObject);
    }

    public void PauseTimer()
    {
        if(enumerator != null) StopCoroutine(enumerator);
    }
    public void ResumeTimer()
    {
        if (enumerator != null) StartCoroutine(enumerator);
    }
    private void OnDisable()
    {
        timePause = System.DateTime.Now;
    }
    private void OnEnable()
    {
        double second = (System.DateTime.Now - timePause).TotalSeconds;
        long minute = (System.DateTime.Now - timePause).Minutes;
        double totalMinute = (System.DateTime.Now - timePause).TotalMinutes;
        double totalDay = (System.DateTime.Now - timePause).TotalDays;
        timePause = System.DateTime.Now;
        if (second > 0)
        {
            timer -= (int)second;
            handleTimetick?.Invoke(Mathf.Max(timer, 0));
            destroyRunner();
            enumerator = runTimer();
            StartCoroutine(enumerator);
        }
    }

    public static TimerController CreateTimer(Transform parent)
    {
        GameObject go = new GameObject();
        go.name = "Timer";
        go.transform.SetParent(parent);
        go.transform.localScale = Vector3.one;
        return go.AddComponent<TimerController>();
    }
}
