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

        pitchRotateSpeedSlider.interactable = false;
        yawRotateSpeedSlider.interactable = false;
    }

    public override void Active(bool active)
    {
        if (active)
        {
            _canvas.enabled = true;
            _canvas.sortingOrder = 3;

            ControlSettingData controlSettingData = SaveDataHelper.LoadSetting<ControlSettingData>();
            pitchRotateSpeedSlider.value = controlSettingData.pitchRotateSpeed / MaxRotateSpeed;
            yawRotateSpeedSlider.value = controlSettingData.yawRotateSpeed / MaxRotateSpeed;
            UpdateYawRotateSpeedText();
            UpdatePitchRotateSpeedText();

            pitchRotateSpeedSlider.interactable = true;
            yawRotateSpeedSlider.interactable = true;
        }
        else
        {
            if (GameManager.Instance.followTarget != null)
            {
                GameManager.Instance.followTarget.PitchRotateSpeed = pitchRotateSpeedSlider.value * MaxRotateSpeed;
                GameManager.Instance.followTarget.YawRotateSpeed = yawRotateSpeedSlider.value * MaxRotateSpeed;
            }

            if(SaveDataHelper.streamingAssetsPath == null)
            {
                SaveDataHelper.streamingAssetsPath = Application.streamingAssetsPath;
            }

            ControlSettingData saveData = new ControlSettingData();
            saveData.yawRotateSpeed = yawRotateSpeedSlider.value * MaxRotateSpeed;
            saveData.pitchRotateSpeed = pitchRotateSpeedSlider.value * MaxRotateSpeed;
            SaveDataHelper.SaveSetting(saveData);

            _canvas.enabled = false;
            _canvas.sortingOrder = 2;

            pitchRotateSpeedSlider.interactable = false;
            yawRotateSpeedSlider.interactable = false;
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
