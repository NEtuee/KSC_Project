using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : EscMenu
{
    private Canvas _canvas;
    void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    void Update()
    {

    }

    public override void Active(bool active)
    {
        if(_canvas != null)
        _canvas.enabled = active;
    }

    public override void Appear(float duration)
    {
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
    }

    public override void Disappear(float duration)
    {
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }
}
