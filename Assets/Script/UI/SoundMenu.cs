using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : EscMenu
{
    public Canvas canvas;

    public List<SliderSet> sliderItems = new List<SliderSet>();

    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider ambientVolumeSlider;
    public Slider bgmVolumeSlider;

    public void Init()
    {
        canvas.enabled = false;
        foreach(var sliderItem in sliderItems)
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

        if(active)
        {
            float value = 0;
            value = GameManager.Instance.soundManager.GetGlobalParam(1);
            masterVolumeSlider.value = value / 100f;
            value = GameManager.Instance.soundManager.GetGlobalParam(2);
            sfxVolumeSlider.value = value / 100f;
            value = GameManager.Instance.soundManager.GetGlobalParam(3);
            ambientVolumeSlider.value = value / 100f;
            value = GameManager.Instance.soundManager.GetGlobalParam(4);
            bgmVolumeSlider.value = value / 100f;
        }
        else
        {
            if (SaveDataHelper.streamingAssetsPath == null)
            {
                SaveDataHelper.streamingAssetsPath = Application.streamingAssetsPath;
            }

            SoundSettingData saveData = new SoundSettingData();
            saveData.masterVolume = (masterVolumeSlider.value * 100f);
            saveData.sfxVolume = (sfxVolumeSlider.value * 100f);
            saveData.ambientVolume = (ambientVolumeSlider.value * 100f);
            saveData.bgmVolume = (bgmVolumeSlider.value * 100f);
            SaveDataHelper.SaveSetting(saveData);
        }
    }

    public void ChangeVolumeValue(int id)
    {
        int value = 0;
        switch (id)
        {
            case 1:
                {
                    value = (int)(masterVolumeSlider.value * 100f);
                }
                break;
            case 2:
                {
                    value = (int)(sfxVolumeSlider.value * 100f);
                }
                break;
            case 3:
                {
                    value = (int)(ambientVolumeSlider.value * 100f);
                }
                break;
            case 4:
                {
                    value = (int)(bgmVolumeSlider.value * 100f);
                }
                break;
        }
        GameManager.Instance.soundManager.SetGlobalParam(id, value);
    }
    public override void Appear(float duration)
    {
      
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
    
    }

    public override void Disappear(float duration)
    {
  
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
   
    }
}
