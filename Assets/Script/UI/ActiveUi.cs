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

public class ActiveUi : MonoBehaviour
{
    [SerializeField] private Image fillTargetImage;
    [SerializeField] private Image[] images;
    [SerializeField] protected bool visible = false;
    [SerializeField] protected float updateSpeed = 0.3f;
    protected float _updateValue;
    protected bool _isFade = false;

    public bool Visible { get { return visible; } }

    void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                fillTargetImage.fillAmount = Mathf.MoveTowards(fillTargetImage.fillAmount, _updateValue, updateSpeed * Time.deltaTime);   
            });
    }

    public void SetValue(float value)
    {
        _updateValue = value / 100;
    }

    protected void Fade(float alpha, float duration, Action whenEnd = null)
    {
        if (_isFade == true)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].DOKill();
                if (i == 0) images[i].DOFade(alpha, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke(); });
                else images[i].DOFade(alpha, duration);
            }
        }
        else
        {
            _isFade = true;
            for (int i = 0; i < images.Length; i++)
            {
                if (i == 0) images[i].DOFade(alpha, duration).OnComplete(() => { _isFade = false; whenEnd?.Invoke(); });
                else images[i].DOFade(alpha, duration);
            }
        }
    }

    public void SetVisible(bool active)
    {
        visible = active;
        if (active)
        {
            Fade(1f, 0.5f);
        }
        else
        {
            Fade(0f, 1.0f);
        }
    }
}
