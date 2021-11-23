using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InfomationText
{
    public string key;
    [TextArea] public string keyboardMouse;
    [TextArea] public string gamepad;
}

[CreateAssetMenu(fileName = "InformationScriptable", menuName = "InformationScriptable")]
public class InformationScriptable : ScriptableObject
{
    public List<InfomationText> data = new List<InfomationText>();
}
