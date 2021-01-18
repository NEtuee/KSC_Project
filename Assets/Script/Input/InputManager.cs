using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        Gamepad
    }

    public class GamepadControlSet
    {
        public bool isAxis;
        public KeyCode keyCode;
        public string axisName;

        public GamepadControlSet(bool isAxis,KeyCode key,string axisName)
        {
            this.isAxis = isAxis;
            this.keyCode = key;
            this.axisName = axisName;
        }
    }

    private static InputManager instance;
    [SerializeField] public KeyBindings keyBindings;
    [SerializeField] private ControlMode controlMode;
    [SerializeField] private float joystickSenstive = 10f;
    private Dictionary<KeybindingActions, KeyCode> pc_keyDict = new Dictionary<KeybindingActions, KeyCode>();
    private Dictionary<KeybindingActions, GamepadControlSet> gamepad_keyDict = new Dictionary<KeybindingActions, GamepadControlSet>();

    private void Awake()
    {
        if(null == instance)
        {
            for (int count = 0; count < keyBindings.keybindingChecks.Length; count++)
            {
                pc_keyDict.Add(keyBindings.keybindingChecks[count].action, keyBindings.keybindingChecks[count].pc);
                gamepad_keyDict.Add(keyBindings.keybindingChecks[count].action, new GamepadControlSet(keyBindings.keybindingChecks[count].isAxis, keyBindings.keybindingChecks[count].gamepad, keyBindings.keybindingChecks[count].axisName));
            }

            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static InputManager Instance
    {
        get
        {
            if(null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    public bool GetKeyDown(KeybindingActions action)
    {
        switch(controlMode)
        {
            case ControlMode.Keyboard:
                return Input.GetKeyDown(pc_keyDict[action]);
            case ControlMode.Gamepad:
                if(gamepad_keyDict[action].isAxis == true)
                {
                    if(Input.GetAxis(gamepad_keyDict[action].axisName) > 0.0f && Input.GetAxis(gamepad_keyDict[action].axisName) < 0.1f)
                    {
                        return true;
                    }
                }
                else
                {
                    return Input.GetKeyDown(gamepad_keyDict[action].keyCode);
                }
                break;
        }
        return false;
    }

    public bool GetKey(KeybindingActions action)
    {
        switch (controlMode)
        {
            case ControlMode.Keyboard:
                return Input.GetKey(pc_keyDict[action]);
            case ControlMode.Gamepad:
                if (gamepad_keyDict[action].isAxis == true)
                {
                    if (Input.GetAxis(gamepad_keyDict[action].axisName) != -0.1f)
                    {
                        return true;
                    }
                }
                else
                {
                    return Input.GetKey(gamepad_keyDict[action].keyCode);
                }
                break;
        }
        return false;
    }

    public bool GetKeyUp(KeybindingActions action)
    {
        switch (controlMode)
        {
            case ControlMode.Keyboard:
                return Input.GetKeyUp(pc_keyDict[action]);
            case ControlMode.Gamepad:
                if (gamepad_keyDict[action].isAxis == true)
                {
                    if (Input.GetAxis(gamepad_keyDict[action].axisName) < 0.0f && Input.GetAxis(gamepad_keyDict[action].axisName) > -0.1f)
                    {
                        return true;
                    }
                }
                else
                {
                    return Input.GetKeyUp(gamepad_keyDict[action].keyCode);
                }
                break;
        }
        return false;
    }

    public float GetMoveAxisVertical()
    {
        return Input.GetAxis("Vertical");
    }

    public float GetMoveAxisHorizontal()
    {
        return Input.GetAxis("Horizontal");
    }

    public float GetCameraAxisX()
    {
        switch (controlMode)
        {
            case ControlMode.Keyboard:
                return Input.GetAxis("Mouse X");
            case ControlMode.Gamepad:
                float result = Input.GetAxis("RightStickX");
                return (Mathf.Abs(result)<0.01f ? 0.0f : result) * joystickSenstive;
            default:
                return Input.GetAxis("Mouse X");
        }
    }

    public float GetCameraAxisY()
    {
        switch (controlMode)
        {
            case ControlMode.Keyboard:
                return Input.GetAxis("Mouse Y");
            case ControlMode.Gamepad:
                float result = Input.GetAxis("RightStickY");
                return (Mathf.Abs(result) < 0.01f ? 0.0f : result) * joystickSenstive;
            default:
                return Input.GetAxis("Mouse Y");
        }
    }
}
