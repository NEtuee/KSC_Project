using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
public class KeyboardType : ICloneable
{
    public ButtonType buttonType;
    public KeyCode key;

    public object Clone()
    {
        KeyboardType copy = new KeyboardType();
        copy.buttonType = buttonType;
        copy.key = key;

        return copy;
    }
}

[System.Serializable]
public class KeyboardTypeToggle : ICloneable
{
    public KeyCode key;

    public object Clone()
    {
        KeyboardTypeToggle copy = new KeyboardTypeToggle();
        copy.key = key;
        return copy;
    }
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
public class GamePadTypeToggle : ICloneable
{
    public PadValueType valueType;
    public KeyCode key;
    public string axisName;

    public object Clone()
    {
        GamePadTypeToggle copy = new GamePadTypeToggle();
        copy.valueType = valueType;
        copy.key = key;
        copy.axisName = axisName;

        return copy;
    }
}

[System.Serializable]
public class KeybindingCheck 
{
    public KeybindingActions action;
    public KeyboardType keyboard;
    public GamePadType dualshock;
    public GamePadType xbox;

 
}

[System.Serializable]
public class KeybindingCheckToggle : ICloneable
{
    public KeybindingActions action;
    public bool isToggle;
    public KeyboardTypeToggle keyboard;
    public GamePadTypeToggle dualshock;
    public GamePadTypeToggle xbox;

    public object Clone()
    {
        KeybindingCheckToggle copy = new KeybindingCheckToggle();
        copy.action = action;
        copy.isToggle = isToggle;
        copy.keyboard = (KeyboardTypeToggle)keyboard.Clone();
        copy.dualshock = (GamePadTypeToggle)dualshock.Clone();
        copy.xbox = (GamePadTypeToggle)xbox.Clone();

        return copy;
    }
}

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class KeyBindings : ScriptableObject
{
    [System.Serializable]
    public class SerializeDictKey : SerializeDictionary<KeybindingActions, KeybindingCheck> { }

    public KeybindingCheck[] keybindingChecks;

}


