using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayMenu : EscMenu
{
    public Canvas canvas;

    public List<DropBoxSet> dropBoxSets = new List<DropBoxSet>();

    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown vsyncDropdown;

    public Vector2[] respondResolutionsVectors;
    public List<string> resolutionStrings = new List<string>();

    private List<Resolution> _respondResolutions = new List<Resolution>();
    private Resolution _currentResolution;

    public override void Init()
    {
        canvas.enabled = false;

        Resolution[] resolutions = Screen.resolutions;

        foreach (var res in resolutions)
        {
            foreach (var respondRes in respondResolutionsVectors)
            {
                if (res.width == (int)respondRes.x && res.height == (int)respondRes.y)
                {
                    if (resolutionStrings.Contains(res.width + "x" + res.height) == false)
                    {
                        resolutionStrings.Add(res.width + "x" + res.height);
                        _respondResolutions.Add(res);
                    }
                    break;
                }
            }
        }

        resolutionDropdown.AddOptions(resolutionStrings);

        _currentResolution = Screen.currentResolution;
        for (int i = 0; i < _respondResolutions.Count; i++)
        {
            if (_currentResolution.height == _respondResolutions[i].height &&
                _currentResolution.width == _respondResolutions[i].width)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        screenModeDropdown.value = Screen.fullScreen ? 0 : 1;

        screenModeDropdown.interactable = false;
        resolutionDropdown.interactable = false;

        foreach(var dropdownItem in dropBoxSets)
        {
            dropdownItem.Active(false);
        }
    }

    public void ChangeScreenMode()
    {
        if (screenModeDropdown.value == 0)
        {
            if (Screen.fullScreen == false)
                Screen.fullScreen = true;
        }
        else
        {
            if (Screen.fullScreen == true)
                Screen.fullScreen = false;
        }
    }

    public void ChangeVsync()
    {
        QualitySettings.vSyncCount = vsyncDropdown.value == 0 ? 0 : 1;
    }

    public void ChangeResolution()
    {
        _currentResolution = _respondResolutions[resolutionDropdown.value];
        Screen.SetResolution(_currentResolution.width, _currentResolution.height, Screen.fullScreen);
    }


    public override void Active(bool active)
    {
        canvas.enabled = active;
        foreach(var dropboxItem in dropBoxSets)
        {
            dropboxItem.Active(active);
        }

        if (active == false)
        {
            DisplaySettingData displaySettingData = new DisplaySettingData();
            displaySettingData.activeVsync = QualitySettings.vSyncCount != 0;
            displaySettingData.screenWidth = _currentResolution.width;
            displaySettingData.screenHeight = _currentResolution.height;
            SaveDataHelper.SaveSetting(displaySettingData);
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
