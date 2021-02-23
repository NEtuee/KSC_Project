using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnterAboveUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float startRatio = 1.0f;
    [SerializeField] private float selectedRatio = 1.0f;
    [SerializeField] private float upOffset = 0.4f;

    private Vector2 targetSize;
    private Vector2 startSize;
    private Vector2 selectedSize;

    private Vector3 startPos;
    private Vector3 targetPos;

    private Color startColor;
    private Color buttonAlphaColor;
    private Color startTextColor;
    private Color textAlphaColor;

    private void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        targetSize = rectTransform.sizeDelta;
        startSize = targetSize * startRatio;
        selectedSize = targetSize * selectedRatio;

        startColor = buttonImage.color;
        buttonAlphaColor = startColor;
        buttonAlphaColor.a = 0;
        buttonImage.color = buttonAlphaColor;

        if (buttonText != null)
        {
            startTextColor = buttonText.color;
            textAlphaColor = startTextColor;
            textAlphaColor.a = 0;
            buttonText.color = textAlphaColor;
        }

        startPos = rectTransform.localPosition + new Vector3(0.0f,upOffset,0.0f);
        targetPos = rectTransform.localPosition;

        canvas.enabled = false;
    }

    public void Appear(float duration, TweenCallback tweenCallback)
    {
        canvas.enabled = true;

        rectTransform.sizeDelta = startSize;
        rectTransform.localPosition = startPos;

        rectTransform.DOSizeDelta(targetSize, duration).OnComplete(tweenCallback).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(targetPos, duration).SetEase(Ease.OutExpo); ;
        buttonImage.DOFade(startColor.a, duration).SetEase(Ease.OutExpo);
        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration).SetEase(Ease.OutExpo);
        }
    }

    public void Appear(float duration,float delay, TweenCallback tweenCallback)
    {
        canvas.enabled = true;

        rectTransform.sizeDelta = startSize;
        rectTransform.localPosition = startPos;

        rectTransform.DOSizeDelta(targetSize, duration).SetDelay(delay).OnComplete(tweenCallback).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(targetPos, duration).SetDelay(delay).SetEase(Ease.OutExpo); ;
        buttonImage.DOFade(startColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        }
    }

    public void Appear(float duration,float delay)
    {
        canvas.enabled = true;

        rectTransform.sizeDelta = startSize;
        rectTransform.localPosition = startPos;

        rectTransform.DOSizeDelta(targetSize, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(targetPos, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        buttonImage.DOFade(startColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        }
    }


    public void Appear(float duration)
    {
        canvas.enabled = true;

        rectTransform.sizeDelta = startSize;
        rectTransform.localPosition = startPos;

        rectTransform.DOSizeDelta(targetSize, duration).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(targetPos, duration).SetEase(Ease.OutExpo);
        buttonImage.DOFade(startColor.a, duration).SetEase(Ease.OutExpo);
        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration).SetEase(Ease.OutExpo);
        }
    }

    public void Disappear(float duration, TweenCallback tweenCallback)
    {
        rectTransform.DOSizeDelta(startSize, duration).OnComplete(tweenCallback).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(startPos, duration).SetEase(Ease.OutExpo);
        buttonImage.DOFade(buttonAlphaColor.a, duration).SetEase(Ease.OutExpo).OnComplete(() => { canvas.enabled = false; });
        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration).SetEase(Ease.OutExpo);
        }
    }

    public void Disappear(float duration)
    {
        rectTransform.DOSizeDelta(startSize, duration).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(startPos, duration).SetEase(Ease.OutExpo);
        buttonImage.DOFade(buttonAlphaColor.a, duration).SetEase(Ease.OutExpo).OnComplete(() => { canvas.enabled = false; });
        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration).SetEase(Ease.OutExpo);
        }
    }

    public void Disappear(float duration,float delay, TweenCallback tweenCallback)
    {
        rectTransform.DOSizeDelta(startSize, duration).SetDelay(delay).OnComplete(tweenCallback).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(startPos, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        buttonImage.DOFade(buttonAlphaColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo).OnComplete(() => { canvas.enabled = false; });
        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        }
    }

    public void Disappear(float duration,float delay)
    {
        rectTransform.DOSizeDelta(startSize, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        rectTransform.DOLocalMove(startPos, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        buttonImage.DOFade(buttonAlphaColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo).OnComplete(() => { canvas.enabled = false; });
        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration).SetDelay(delay).SetEase(Ease.OutExpo);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOSizeDelta(selectedSize, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOSizeDelta(targetSize, 0.1f);
    }
}
