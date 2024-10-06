using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunOnAnimationDone : MonoBehaviour
{
    public Action WhenDone;

    public void SetWhenDone(Action done)
    {
        WhenDone = done;
    }

    public void OnAnimationDone()
    {
        WhenDone?.Invoke();
    }
}
