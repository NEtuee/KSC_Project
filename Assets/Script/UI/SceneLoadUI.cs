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
   

    private void Start()
    {
        loadCanvas.enabled = true;
        tipCanvas.enabled = false;
        loadingCanvas.enabled = false;
        fadeImage.color = Color.black;
        tipText.DOFade(0f, 0.001f);
        loadingSlider.fillAmount = 0.0f;
    }

    public void StartLoad(TweenCallback complte)
    {
        loadCanvas.enabled = true;
        fadeImage.DOFade(1f, 0.5f).OnComplete(()=>
        {
            loadingCanvas.enabled = true;
            tipCanvas.enabled = true;
            tipText.DOFade(1f, 0.5f);
            complte.Invoke();
        });
    }

    public void SetLoadingValue(float value)
    {
        loadingSlider.fillAmount = value;
    }

    public void EndLoad()
    {
        loadingCanvas.enabled = false;
        tipCanvas.enabled = false;
        tipText.DOFade(0f, 0.1f);
        loadingSlider.fillAmount = 0.0f;

        fadeImage.DOFade(0f, 2f).OnComplete(() => loadCanvas.enabled = false);
    }

}
