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
    [SerializeField] protected Image fillTargetImage;
    [SerializeField] protected Image[] images;
    [SerializeField] protected bool visible = false;
    [SerializeField] protected float updateSpeed = 0.3f;
    [SerializeField] protected float remainingVisibleTime = 5.0f;
    protected float _updateValue;
    [SerializeField] protected float _currentVisibleTime = 0.0f;
    protected bool _isFade = false;
    
    protected void Start()
    {
        if(visible == false)
        {
            visible = true;
            _currentVisibleTime = remainingVisibleTime;
        }

        this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                if(fillTargetImage != null)
                fillTargetImage.fillAmount = Mathf.MoveTowards(fillTargetImage.fillAmount, _updateValue, updateSpeed * Time.deltaTime);

                if (visible == false)
                    return;

                _currentVisibleTime -= Time.deltaTime;
                if (_currentVisibleTime < 0.0f) _currentVisibleTime = 0.0f;

                if (_currentVisibleTime <= 0.0f)
                {
                    visible = false;
                    Fade(0.0f,1.0f);
                }
            });
    }

    protected virtual void Fade(float alpha, float duration, Action whenEnd = null)
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

    public virtual void SetValue(float value, bool setVisible = true)
    {
        _updateValue = value / 100;


        if (setVisible == false)
            return;


        _currentVisibleTime = remainingVisibleTime;

        if (visible == false)
        {
            visible = true;
             Fade(1f, 0.5f);
        }
   

        //if (visible)
        //{
        //    _currentVisibleTime = remainingVisibleTime;
        //    Debug.Log(_currentVisibleTime);
        //}
        //else
        //{
        //    visible = true;
        //    Fade(1f, 0.5f);
        //}
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
