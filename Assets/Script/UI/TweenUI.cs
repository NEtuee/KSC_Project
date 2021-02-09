
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class TweenUI : MonoBehaviour
{
    public abstract void Appear(float duration);
    public abstract void Appear(float duration, TweenCallback tweenCallback);

    public abstract void Disapper(float duration);

    public abstract void Disapper(float duration, TweenCallback tweenCallback);
   
}
