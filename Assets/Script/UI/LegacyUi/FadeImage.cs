using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class FadeImage : MonoBehaviour
{
    [SerializeField] private Image image;
    public float beforeWaitTime;
    public float fadeDuration;
    public UnityEvent whenEndFade;
    void Awake()
    {
        image = GetComponent<Image>();
        image.DOFade(0.0f, 0.0f);
    }

    private void Start()
    {
        StartCoroutine(FadeStart());
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(beforeWaitTime);
        image.DOFade(1.0f,fadeDuration).OnComplete(()=>whenEndFade?.Invoke());
    }
}
