using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationTrigger : MonoBehaviour
{
    public UnityEvent _OnTrigger;
    public void OnTrigger()
    {
        _OnTrigger?.Invoke();
    }
}
