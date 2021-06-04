using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMainMenu : EscMenu
{
    public ButtonHUD inputMenuButton;
    public ButtonHUD soundMenuButton;
    public ButtonHUD titleButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Appear(float duration)
    {
        inputMenuButton.Appear(duration, () => soundMenuButton.Appear(duration, () => titleButton.Appear(duration)));
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        inputMenuButton.Appear(duration, () => soundMenuButton.Appear(duration, () => titleButton.Appear(duration, tweenCallback)));
    }

    public override void Disappear(float duration)
    {
        titleButton.Disappear(duration, () => soundMenuButton.Disappear(duration, () => inputMenuButton.Disappear(duration)));
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        titleButton.Disappear(duration, () => soundMenuButton.Disappear(duration, () => inputMenuButton.Disappear(duration, tweenCallback)));
    }

    public override void Active(bool active)
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }
}
