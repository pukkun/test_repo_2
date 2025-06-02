using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopupAppear : MonoBehaviour
{
    [SerializeField] Transform transformMain = null;

    private void OnEnable()
    {
        transformMain.DOScale(0.95f, 0.1f).OnComplete(() =>
        {
            transformMain.DOScale(1, 0.1f).OnComplete(() =>
            {

            });
        });
    }
    private void OnDisable()
    {
        transformMain.localScale = Vector3.one * 1.1f;
    }
    public void DisAppear(System.Action callback = null)
    {
        transformMain.DOScale(1.2f, 0.1f).OnComplete(() =>
        {
            transformMain.DOScale(0, 0.1f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            });
        });
    }
}
