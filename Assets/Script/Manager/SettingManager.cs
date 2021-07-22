using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : ManagerBase
{
    [SerializeField] private FollowTargetCtrl followTarget;

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

    public void SaveSoundVolume(float master, float vfx, float ambient, float bgm)
    {

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
    public float vfx;
    public float ambient;
    public float bgm;
}
