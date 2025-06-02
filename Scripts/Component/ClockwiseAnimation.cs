using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClockwiseAnimation : MonoBehaviour
{
    Vector3 rotate1 = new Vector3(0, 0, -90);
    Vector3 rotate2 = new Vector3(0, 0, -180);
    Vector3 rotate3 = new Vector3(0, 0, -270);
    Vector3 rotate4 = new Vector3(0, 0, 0);
    Sequence sequence;
    private void OnEnable()
    {
        //playAnim();
    }
    public void Play()
    {
        playAnim();
    }
    public void Resume()
    {
        if (sequence != null)
        {
            sequence.Play();
        }
    }
    public void Pause()
    {
        if (sequence != null)
        {
            sequence.Pause();
        }
    }
    void playAnim() 
    {
        if (sequence == null) sequence = DOTween.Sequence();
        else 
        {
            sequence.Kill(true);
            sequence = DOTween.Sequence();
        }
        transform.eulerAngles = Vector3.zero;
        float timeRotate = 0.2f;
        float timeDelay = 1;
        sequence.Append(transform.DOLocalRotate(rotate1, timeRotate).SetEase(Ease.OutBack).SetDelay(timeDelay));
        sequence.Append(transform.DOLocalRotate(rotate2, timeRotate).SetEase(Ease.OutBack).SetDelay(timeDelay));
        sequence.Append(transform.DOLocalRotate(rotate3, timeRotate).SetEase(Ease.OutBack).SetDelay(timeDelay));
        sequence.Append(transform.DOLocalRotate(rotate4, timeRotate).SetEase(Ease.OutBack).SetDelay(timeDelay));
        sequence.SetLoops(int.MaxValue);
        sequence.Play();
    }
    private void OnDisable()
    {
        Pause();
    }
}
