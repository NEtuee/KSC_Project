using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.Events;
using UniRx.Triggers;
using TMPro;

public class FadeUI : MonoBehaviour
{
    [SerializeField] private Image fillTargetImage;
    [SerializeField] private Image[] images;
    [SerializeField] private bool visible = false;
    [SerializeField] private float updateSpeed = 0.3f;
    [SerializeField] private float remainingVisibleTime = 5.0f;
    private float _updateValue;
    private float _currentVisibleTime = 0.0f;
    private bool _isFade = false;
    
    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                fillTargetImage.fillAmount = Mathf.MoveTowards(fillTargetImage.fillAmount, _updateValue, updateSpeed * Time.deltaTime);
                _currentVisibleTime -= Time.deltaTime;
                if (_currentVisibleTime <= 0.0f) _currentVisibleTime = 0.0f;

                if (visible == false)
                    return;

                if (_currentVisibleTime <= 0.0f)
                {
                    visible = false;
                    Fade(0.0f,1.0f);
                }
            });
    }

    private void Fade(float alpha, float duration, Action whenEnd = null)
    {
        if (_isFade == true)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].DOKill();
                if (i == 0) images[i].DOFade(alpha, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke();});
                else images[i].DOFade(alpha, duration);
            }
        }
        else
        {
            _isFade = true;
            for (int i = 0; i < images.Length; i++)
            {
                if (i == 0) images[i].DOFade(alpha, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke();});
                else images[i].DOFade(alpha, duration);
            }   
        }
    }

    public void SetValue(float value)
    {
        _updateValue = value / 100;
        if (visible)
            _currentVisibleTime = remainingVisibleTime;
        else
        {
            visible = true;
            Fade(1f,0.5f);
        }
    }

    public void SetVisible(bool active)
    {
        if (active)
        {
            _currentVisibleTime = remainingVisibleTime;
            if (visible == false)
            {
                visible = true;
                Fade(1f, 0.5f);
            }
        }
        else
        {
            visible = false;
            Fade(0f,1.0f);
        }
    }
}
