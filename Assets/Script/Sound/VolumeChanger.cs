using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChanger : MonoBehaviour
{
    public AudioSource targetAudio;

    public void VolumeSync()
    {
        SoundSettingData soundSettingData = SaveDataHelper.LoadSetting<SoundSettingData>();
        float value =  (0.01f * soundSettingData.masterVolume) * (0.01f * soundSettingData.bgmVolume);

        targetAudio.volume = value;
    }
}
