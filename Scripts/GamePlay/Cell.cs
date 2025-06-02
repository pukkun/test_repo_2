using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    [SerializeField] Transform transformHighlight = null;
    [SerializeField] Transform transformHighlightMove = null;

    Tween tween;
    Tween tweenMove;

    public void ShowHighlight()
    {
        if (tween != null) tween.Kill();
        transformHighlight.localScale = Vector3.one * 0.9f;
        transformHighlight.gameObject.SetActive(true);
        transformHighlight.DOScale(0.97f, 0.2f).OnComplete(() => {

            tween = DOVirtual.DelayedCall(0.7f, () => {
                transformHighlight.gameObject.SetActive(false);
                transformHighlight.DOScale(0, 0.3f).OnComplete(() => {
                    
                });
                
            });
            
        });
    }
    public void ShowHighlightMove()
    {
        if (tweenMove != null) tweenMove.Kill();
        transformHighlightMove.localScale = Vector3.one * 0.9f;
        transformHighlightMove.gameObject.SetActive(true);
        transformHighlightMove.DOScale(0.97f, 0.2f).OnComplete(() => {
            tweenMove = DOVirtual.DelayedCall(0.7f, () => {
                transformHighlightMove.gameObject.SetActive(false);
                transformHighlightMove.DOScale(0, 0.3f).OnComplete(() => {
                    
                });

            });
        });
    }
}
