using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseDummyMenu : EscMenu
{
    public TextMeshProUGUI pauseText;
    public ButtonHUD exitButton;

    private void Start()
    {
        pauseText.DOFade(0, 0.01f);
    }

    public override void Appear(float duration)
    {
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        pauseText.DOFade(1f, duration);
        exitButton.Appear(duration, tweenCallback);
    }

    public override void Disappear(float duration)
    {

    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        pauseText.DOFade(0f, duration);
        exitButton.Disappear(duration, tweenCallback);
    }
}
