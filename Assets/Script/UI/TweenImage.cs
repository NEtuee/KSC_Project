
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenImage : TweenUI
{
    private RectTransform rectTransform;
    private Image image;
    [SerializeField] [Range(1.0f, 2.0f)] private float startScale = 1.2f;

    private float startAlpha;

    private Vector2 targetSize;
    private Vector2 disapperSize;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        startAlpha = image.color.a;

        targetSize = rectTransform.sizeDelta;
        disapperSize = targetSize * startScale;

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        rectTransform.sizeDelta = disapperSize;
    }

    public override void Appear(float duration)
    {
        rectTransform.DOSizeDelta(targetSize, duration);
        image.DOFade(startAlpha, duration);
    }

    public override void Appear(float duration,TweenCallback tweenCallback)
    {
        rectTransform.DOSizeDelta(targetSize, duration).OnComplete(tweenCallback);
        image.DOFade(startAlpha, duration);
    }

    public override void Disapper(float duration)
    {
        rectTransform.DOSizeDelta(disapperSize, duration);
        image.DOFade(0, duration);
    }

    public override void Disapper(float duration, TweenCallback tweenCallback)
    {
        rectTransform.DOSizeDelta(disapperSize, duration).OnComplete(tweenCallback);
        image.DOFade(0, duration);
    }
}
