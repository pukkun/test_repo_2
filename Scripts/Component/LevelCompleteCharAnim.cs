using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelCompleteCharAnim : MonoBehaviour
{
    private static Vector2 firstScale = new Vector2(1.2f, 0.8f);
    private static Vector2 secondScale = new Vector2(0.9f, 1.2f);
    [SerializeField] private float delay = 0;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 charPos;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        initState();
    }
    private void OnDisable()
    {
        transform.localPosition = startPos;
        transform.localScale = Vector2.zero;
        DOTween.Kill(transform);
    }
    private void initState()
    {
        transform.localPosition = startPos;
        transform.localScale = Vector2.zero;
        transform.DOLocalMove(charPos, 0.2f).SetDelay(delay);
        transform.DOScale(firstScale, 0.2f).SetDelay(delay).OnComplete(() =>
        {
            transform.DOScale(secondScale, 0.2f).OnComplete(() =>
            {
                transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                {

                });
            });
        });
    }
}
