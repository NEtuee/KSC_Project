using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputManager : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        DualShock,
        XboxPad
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

    public delegate bool ActionResult(KeybindingActions action);

    private static InputManager instance;
    [SerializeField] public KeyBindings keyBindings;
    [SerializeField] private ControlMode controlMode;
    [SerializeField] private float joystickSenstive = 10f;
    [SerializeField] private float DebugAxis;
    private Dictionary<KeybindingActions, KeyCode> pc_keyDict = new Dictionary<KeybindingActions, KeyCode>();
    private Dictionary<KeybindingActions, GamepadControlSet> gamepad_keyDict = new Dictionary<KeybindingActions, GamepadControlSet>();

    private Dictionary<KeybindingActions, KeybindingCheck> actionData = new Dictionary<KeybindingActions, KeybindingCheck>();
    private Dictionary<KeybindingActions, ActionResult> actionBinding = new Dictionary<KeybindingActions, ActionResult>();
    private void Awake()
    {
        if (null == instance)
        {
            //for (int count = 0; count < keyBindings.keybindingChecks.Length; count++)
            //{
            //    pc_keyDict.Add(keyBindings.keybindingChecks[count].action, keyBindings.keybindingChecks[count].pc);
            //    gamepad_keyDict.Add(keyBindings.keybindingChecks[count].action, new GamepadControlSet(keyBindings.keybindingChecks[count].isAxis, keyBindings.keybindingChecks[count].gamepad, keyBindings.keybindingChecks[count].axisName));
            //}

            InitializeKeyBind();

            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        //DebugAxis = Input.GetAxis("RightTrigger_Xbox");
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

    public bool GetAction(KeybindingActions action)
    {
        return actionBinding[action](action);
    }

    public void InitializeKeyBind()
    {
        actionData.Clear();
        actionBinding.Clear();

        for(int count = 0; count < keyBindings.keybindingChecks.Length; count++)
        {
            actionData.Add(keyBindings.keybindingChecks[count].action, keyBindings.keybindingChecks[count]);
            
            switch(controlMode)
            {
                case ControlMode.Keyboard:
                    {
                        switch(keyBindings.keybindingChecks[count].keyboard.buttonType)
                        {
                            case ButtonType.GetKey:
                                actionBinding.Add(keyBindings.keybindingChecks[count].action, BindKeyboard_GetKey);
                                break;
                            case ButtonType.GetKeyDown:
                                actionBinding.Add(keyBindings.keybindingChecks[count].action, BindKeyboard_GetKeyDown);
                                break;
                            case ButtonType.GetKeyUp:
                                actionBinding.Add(keyBindings.keybindingChecks[count].action, BindKeyboard_GetKeyUp);
                                break;
                        }
                    }
                    break;
                case ControlMode.DualShock:
                    {
                        switch (keyBindings.keybindingChecks[count].dualshock.valueType)
                        {
                            case PadValueType.Button:
                                {
                                    switch(keyBindings.keybindingChecks[count].dualshock.buttonType)
                                    {
                                        case ButtonType.GetKeyDown:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_GetKeyDown);
                                            break;
                                        case ButtonType.GetKey:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_GetKey);
                                            break;
                                        case ButtonType.GetKeyUp:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_GetKeyUp);
                                            break;
                                    }
                                }
                                break;
                            case PadValueType.Axis:
                                {
                                    switch (keyBindings.keybindingChecks[count].dualshock.condition)
                                    {
                                        case AxisCondition.Equal:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_AxisEqual);
                                            break;
                                        case AxisCondition.NotEqual:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_AxisNotEqual);
                                            break;
                                        case AxisCondition.Greater:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_AxisGreater);
                                            break;
                                        case AxisCondition.Less:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindDualShock_AxisLess);
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ControlMode.XboxPad:
                    {
                        switch (keyBindings.keybindingChecks[count].xbox.valueType)
                        {
                            case PadValueType.Button:
                                {
                                    switch (keyBindings.keybindingChecks[count].xbox.buttonType)
                                    {
                                        case ButtonType.GetKeyDown:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_GetKeyDown);
                                            break;
                                        case ButtonType.GetKey:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_GetKey);
                                            break;
                                        case ButtonType.GetKeyUp:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_GetKeyUp);
                                            break;
                                    }
                                }
                                break;
                            case PadValueType.Axis:
                                {
                                    switch (keyBindings.keybindingChecks[count].xbox.condition)
                                    {
                                        case AxisCondition.Equal:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_AxisEqual);
                                            break;
                                        case AxisCondition.NotEqual:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_AxisNotEqual);
                                            break;
                                        case AxisCondition.Greater:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_AxisGreater);
                                            break;
                                        case AxisCondition.Less:
                                            actionBinding.Add(keyBindings.keybindingChecks[count].action, BindXbox_AxisLess);
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }


    public bool GetKeyDown(KeybindingActions action)
    {
        switch(controlMode)
        {
            case ControlMode.Keyboard:
                return Input.GetKeyDown(pc_keyDict[action]);
            case ControlMode.DualShock:
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
            case ControlMode.DualShock:
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
            case ControlMode.DualShock:
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
            case ControlMode.DualShock:
                return Input.GetAxis("RightStickX_DualShock");
            case ControlMode.XboxPad:
                return Input.GetAxis("RightStickX_Xbox");
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
            case ControlMode.DualShock:
                return Input.GetAxis("RightStickY_DualShock");
            case ControlMode.XboxPad:
                return Input.GetAxis("RightStickY_Xbox");
            default:
                return Input.GetAxis("Mouse Y");
        }
    }

    #region 키보드 바인딩
    private bool BindKeyboard_GetKeyDown(KeybindingActions action)
    {
        return Input.GetKeyDown(actionData[action].keyboard.key);
    }

    private bool BindKeyboard_GetKey(KeybindingActions action)
    {
        return Input.GetKey(actionData[action].keyboard.key);
    }

    private bool BindKeyboard_GetKeyUp(KeybindingActions action)
    {
        return Input.GetKeyUp(actionData[action].keyboard.key);
    }
    #endregion

    #region 듀얼쇼크 바인딩
    private bool BindDualShock_GetKeyDown(KeybindingActions action)
    {
        return Input.GetKeyDown(actionData[action].dualshock.key);
    }

    private bool BindDualShock_GetKey(KeybindingActions action)
    {
        return Input.GetKey(actionData[action].dualshock.key);
    }

    private bool BindDualShock_GetKeyUp(KeybindingActions action)
    {
        return Input.GetKeyUp(actionData[action].dualshock.key);
    }

    private bool BindDualShock_AxisEqual(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].dualshock.axisName) == actionData[action].dualshock.value);
    }

    private bool BindDualShock_AxisNotEqual(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].dualshock.axisName) != actionData[action].dualshock.value);
    }

    private bool BindDualShock_AxisGreater(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].dualshock.axisName) > actionData[action].dualshock.value);
    }

    private bool BindDualShock_AxisLess(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].dualshock.axisName) < actionData[action].dualshock.value);
    }
    #endregion

    #region 엑스박스 패드 바인딩
    private bool BindXbox_GetKeyDown(KeybindingActions action)
    {
        return Input.GetKeyDown(actionData[action].xbox.key);
    }

    private bool BindXbox_GetKey(KeybindingActions action)
    {
        return Input.GetKey(actionData[action].xbox.key);
    }

    private bool BindXbox_GetKeyUp(KeybindingActions action)
    {
        return Input.GetKeyUp(actionData[action].xbox.key);
    }
    
    private bool BindXbox_AxisEqual(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].xbox.axisName) == actionData[action].xbox.value);
    }

    private bool BindXbox_AxisNotEqual(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].xbox.axisName) != actionData[action].xbox.value);
    }

    private bool BindXbox_AxisGreater(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].xbox.axisName) > actionData[action].xbox.value);
    }

    private bool BindXbox_AxisLess(KeybindingActions action)
    {
        return (Input.GetAxis(actionData[action].xbox.axisName) < actionData[action].xbox.value);
    }
    #endregion

    #region 엑박 패드 진동
    public void GamePadSetVibrate(float time, float power)
    {
        if (controlMode != ControlMode.XboxPad)
            return;

        StartCoroutine(GamePadVibrate(time,power));
    }

    IEnumerator GamePadVibrate(float time,float power)
    {
        float currentTime = 0.0f;
        GamePad.SetVibration(0, power, power);
        while (currentTime < time)
        {
            currentTime += Time.fixedUnscaledDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        GamePad.SetVibration(0, 0, 0);
    }
    #endregion
}
