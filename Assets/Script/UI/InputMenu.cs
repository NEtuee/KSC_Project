using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMenu : EscMenu
{
    public EnterAboveUI[] bars = new EnterAboveUI[6];

    public override void Appear(float duration)
    {
        bars[0].Appear(duration, () => bars[1].Appear(duration, ()=>bars[2].Appear(duration, () => bars[3].Appear(duration, () => bars[4].Appear(duration, () => bars[5].Appear(duration))))));
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        bars[0].Appear(duration, () => bars[1].Appear(duration, () => bars[2].Appear(duration, () => bars[3].Appear(duration, () => bars[4].Appear(duration, () => bars[5].Appear(duration,tweenCallback))))));
    }

    public override void Disappear(float duration)
    {
        bars[5].Disappear(duration, () => bars[4].Disappear(duration, () => bars[3].Disappear(duration, () => bars[2].Disappear(duration, () => bars[1].Disappear(duration, () => bars[0].Disappear(duration))))));
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        bars[5].Disappear(duration, () => bars[4].Disappear(duration, () => bars[3].Disappear(duration, () => bars[2].Disappear(duration, () => bars[1].Disappear(duration, () => bars[0].Disappear(duration,tweenCallback))))));
    }
}
