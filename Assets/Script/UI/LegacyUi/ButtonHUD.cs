using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonHUD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private bool visible;
    [SerializeField] private Vector2 targetSize;
    [SerializeField] private Vector2 seletedSize;

    [SerializeField] private TweenUI inUi;
    
    [SerializeField]private Vector2 startSize;
    [SerializeField] private CircleGage gage;

    public UnityEvent OnClick;

    private Color startColor;
    private Color buttonAlphaColor;
    private Color startTextColor;
    private Color textAlphaColor;

    void Start()
    {
        if(rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        //startSize = rectTransform.sizeDelta;

        startColor = buttonImage.color;
        buttonAlphaColor = startColor;
        buttonAlphaColor.a = 0;
        buttonImage.color = buttonAlphaColor;
        //textStartSize = buttonText.rectTransform.sizeDelta;

        if (buttonText != null)
        {
            startTextColor = buttonText.color;
            textAlphaColor = startTextColor;
            textAlphaColor.a = 0;
            buttonText.color = textAlphaColor;
        }

        canvas.enabled = false;
    }

    public void Appear(float duration)
    {
        canvas.enabled = true;

        rectTransform.sizeDelta = startSize;

        rectTransform.DOSizeDelta(targetSize, duration);
        buttonImage.DOFade(startColor.a, duration).OnComplete(() => { inUi.Appear(0.5f);gage.Active(); });
        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration);
        }
    }

    public void Appear(float duration, TweenCallback tweenCallback)
    {
        canvas.enabled = true;

        rectTransform.sizeDelta = startSize;

        rectTransform.DOSizeDelta(targetSize, duration).OnComplete(tweenCallback);
        buttonImage.DOFade(startColor.a, duration).OnComplete(() => { inUi.Appear(0.5f); gage.Active(); });
        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration);
        }
    }

    public void Disappear(float duration)
    {
        rectTransform.DOSizeDelta(startSize, duration);
        buttonImage.DOFade(buttonAlphaColor.a, duration).OnComplete(()=> { canvas.enabled = false; });
        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration);
        }

        if(inUi)
        inUi.Disapper(duration);
        if(gage)
        gage.Disapper();
    }

    public void Disappear(float duration, TweenCallback tweenCallback)
    {
        rectTransform.DOSizeDelta(startSize, duration).OnComplete(tweenCallback);
        buttonImage.DOFade(buttonAlphaColor.a, duration).OnComplete(() => { canvas.enabled = false; });
        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration);
        }

        if(inUi)
        inUi.Disapper(duration);
        if(gage)
        gage.Disapper();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOSizeDelta(seletedSize, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOSizeDelta(targetSize, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
