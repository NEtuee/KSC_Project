using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;


public class HpPackUI : FadeUI
{
    [SerializeField] private Image firstHpPackIcon;
    [SerializeField] private Image secondHpPackIcon;
    [SerializeField] private Image thirdHpPackIcon;

    private float firstAlpha;
    private float secondAlpha;
    private float thirdAlpha;

    private float usedAlpha = 0.3f;

    new void Start()
    {
        if (visible == false)
        {
            visible = true;
            _currentVisibleTime = remainingVisibleTime;
        }

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (visible == false)
                    return;

                _currentVisibleTime -= Time.deltaTime;
                if (_currentVisibleTime < 0.0f) _currentVisibleTime = 0.0f;

                if (_currentVisibleTime <= 0.0f)
                {
                    visible = false;
                    Fade(0.0f, 1.0f);
                }
            });
    }

    protected override void Fade(float alpha, float duration, Action whenEnd = null)
    {
        if (_isFade == true)
        {
            if(alpha == 0.0f)
            {
                firstHpPackIcon.DOKill();
                firstHpPackIcon.DOFade(0.0f, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke(); });
                secondHpPackIcon.DOKill();
                secondHpPackIcon.DOFade(0.0f, duration);
                thirdHpPackIcon.DOKill();
                thirdHpPackIcon.DOFade(0.0f, duration);
                return;
            }

            firstHpPackIcon.DOKill();
            firstHpPackIcon.DOFade(firstAlpha, duration).OnComplete(()=> { _isFade = false; whenEnd?.Invoke(); });
            secondHpPackIcon.DOKill();
            secondHpPackIcon.DOFade(secondAlpha, duration);
            thirdHpPackIcon.DOKill();
            thirdHpPackIcon.DOFade(thirdAlpha, duration);
        }
        else
        {
            _isFade = true;

            if (alpha == 0.0f)
            {
                firstHpPackIcon.DOFade(0.0f, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke(); });
                secondHpPackIcon.DOFade(0.0f, duration);
                thirdHpPackIcon.DOFade(0.0f, duration);
                return;
            }

            firstHpPackIcon.DOFade(firstAlpha, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke(); });
            secondHpPackIcon.DOFade(secondAlpha, duration);
            thirdHpPackIcon.DOFade(thirdAlpha, duration);
        }
    }


    public void SetValue(int hpPackCount, bool setVisible = true)
    {
        switch(hpPackCount)
        {
            case 0:
                {
                    firstAlpha = usedAlpha;
                    secondAlpha = usedAlpha;
                    thirdAlpha = usedAlpha;
                }
                break;
            case 1:
                {
                    firstAlpha = 1.0f;
                    secondAlpha = usedAlpha;
                    thirdAlpha = usedAlpha;
                }
                break;
            case 2:
                {
                    firstAlpha = 1.0f;
                    secondAlpha = 1.0f;
                    thirdAlpha = usedAlpha;
                }
                break;
            case 3:
                {
                    firstAlpha = 1.0f;
                    secondAlpha = 1.0f;
                    thirdAlpha = 1.0f;
                }
                break;
        }

        if (setVisible == false)
            return;

        visible = true;
        _currentVisibleTime = remainingVisibleTime;

        Fade(1.0f, 1.0f);
    }
}
