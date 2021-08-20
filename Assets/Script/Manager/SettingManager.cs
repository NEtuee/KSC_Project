using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class SettingManager : ManagerBase
{
    [SerializeField] private FollowTargetCtrl followTarget;

    public Vector2[] respondResolutionsVectors;
    private List<Resolution> _respondResolutions = new List<Resolution>();

    private const float MaxRotateSpeed = 1000f;
    private int currentWidth;
    private int currentHeight;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("SettingManager",true);

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

        AddAction(MessageTitles.setting_setScreenMode, (msg) =>
         {
             IntData data = MessageDataPooling.CastData<IntData>(msg.data);
             SetScreenMode(data.value);
         });
        AddAction(MessageTitles.setting_setResolution, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            SetResolution(data.value);
        });
        AddAction(MessageTitles.setting_setVsync, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            SetVsnc(data.value);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        ControlSettingData controlSettingData = SaveDataHelper.LoadSetting<ControlSettingData>();
        if (followTarget != null)
        {
            followTarget.PitchRotateSpeed = controlSettingData.pitchRotateSpeed;
            followTarget.YawRotateSpeed = controlSettingData.yawRotateSpeed;
        }

        CameraRotateSpeedData cameraRotateSpeedData = MessageDataPooling.GetMessageData<CameraRotateSpeedData>();
        cameraRotateSpeedData.pitch = controlSettingData.pitchRotateSpeed / MaxRotateSpeed;
        cameraRotateSpeedData.yaw = controlSettingData.yawRotateSpeed / MaxRotateSpeed;

        SendMessageEx(MessageTitles.uimanager_setvaluecamerarotatespeedslider, GetSavedNumber("UIManager"), cameraRotateSpeedData);

        DisplaySettingData displaySettingData = SaveDataHelper.LoadSetting<DisplaySettingData>();
        QualitySettings.vSyncCount = displaySettingData.activeVsync == true ? 1 : 0;
        currentWidth = displaySettingData.screenWidth; currentHeight = displaySettingData.screenHeight;
        //Debug.Log("Start : "+currentWidth + " x " + currentHeight);
        Screen.SetResolution(displaySettingData.screenWidth, displaySettingData.screenHeight, Screen.fullScreen);

        Resolution[] resolutions = Screen.resolutions;
        ResolutionData resolutionData = MessageDataPooling.GetMessageData<ResolutionData>();
        resolutionData.resolutionStrings.Clear();
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
            if (currentHeight == _respondResolutions[i].height &&
                currentWidth == _respondResolutions[i].width)
            {
                IntData intData = MessageDataPooling.GetMessageData<IntData>();
                intData.value = i;
                SendMessageEx(MessageTitles.uimanager_setvalueresolutiondropdown, GetSavedNumber("UIManager"), intData);
                break;
            }
        }

        IntData fullScreen = MessageDataPooling.GetMessageData<IntData>();
        fullScreen.value = Screen.fullScreen ? 0 : 1;
        SendMessageEx(MessageTitles.uimanager_setvaluescreenmodedropdown, GetSavedNumber("UIManager"), fullScreen);
        IntData vsync = MessageDataPooling.GetMessageData<IntData>();
        vsync.value = QualitySettings.vSyncCount == 0 ? 0 : 1;
        SendMessageEx(MessageTitles.uimanager_setvaluevsyncdropdown, GetSavedNumber("UIManager"), vsync);

        SoundSettingData soundSettingData = SaveDataHelper.LoadSetting<SoundSettingData>();
        SetParameterData setParameterData = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData.paramId = 1; setParameterData.value = soundSettingData.masterVolume; setParameterData.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"), setParameterData);
        SetParameterData setParameterData2 = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData2.paramId = 2; setParameterData2.value = soundSettingData.sfxVolume; setParameterData2.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData2);
        SetParameterData setParameterData3 = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData3.paramId = 3; setParameterData3.value = soundSettingData.ambientVolume; setParameterData3.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData3);
        SetParameterData setParameterData4 = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData4.paramId = 4; setParameterData4.value = soundSettingData.bgmVolume; setParameterData4.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData4);

        VolumeData volumeData = MessageDataPooling.GetMessageData<VolumeData>();
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
        if (followTarget != null)
        {
            followTarget.YawRotateSpeed = saveData.yawRotateSpeed;
            followTarget.PitchRotateSpeed = saveData.pitchRotateSpeed;
        }
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

        SetParameterData setParameterData = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData.paramId = 1; setParameterData.value = soundData.masterVolume; setParameterData.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData);
        SetParameterData setParameterData2 = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData2.paramId = 2; setParameterData2.value = soundData.sfxVolume; setParameterData2.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData2);
        SetParameterData setParameterData3 = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData3.paramId = 3; setParameterData3.value = soundData.ambientVolume; setParameterData3.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData3);
        SetParameterData setParameterData4 = MessageDataPooling.GetMessageData<SetParameterData>();
        setParameterData4.paramId = 4; setParameterData4.value = soundData.bgmVolume; setParameterData4.soundId = 0;
        SendMessageEx(MessageTitles.fmod_setGlobalParam, GetSavedNumber("FMODManager"), setParameterData4);

    }

    public void SetResolution(int value)
    {
        Screen.SetResolution(_respondResolutions[value].width, _respondResolutions[value].height, Screen.fullScreen);
        currentWidth = _respondResolutions[value].width; currentHeight = _respondResolutions[value].height;
        //Debug.Log(currentWidth + " x " + currentHeight);
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
        displaySettingData.screenWidth = currentWidth;
        displaySettingData.screenHeight = currentHeight;
        //Debug.Log("Save : " + displaySettingData.screenWidth + " x " + displaySettingData.screenHeight);
        SaveDataHelper.SaveSetting(displaySettingData);
    }

    protected void Start()
    {
        if(followTarget == null)
        {
            Debug.LogWarning("Not Set FollowTarget");
        }
    }
}

namespace MD
{
    public class CameraRotateSpeedData : MessageData
    {
        public float yaw;
        public float pitch;
    }

    public class VolumeData : MessageData
    {
        public float master;
        public float sfx;
        public float ambient;
        public float bgm;
    }

    public class ResolutionData : MessageData
    {
        public List<string> resolutionStrings = new List<string>();
    }
}