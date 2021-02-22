using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EscMenu : MonoBehaviour
{
    public abstract void Appear(float duration);

    public abstract void Appear(float duration, TweenCallback tweenCallback);

    public abstract void Disappear(float duration);

    public abstract void Disappear(float duration, TweenCallback tweenCallback);

}
