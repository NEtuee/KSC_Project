using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class SaveRebind : MonoBehaviour
{
    public InputActionAsset actions;

    private const string keyBindingJsonDataPath = "/rebinds.json";

    private void Awake()
    {
        LoadBinding();
    }

    public void LoadBinding()
    {
        //string path = Application.streamingAssetsPath + keyBindingJsonDataPath;
        //if (File.Exists(path) == false)
        //{
        //    SaveBinding();
        //    return;
        //}


        //actions.LoadFromJson(File.ReadAllText(path));

        Debug.Log("Load");
        foreach (var actionMap in actions.actionMaps)
        {
            var bindings = actionMap.bindings;
            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                string key = binding.id.ToString();
                string val = PlayerPrefs.GetString(key, null);
                if (string.IsNullOrEmpty(val)) continue;
                actionMap.ApplyBindingOverride(i, new InputBinding { overridePath = val });
            }
        }
    }

    public void SaveBinding()
    {
        //string path = Application.streamingAssetsPath + keyBindingJsonDataPath;
        //string rebinds = actions.ToJson();
        //File.WriteAllText(path, rebinds);
        Debug.Log("Save");

        foreach (var actionMap in actions.actionMaps)
        {
            foreach (var binding in actionMap.bindings)
            {
                if (!string.IsNullOrEmpty(binding.overridePath))
                {
                    string key = binding.id.ToString();
                    string val = binding.overridePath;
                    PlayerPrefs.SetString(key, val);
                }
            }
        }
    }
}
