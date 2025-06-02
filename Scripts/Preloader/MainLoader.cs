using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoader : MonoBehaviour
{
    public System.Action OnComplete;
    [SerializeField] private Animator animator = null;
    public void RunClose(System.Action callback)
    {
        OnComplete = callback;
        animator.Play("MainLoader_Close");
    }
    public void AnimCloseComplete()
    {
        OnComplete?.Invoke();
        Debug.LogError("AnimCloseComplete");
    }
}
