using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    GetKeyDown,
    GetKey,
    GetKeyUp
}

public enum PadValueType
{
    Button,
    Axis
}

public enum AxisCondition
{
    Equal,
    NotEqual,
    Greater,
    Less
}

[System.Serializable]
public class KeyboardType
{
    public ButtonType buttonType;
    public KeyCode key;
}

[System.Serializable]
public class GamePadType
{
    public PadValueType valueType;
    public ButtonType buttonType;
    public KeyCode key;
    public string axisName;
    public AxisCondition condition;
    public float value;
}

[System.Serializable]
public class KeybindingCheck
{
    public KeybindingActions action;
    public KeyboardType keyboard;
    public GamePadType dualshock;
    public GamePadType xbox;
}

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class KeyBindings : ScriptableObject
{
    [System.Serializable]
    public class SerializeDictKey : SerializeDictionary<KeybindingActions, KeybindingCheck> { }

    public KeybindingCheck[] keybindingChecks;

}
