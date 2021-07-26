using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : ManagerBase
{
    [SerializeField] private FollowTargetCtrl followTarget;

    public Vector2[] respondResolutionsVectors;
    private List<Resolution> _respondResolutions = new List<Resolution>();

    private const float MaxRotateSpeed = 1000f;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("SettingManager");

        SaveDataHelper.streamingAssetsPath = Application.streamingAssetsPath;

        AddAction(MessageTitles.setting_savecamerarotatespeed, (msg) => 
        {
            CameraRotateSpeedData data = (CameraRotateSpeedData)msg.data;
            SaveCameraRotateSpeed(data.pitch, data.yaw);
        });

        AddAction(MessageTitles.setting_saveVolume, (msg) =>
        {
            VolumeData data = (VolumeData)msg.data;
            SaveSoundVolume(data.master, data.sfx, data.ambient, data.bgm);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        ControlSettingData controlSettingData = SaveDataHelper.LoadSetting<ControlSettingData>();
        followTarget.PitchRotateSpeed = controlSettingData.pitchRotateSpeed;
        followTarget.YawRotateSpeed = controlSettingData.yawRotateSpeed;

        CameraRotateSpeedData cameraRotateSpeedData;
        cameraRotateSpeedData.pitch = followTarget.PitchRotateSpeed / MaxRotateSpeed;
        cameraRotateSpeedData.yaw = followTarget.YawRotateSpeed / MaxRotateSpeed;

        SendMessageEx(MessageTitles.uimanager_setvaluecamerarotatespeedslider, GetSavedNumber("UIManager"), cameraRotateSpeedData);

        DisplaySettingData displaySettingData = SaveDataHelper.LoadSetting<DisplaySettingData>();
        QualitySettings.vSyncCount = displaySettingData.activeVsync == true ? 1 : 0;
        Screen.SetResolution(displaySettingData.screenWidth, displaySettingData.screenHeight, Screen.fullScreen);

        Resolution[] resolutions = Screen.resolutions;
        ResolutionData resolutionData;
        resolutionData.resolutionStrings = new List<string>();
        foreach (var res in resolutions)
        {
            foreach (var respondRes in respondResolutionsVectors)
            {
                if (res.width == (int)respondRes.x && res.height == (int)respondRes.y)
                {
                    if (resolutionData.resolutionStrings.Contains(res.width + "x" + res.height) == false)
                    {
                        resolutionData.resolutionStrings.Add(res.width + "x" + res.height);
                        _respondResolutions.Add(res);
                    }
                    break;
                }
            }
        }
        SendMessageEx(MessageTitles.uimanager_setresolutiondropdown, GetSavedNumber("UIManager"), resolutionData);

        for (int i = 0; i < _respondResolutions.Count; i++)
        {
            if (Screen.currentResolution.height == _respondResolutions[i].height &&
                Screen.currentResolution.width == _respondResolutions[i].width)
            {
                SendMessageEx(MessageTitles.uimanager_setvalueresolutiondropdown, GetSavedNumber("UIManager"), i);
                break;
            }
        }
        SendMessageEx(MessageTitles.uimanager_setvaluescreenmodedropdown, GetSavedNumber("UIManager"), Screen.fullScreen ? 0 : 1);
        SendMessageEx(MessageTitles.uimanager_setvaluevsyncdropdown, GetSavedNumber("UIManager"), QualitySettings.vSyncCount == 0 ? 0 : 1);

        SoundSettingData soundSettingData = SaveDataHelper.LoadSetting<SoundSettingData>();
        SetParameterData setParameterData;
        setParameterData.paramId = 1; setParameterData.value = soundSettingData.masterVolume; setParameterData.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"), setParameterData);
        SetParameterData setParameterData2;
        setParameterData2.paramId = 2; setParameterData2.value = soundSettingData.sfxVolume; setParameterData2.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData2);
        SetParameterData setParameterData3;
        setParameterData3.paramId = 3; setParameterData3.value = soundSettingData.ambientVolume; setParameterData3.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData3);
        SetParameterData setParameterData4;
        setParameterData4.paramId = 4; setParameterData4.value = soundSettingData.bgmVolume; setParameterData4.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData4);

        VolumeData volumeData;
        volumeData.master = soundSettingData.masterVolume / 100f;
        volumeData.sfx = soundSettingData.sfxVolume / 100f;
        volumeData.ambient = soundSettingData.ambientVolume / 100f;
        volumeData.bgm = soundSettingData.bgmVolume / 100f;
        SendMessageEx(MessageTitles.uimanager_setvaluevolumeslider, GetSavedNumber("UIManager"), volumeData);
    }

    public void SaveCameraRotateSpeed(float pitch, float yaw)
    {
        ControlSettingData saveData;
        saveData.yawRotateSpeed = pitch * MaxRotateSpeed;
        saveData.pitchRotateSpeed = yaw * MaxRotateSpeed;
        followTarget.YawRotateSpeed = saveData.yawRotateSpeed;
        followTarget.PitchRotateSpeed = saveData.pitchRotateSpeed;
        SaveDataHelper.SaveSetting(saveData);
    }

    public void SaveSoundVolume(float master, float sfx, float ambient, float bgm)
    {
        SoundSettingData soundData;
        soundData.masterVolume = master * 100f;
        soundData.sfxVolume = sfx * 100f;
        soundData.ambientVolume = ambient * 100f;
        soundData.bgmVolume = bgm * 100f;
        SaveDataHelper.SaveSetting(soundData);
    }

    public void SetResoultion(int value)
    {
        Screen.SetResolution(_respondResolutions[value].width, _respondResolutions[value].height, Screen.fullScreen);
        SaveDisplaySetting();
    }

    public void SetScreenMode(int value)
    {
        if (value == 0)
        {
            if (Screen.fullScreen == false)
                Screen.fullScreen = true;
        }
        else
        {
            if (Screen.fullScreen == true)
                Screen.fullScreen = false;
        }

        SaveDisplaySetting();
    }

    public void SetVsnc(int value)
    {
        QualitySettings.vSyncCount = value == 0 ? 0 : 1;
    }

    public void SaveDisplaySetting()
    {
        DisplaySettingData displaySettingData;
        displaySettingData.activeVsync = QualitySettings.vSyncCount != 0;
        displaySettingData.screenWidth = Screen.currentResolution.width;
        displaySettingData.screenHeight = Screen.currentResolution.height;
        SaveDataHelper.SaveSetting(displaySettingData);
    }

    protected void Start()
    {
        if(followTarget == null)
        {
            Debug.LogError("Not Set FollowTarget");
        }
    }
}

public struct CameraRotateSpeedData
{
    public float yaw;
    public float pitch;
}

public struct VolumeData
{
    public float master;
    public float sfx;
    public float ambient;
    public float bgm;
}

public struct ResolutionData
{
    public List<string> resolutionStrings;
}
