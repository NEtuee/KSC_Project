using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionText
{
    public string key;
    [TextArea] public string title;
    [TextArea] public string description;
}

[CreateAssetMenu(fileName = "MissionTextScriptable", menuName = "MissionTextScriptable")]

public class MissionTextScriptable : ScriptableObject
{
    public List<MissionText> descriptions = new List<MissionText>();
}
