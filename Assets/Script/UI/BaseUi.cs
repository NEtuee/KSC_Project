using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseUi : MonoBehaviour
{
    private bool _visible;

    public bool Visible { get => _visible; }

    public UnityEvent endAppear;
    public UnityEvent endDisappear;

    public abstract void Initialize();

    public virtual void Appear()
    {
        _visible = true;
    }

    public virtual void Disappear()
    {
        _visible = false;
    }
}
