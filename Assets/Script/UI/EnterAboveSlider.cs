using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnterAboveSlider : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform slidRect;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image handleImage;

    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float startRatio = 1.0f;
    [SerializeField] private float selectedRatio = 1.0f;
    [SerializeField] private float upOffset = 0.4f;

    private Vector2 targetSize;
    private Vector2 startSize;
    private Vector2 selectedSize;

    private Vector3 startPos;
    private Vector3 targetPos;

    private Color backGroundColor;
    private Color backGroundAlpha;

    private Color fillColor;
    private Color fillAlpha;

    private Color handleColor;
    private Color handleAlpha;

    private Color startTextColor;
    private Color textAlphaColor;


    // Start is called before the first frame update
    void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        targetSize = rectTransform.sizeDelta;
        startSize = targetSize * startRatio;
        selectedSize = targetSize * selectedRatio;

        backGroundColor = backGroundImage.color;
        backGroundAlpha = backGroundColor;
        backGroundAlpha.a = 0;

        fillColor = fillImage.color;
        fillAlpha = fillColor;
        fillAlpha.a = 0;

        handleColor = handleImage.color;
        handleAlpha = handleColor;
        handleAlpha.a = 0;

        if (buttonText != null)
        {
            startTextColor = buttonText.color;
            textAlphaColor = startTextColor;
            textAlphaColor.a = 0;
            buttonText.color = textAlphaColor;
        }

        //startPos = rectTransform.localPosition + new Vector3(0.0f, upOffset, 0.0f);
        //targetPos = rectTransform.localPosition;

        canvas.enabled = false;
    }

    public void Appear(float duration, TweenCallback tweenCallback)
    {
        canvas.enabled = true;

        //rectTransform.sizeDelta = startSize;
        //rectTransform.localPosition = startPos;

        //rectTransform.DOSizeDelta(targetSize, duration).OnComplete(tweenCallback);
        //rectTransform.DOLocalMove(targetPos, duration).OnComplete(tweenCallback);
        slidRect.offsetMax = new Vector2(-300f, slidRect.offsetMax.y);
        DOTween.To(() => slidRect.offsetMax, x => slidRect.offsetMax = x, new Vector2(0.0f, slidRect.offsetMax.y), 1f).OnComplete(tweenCallback);

        backGroundImage.DOFade(1f, duration);
        fillImage.DOFade(1f, duration);
        handleImage.DOFade(1f, duration);

        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration);
        }
    }

    public void Appear(float duration)
    {
        canvas.enabled = true;

        //rectTransform.sizeDelta = startSize;
        //rectTransform.localPosition = startPos;

        //rectTransform.DOSizeDelta(targetSize, duration);
        //rectTransform.DOLocalMove(targetPos, duration);
        slidRect.offsetMax = new Vector2(-300f, slidRect.offsetMax.y);
        DOTween.To(() => slidRect.offsetMax, x => slidRect.offsetMax = x, new Vector2(0.0f, slidRect.offsetMax.y), 1f);

        backGroundImage.DOFade(1f, duration);
        fillImage.DOFade(1f, duration);
        handleImage.DOFade(1f, duration);

        if (buttonText != null)
        {
            buttonText.DOFade(startTextColor.a, duration);
        }
    }

    public void Disappear(float duration, TweenCallback tweenCallback)
    {
        //rectTransform.DOSizeDelta(startSize, duration).OnComplete(tweenCallback);
        //rectTransform.DOLocalMove(startPos, duration).OnComplete(tweenCallback);
        //buttonImage.DOFade(buttonAlphaColor.a, duration).OnComplete(() => { canvas.enabled = false; });
        DOTween.To(() => slidRect.offsetMax, x => slidRect.offsetMax = x, new Vector2(-300.0f, slidRect.offsetMax.y), 1f).OnComplete(tweenCallback);
        backGroundImage.DOFade(0f, duration).OnComplete(()=> { canvas.enabled = false; });
        fillImage.DOFade(0f, duration);
        handleImage.DOFade(0f, duration);

        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration);
        }
    }

    public void Disappear(float duration)
    {
        //rectTransform.DOSizeDelta(startSize, duration);
        //rectTransform.DOLocalMove(startPos, duration);
        //buttonImage.DOFade(buttonAlphaColor.a, duration).OnComplete(() => { canvas.enabled = false; });
        DOTween.To(() => slidRect.offsetMax, x => slidRect.offsetMax = x, new Vector2(-300.0f, slidRect.offsetMax.y), 1f);
        backGroundImage.DOFade(0f, duration).OnComplete(() => { canvas.enabled = false; });
        fillImage.DOFade(0f, duration);
        handleImage.DOFade(0f, duration);

        if (buttonText != null)
        {
            buttonText.DOFade(textAlphaColor.a, duration);
        }
    }
}
