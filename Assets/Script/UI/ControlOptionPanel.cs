using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ControlOptionPanel : EscMenu
{
    private const float MaxRotateSpeed = 1000f;
    private Canvas _canvas;
    public Slider pitchRotateSpeedSlider;
    public Slider yawRotateSpeedSlider;
    public TextMeshProUGUI pitchRotateSpeedText;
    public TextMeshProUGUI yawRotateSpeedText;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    public override void Active(bool active)
    {
        if (active)
        {
            _canvas.enabled = true;
            pitchRotateSpeedSlider.value = GameManager.Instance.followTarget.PitchRotateSpeed / MaxRotateSpeed;
            yawRotateSpeedSlider.value = GameManager.Instance.followTarget.YawRotateSpeed / MaxRotateSpeed;
            UpdateYawRotateSpeedText();
            UpdatePitchRotateSpeedText();
        }
        else
        {
            GameManager.Instance.followTarget.PitchRotateSpeed = pitchRotateSpeedSlider.value * MaxRotateSpeed;
            GameManager.Instance.followTarget.YawRotateSpeed = yawRotateSpeedSlider.value * MaxRotateSpeed;
            _canvas.enabled = false;
        }
    }

    public void UpdateYawRotateSpeedText()
    {
        yawRotateSpeedText.text = ((int)(yawRotateSpeedSlider.value * MaxRotateSpeed)).ToString();
    }
    
    public void UpdatePitchRotateSpeedText()
    {
        pitchRotateSpeedText.text = ((int)(pitchRotateSpeedSlider.value * MaxRotateSpeed)).ToString();
    }

    public override void Appear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }
    
}
