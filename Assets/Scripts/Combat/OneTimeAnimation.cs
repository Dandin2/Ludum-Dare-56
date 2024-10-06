using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeAnimation : MonoBehaviour
{

    public Action OnComplete;

    public void SetCompleteAction(Action whenDone)
    {
        OnComplete = whenDone;
    }


    public void AnimationDone()
    {
        OnComplete?.Invoke();
        Destroy(gameObject);
    }
}
