using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RespawnFadeCtrl : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fadeImage;
    public float fadeInDuration;
    public float fadeOutDuration;
    public float blackOutDuration;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        fadeImage.DOFade(0.0f, 0.0f);
        canvas.enabled = false;
    }

    public void FadeInOut(Action fadeOutActon = null)
    {
        fadeImage.DOFade(0.0f, 0.0f);
        canvas.enabled = true;
        fadeImage.DOFade(1f, fadeInDuration).OnComplete(() =>
        {
            fadeImage.DOFade(0.0f, fadeOutDuration).SetDelay(blackOutDuration)
                .OnStart(()=>fadeOutActon?.Invoke())
                .OnComplete(()=>canvas.enabled=false);
        });
        
    }
}
