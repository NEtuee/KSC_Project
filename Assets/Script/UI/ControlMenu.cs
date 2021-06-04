using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMenu : EscMenu
{
    public Canvas canvas;

    public List<SliderSet> sliderItems = new List<SliderSet>();

    public Slider yawRotateSpeedSlider;
    public Slider pitchRotateSpeedSlider;

    private const float MaxRotateSpeed = 1000f;

    public override void Init()
    {
        canvas.enabled = false;
        foreach (var sliderItem in sliderItems)
        {
            sliderItem.Active(false);
        }
    }

    public override void Active(bool active)
    {
        canvas.enabled = active;
        foreach (var sliderItem in sliderItems)
        {
            sliderItem.Active(active);
        }

        if (active)
        {
            ControlSettingData controlSettingData = SaveDataHelper.LoadSetting<ControlSettingData>();
            pitchRotateSpeedSlider.value = controlSettingData.pitchRotateSpeed / MaxRotateSpeed;
            yawRotateSpeedSlider.value = controlSettingData.yawRotateSpeed / MaxRotateSpeed;
        }
        else
        {
            if (GameManager.Instance.followTarget != null)
            {
                GameManager.Instance.followTarget.PitchRotateSpeed = pitchRotateSpeedSlider.value * MaxRotateSpeed;
                GameManager.Instance.followTarget.YawRotateSpeed = yawRotateSpeedSlider.value * MaxRotateSpeed;
            }

            if (SaveDataHelper.streamingAssetsPath == null)
            {
                SaveDataHelper.streamingAssetsPath = Application.streamingAssetsPath;
            }

            ControlSettingData saveData = new ControlSettingData();
            saveData.yawRotateSpeed = yawRotateSpeedSlider.value * MaxRotateSpeed;
            saveData.pitchRotateSpeed = pitchRotateSpeedSlider.value * MaxRotateSpeed;
            SaveDataHelper.SaveSetting(saveData);
        }
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
