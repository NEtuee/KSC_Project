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
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void SaveBinding()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
