using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class KeyBindings : ScriptableObject
{
    [System.Serializable]
    public class SerializeDictKey : SerializeDictionary<KeybindingActions, KeybindingCheck> { }

    [System.Serializable]
    public class KeybindingCheck
    {
        public KeybindingActions action;
        public KeyCode pc;
        public KeyCode gamepad;
        public bool isAxis;
        public string axisName;
    }

    public KeybindingCheck[] keybindingChecks;

}
