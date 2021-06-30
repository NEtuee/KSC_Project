using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : EscMenu
{
    public Canvas canvas;

    public List<ImageBaseButton> imageBaseButtons = new List<ImageBaseButton>();

    public override void Init()
    {
        foreach(var button in imageBaseButtons)
        {
            button.Active(false);
        }
        canvas.enabled = false;
    }


    public override void Active(bool active)
    { 
        canvas.enabled = active;
        foreach (var button in imageBaseButtons)
        {
            button.Active(active);
            button.Select(false);
        }
    }

    public override void Appear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

}
