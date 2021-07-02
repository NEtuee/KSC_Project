using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseAppearButton : Button, Appearable
{
    protected override void Start()
    {
        base.Start();
    }

    public virtual void Appear()
    {
    }

    public virtual void Disappear()
    {
    }

    public virtual void Init()
    {
    }
}
