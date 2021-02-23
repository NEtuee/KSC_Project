using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenu : EscMenu
{
    public EnterAboveSlider mainSlider;
    public EnterAboveSlider vfxSlider;
    public EnterAboveSlider bgmSlider;

    public override void Appear(float duration)
    {
        mainSlider.Appear(duration);
        vfxSlider.Appear(duration);
        bgmSlider.Appear(duration);
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        mainSlider.Appear(duration);
        vfxSlider.Appear(duration);
        bgmSlider.Appear(duration,tweenCallback);
    }

    public override void Disappear(float duration)
    {
        mainSlider.Disappear(duration);
        vfxSlider.Disappear(duration);
        bgmSlider.Disappear(duration);
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        mainSlider.Disappear(duration);
        vfxSlider.Disappear(duration);
        bgmSlider.Disappear(duration,tweenCallback);
    }
}
