using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonOnOff : Button
{
    [SerializeField] private Image[] transOn = null;
    [SerializeField] private Transform transCircle = null;
    [SerializeField] private float distanceMotion = 50;
    [SerializeField] private float timeMotion = 1.5f;
    [SerializeField] private float offSet = 0f;
    private bool isOn;
    public bool IsOn
    {
        get { return isOn; }
        set
        {
            isOn = value;
            RefreshUI();
        }
    }

    public delegate void OnChangeDelegate();
    /// <summary>
    /// thuc thi sau khi chay xong motion
    /// </summary>
    public event OnChangeDelegate OnChangeComplete;
    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (interactable)
        {
            isOn = !isOn;
            onClick?.Invoke();
            runEffect();
        }
        base.OnPointerClick(eventData);
    }

    protected override void Start()
    {
        base.Start();
        RefreshUI();
    }
    private void RefreshUI()
    {
        transOn.ForEach(item => {
            item.gameObject.SetActive(isOn);
            item.SetAlpha((isOn) ? 1 : 0);
        });
        //transOn.gameObject.SetActive(isOn);
        //transOn.SetAlpha((isOn) ? 1 : 0);
        Vector3 posCirle = transCircle.localPosition;
        if (isOn) posCirle.x = distanceMotion + offSet;
        else posCirle.x = -distanceMotion + offSet;
        transCircle.localPosition = posCirle;
    }

    private void runEffect()
    {
        if (isOn) animationOn();
        else animationOff();
    }

    private void animationOn()
    {
        interactable = false;


        //transOn.gameObject.SetActive(true);
        //transOn.SetAlpha(0);
        //transOn.DOFade(1, 0.2f);
        if (timeMotion > 0)
        {
            this.Wait(timeMotion / 2, () =>
            {
                transOn.ForEach(item =>
                {
                    item.gameObject.SetActive(true);
                    item.SetAlpha(0);
                    item.DOFade(1, 0.2f);
                });
            });
        }
        else
        {
            transOn.ForEach(item => {
                item.gameObject.SetActive(true);
                item.SetAlpha(0);
                item.DOFade(1, 0.2f);
            });
        }
        transCircle.DOLocalMoveX(distanceMotion + offSet, timeMotion).SetEase(Ease.Linear).OnComplete(() => {
            
            // transOn.gameObject.SetActive(true);
            interactable = true;
            OnChangeComplete?.Invoke();
        });
    }

    private void animationOff()
    {
        interactable = false;
        transOn.ForEach(item => {
            item.gameObject.SetActive(true);
            item.SetAlpha(1);
            item.DOFade(0, 0.2f);
        });
        //transOn.gameObject.SetActive(true);
        //transOn.SetAlpha(1);
        //transOn.DOFade(0, 0.2f);
        transCircle.DOLocalMoveX(-distanceMotion + offSet, timeMotion).OnComplete(() => {
            transOn.ForEach(item => { item.gameObject.SetActive(false);});
            interactable = true;
            OnChangeComplete?.Invoke();
        });
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(ButtonOnOff))]
public class ButtonOnOffEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
