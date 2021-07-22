using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct ControlSettingData
{
    public float pitchRotateSpeed;
    public float yawRotateSpeed;
}

[System.Serializable]
public struct SoundSettingData
{
    public float masterVolume;
    public float sfxVolume;
    public float ambientVolume;
    public float bgmVolume;
}

[System.Serializable]
public struct DisplaySettingData
{
    public bool activeVsync;
    public int screenWidth;
    public int screenHeight;
}

public static class SaveDataHelper
{
    public static string streamingAssetsPath = null;

    private static string _controlSaveFileName = "ControlSave.json";
    private static string _soundSaveFileName = "SoundSave.json";
    private static string _displaySaveFileName = "DisplaySave.json";

    public static void SaveSetting<T>(T data)
    {
        string path = streamingAssetsPath;

        if (data is ControlSettingData)
        {
            path += "/"+ _controlSaveFileName;
        }
        else if(data is SoundSettingData)
        {
            path += "/" + _soundSaveFileName;
        }
        else if(data is DisplaySettingData)
        {
            path += "/" + _displaySaveFileName;
        }

        if (File.Exists(path) == false)
        {
            File.Create(path);
        }

        string jsonData = JsonUtility.ToJson(data,true);
        File.WriteAllText(path, jsonData);
    }

    public static T LoadSetting<T>() where T : new()
    {
        string path = streamingAssetsPath;

        T loadData = new T();

        if (loadData.GetType() == typeof(ControlSettingData))
        {
            path += "/" + _controlSaveFileName;
        }
        else if (loadData.GetType() == typeof(SoundSettingData))
        {
            path += "/" + _soundSaveFileName;
        }
        else if(loadData.GetType() == typeof(DisplaySettingData))
        {
            path += "/" + _displaySaveFileName;
        }

        string jsonData = File.ReadAllText(path);
        loadData = JsonUtility.FromJson<T>(jsonData);
        return loadData;
    }
}
