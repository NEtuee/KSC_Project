using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class DisplayOptionPanel : EscMenu
{
    private Canvas _canvas;

    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown vsyncDropdown;
    public Vector2[] respondResolutionsVectors;
    public List<string> resolutionStrings = new List<string>();
    
    private List<Resolution> _respondResolutions = new List<Resolution>();

    private void Awake()
    {
        // _canvas = GetComponent<Canvas>();
        // _canvas.enabled = false;
        //
        // Resolution[] resolutions = Screen.resolutions;
        // foreach (var res in resolutions)
        // {
        //     foreach (var respondRes in respondResolutionsVectors)
        //     {
        //         if (res.width == (int) respondRes.x && res.height == (int) respondRes.y)
        //         {
        //             resolutionStrings.Add(res.width + "x" + res.height);
        //             _respondResolutions.Add(res);
        //             break;
        //         }
        //     }
        // }
        //
        // resolutionDropdown.AddOptions(resolutionStrings);
        //
        // Resolution currenResolution = Screen.currentResolution;
        // for (int i = 0; i < _respondResolutions.Count; i++)
        // {
        //     if (currenResolution.height == _respondResolutions[i].height &&
        //         currenResolution.width == _respondResolutions[i].width)
        //     {
        //         resolutionDropdown.value = i;
        //         break;
        //     }
        // }
        //
        // screenModeDropdown.value = Screen.fullScreen ? 0 : 1;
    }

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        
        Resolution[] resolutions = Screen.resolutions;
        //Debug.Log(resolutions.Length);
        
        //foreach (var res in resolutions)
        //{
        //    Debug.Log(res.width+"x"+res.height);
        //}
        
        foreach (var res in resolutions)
        {
            foreach (var respondRes in respondResolutionsVectors)
            {
                if (res.width == (int) respondRes.x && res.height == (int) respondRes.y)
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

        Resolution currenResolution = Screen.currentResolution;
        for (int i = 0; i < _respondResolutions.Count; i++)
        {
            if (currenResolution.height == _respondResolutions[i].height &&
                currenResolution.width == _respondResolutions[i].width)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        screenModeDropdown.value = Screen.fullScreen ? 0 : 1;

        screenModeDropdown.interactable = false;
        resolutionDropdown.interactable = false;

    }

    public void ChangeScreenMode()
    {
        if (screenModeDropdown.value == 0)
        {
            if(Screen.fullScreen == false)
               Screen.fullScreen = true;
        }
        else
        {
            if(Screen.fullScreen == true)
                Screen.fullScreen = false;
        }
    }

    public void ChangeVsync()
    {
        if(vsyncDropdown.value == 0)
        {
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
        }
    }

    public void ChangeResolution()
    {
        Resolution currentResolution = _respondResolutions[resolutionDropdown.value];

        //if (Screen.currentResolution.height != currentResolution.height || Screen.currentResolution.width != currentResolution.width)
        //{
        //    Screen.SetResolution(currentResolution.width,currentResolution.height,Screen.fullScreen);
        //}
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
    }

    public override void Active(bool active)
    {
        if (active)
        {
            _canvas.enabled = true;
            _canvas.sortingOrder = 3;

            screenModeDropdown.interactable = true;
            resolutionDropdown.interactable = true;
        }
        else
        {
            _canvas.enabled = false;
            _canvas.sortingOrder = 2;

            screenModeDropdown.interactable = false;
            resolutionDropdown.interactable = false;

            DisplaySettingData displaySettingData = new DisplaySettingData();
            displaySettingData.activeVsync = QualitySettings.vSyncCount != 0;
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
