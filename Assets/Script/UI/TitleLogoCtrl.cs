using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class TitleLogoCtrl : MonoBehaviour
{
    public Image logoSImage;
    public Image logoOImage;
    public Image logoImage;

    public float logoOinitXpos;
    private float _logoOTargetXpos;
    private RectTransform _logoORect;
    public float characterAppearDuration;
    public float characterMoveDuration;
    public float titleLogoAppearDuration;

    public UnityEvent whenEndApper;

    void Start()
    {
        
    }

    public void Init()
    {
        _logoORect = logoOImage.GetComponent<RectTransform>();

        logoImage.DOFade(0.0f, 0.0f);
        logoOImage.DOFade(0.0f, 0.0f);
        logoSImage.DOFade(0.0f, 0.0f);

        _logoOTargetXpos = _logoORect.anchoredPosition.x;
        Vector2 originPos = _logoORect.anchoredPosition;
        _logoORect.anchoredPosition = new Vector2(logoOinitXpos, originPos.y);
    }

    public void Appear()
    {
        logoOImage.DOFade(1.0f, characterAppearDuration);
        logoSImage.DOFade(1.0f, characterAppearDuration).OnComplete(() =>
        {
            _logoORect.DOAnchorPosX(_logoOTargetXpos, characterMoveDuration).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                logoImage.DOFade(1.0f, titleLogoAppearDuration).SetDelay(0.5f).OnComplete(()=>
                {
                    whenEndApper?.Invoke();
                });
            });
        });
    }
}
