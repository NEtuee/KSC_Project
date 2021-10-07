using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.Events;
using UniRx.Triggers;
public class GunGageUi : ActiveUi
{
    [SerializeField] private Image frontGageImage;
    [SerializeField] private Image backGageImage;
    [SerializeField]private float _frontTargetUpdateValue;
    [SerializeField]private float _backTargetUpdateValue;
    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                frontGageImage.fillAmount = _frontTargetUpdateValue;
                backGageImage.fillAmount = Mathf.MoveTowards(backGageImage.fillAmount, _backTargetUpdateValue, updateSpeed * Time.deltaTime);
            });
    }

    public void SetBackValue(float backValue)
    {
        _backTargetUpdateValue = backValue / 100;

    }

    public void SetFrontValue(float value)
    {
        _frontTargetUpdateValue = value;
    }
}
