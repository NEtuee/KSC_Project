using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SceneLoadUI : MonoBehaviour
{
    public Canvas loadCanvas;
    public Image fadeImage;
    public Canvas tipCanvas;
    public TextMeshProUGUI tipText;
    public Canvas loadingCanvas;
    public Image loadingSlider;

    private float _loadingTime = 0.0f;
    private float _minLoadUiShowTime = 1f;

    private void Start()
    {
        loadCanvas.enabled = true;
        tipCanvas.enabled = false;
        loadingCanvas.enabled = false;
        fadeImage.color = Color.black;
        tipText.DOFade(0f, 0.001f);
        loadingSlider.fillAmount = 0.0f;
    }

    public void StartLoad(TweenCallback complete)
    {
        loadCanvas.enabled = true;
        _loadingTime = 0.0f;
        fadeImage.DOFade(1f, 0.5f).OnComplete(()=>
        {
            loadingCanvas.enabled = true;
            tipCanvas.enabled = true;
            tipText.DOFade(1f, 0.5f);
            complete?.Invoke();
        });
    }

    public void SetLoadingValue(float value)
    {
        loadingSlider.fillAmount = value;
        _loadingTime += Time.deltaTime;
    }

    public void EndLoad()
    {
        SetLoadingValue(1.0f);

        if (_loadingTime >= _minLoadUiShowTime)
        {
            loadingCanvas.enabled = false;
            tipCanvas.enabled = false;
            tipText.DOFade(0f, 0.1f);
            loadingSlider.fillAmount = 0.0f;

            fadeImage.DOFade(0f, 2f).OnComplete(() => loadCanvas.enabled = false);
        }
        else
        {
            StartCoroutine(DelayLoadUI(_minLoadUiShowTime - _loadingTime));
        }
    }

    IEnumerator DelayLoadUI(float time)
    {
        yield return new WaitForSeconds(time);
        
        SetLoadingValue(1.0f);
        loadingCanvas.enabled = false;
        tipCanvas.enabled = false;
        tipText.DOFade(0f, 0.1f);
        loadingSlider.fillAmount = 0.0f;

        fadeImage.DOFade(0f, 2f).OnComplete(() => loadCanvas.enabled = false);
    }
    

    public void FadeScreen(float fadeTarget, float duration, Action fadeEndAction)
    {
        loadCanvas.enabled = true;
        fadeImage.DOFade(fadeTarget, duration).OnComplete(() => fadeEndAction?.Invoke());
    }
}
