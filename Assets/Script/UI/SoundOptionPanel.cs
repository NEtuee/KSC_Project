using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class SoundOptionPanel : EscMenu
{
    private Canvas _canvas;

    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider ambientVolumeSlider;
    public Slider bgmVolumeSlider;

    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI ambientVolumeText;
    public TextMeshProUGUI bgmVolumeText;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        masterVolumeSlider.interactable = false;
        sfxVolumeSlider.interactable = false;
        ambientVolumeSlider.interactable = false;
        bgmVolumeSlider.interactable = false;
    }

    public override void Active(bool active)
    {
        if (active)
        {
            //GameManager.Instance.soundManager.Play(4000, Vector3.zero);
            //GameManager.Instance.soundManager.Play(4001, Vector3.zero);
            //GameManager.Instance.soundManager.Play(4002, Vector3.zero);
            //GameManager.Instance.soundManager.Play(4003, Vector3.zero);

            _canvas.enabled = true;
            _canvas.sortingOrder = 3;

            float value = 0;
            value = GameManager.Instance.soundManager.GetGlobalParam(1);
            masterVolumeText.text = ((int)value).ToString();
            masterVolumeSlider.value = value / 100f;
            value = GameManager.Instance.soundManager.GetGlobalParam(2);
            sfxVolumeText.text = ((int)value).ToString();
            sfxVolumeSlider.value = value / 100f;
            value = GameManager.Instance.soundManager.GetGlobalParam(3);
            ambientVolumeText.text = ((int)value).ToString();
            ambientVolumeSlider.value = value / 100f;
            value = GameManager.Instance.soundManager.GetGlobalParam(4);
            bgmVolumeText.text = ((int)value).ToString();
            bgmVolumeSlider.value = value / 100f;

            masterVolumeSlider.interactable = true;
            sfxVolumeSlider.interactable = true;
            ambientVolumeSlider.interactable = true;
            bgmVolumeSlider.interactable = true;
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

            _canvas.enabled = false;
            _canvas.sortingOrder = 2;

            masterVolumeSlider.interactable = false;
            sfxVolumeSlider.interactable = false;
            ambientVolumeSlider.interactable = false;
            bgmVolumeSlider.interactable = false;
        }
    }

    public void ChangeVolumeValue(int id)
    {
        int value = 0;
        switch (id)
        {
            case 1:
            {
                value = (int) (masterVolumeSlider.value * 100f);
                masterVolumeText.text = value.ToString();
            }
                break;
            case 2:
            {
                value = (int) (sfxVolumeSlider.value * 100f);
                sfxVolumeText.text = value.ToString();
            }
                break;
            case 3:
            {
                value = (int) (ambientVolumeSlider.value * 100f);
                ambientVolumeText.text = value.ToString();
            }
                break;
            case 4:
            {
                value = (int) (bgmVolumeSlider.value * 100f);
                bgmVolumeText.text = value.ToString();
            }
                break;
        }
        GameManager.Instance.soundManager.SetGlobalParam(id,value);
        //Debug.Log(GameManager.Instance.soundManager.GetGlobalParam(id));
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
