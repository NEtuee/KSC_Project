using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using System.IO;
public enum InputType
{
    Keyboard,
    DualShock,
    XboxPad
}

public enum AdditionalType
{
    First,
    Second,
    Third,
    Forth
}

public class InputManager : MonoBehaviour
{
    public enum ConnectGamePad
    {
        None,DualShock,XboxPad
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

    public class InputSet
    {
        public ActionResult GetInput;
        public ActionResult GetRelease;
        public ActionResult GetKeep;
    }

    public delegate bool ActionResult(KeybindingActions action);

    public bool inputBlock = false;

    private static InputManager instance;
    [SerializeField] public KeyBindingsToggle keyBindingsToggle;
    [SerializeField] public KeyBindingsToggle defaultKeyBinding;
    [SerializeField] public KeyBindingsToggle saveTarget;
    [SerializeField] public GamePadKeyTranslate dualshockTranslateData;
    [SerializeField] public GamePadKeyTranslate xboxTranslateData;
    [SerializeField] private float joystickSenstive = 10f;
    [SerializeField] private float DebugAxis;
    private Dictionary<KeybindingActions, KeyCode> pc_keyDict = new Dictionary<KeybindingActions, KeyCode>();
    private Dictionary<KeybindingActions, GamepadControlSet> gamepad_keyDict = new Dictionary<KeybindingActions, GamepadControlSet>();

    private Dictionary<KeybindingActions, KeybindingCheckToggle> actionData = new Dictionary<KeybindingActions, KeybindingCheckToggle>();
    private Dictionary<KeybindingActions, ActionResult> actionBinding = new Dictionary<KeybindingActions, ActionResult>();
    private Dictionary<KeybindingActions, InputSet> actionBindingToggle = new Dictionary<KeybindingActions, InputSet>();
    private Dictionary<KeybindingActions, InputSet> actionBindingDualShock = new Dictionary<KeybindingActions, InputSet>();
    private Dictionary<KeybindingActions, InputSet> actionBindingXbox = new Dictionary<KeybindingActions, InputSet>();
    private Dictionary<KeybindingActions, bool> dualShockAxisDownFlag = new Dictionary<KeybindingActions, bool>();
    private Dictionary<KeybindingActions, bool> xboxAxisDownFlag = new Dictionary<KeybindingActions, bool>();

    private List<KeybindingActions> xboxAxisActions = new List<KeybindingActions>();
    private List<KeybindingActions> dualShockActions = new List<KeybindingActions>();
    private Dictionary<KeybindingActions, bool> dualShockAxisUpFlag = new Dictionary<KeybindingActions, bool>();
    private Dictionary<KeybindingActions, bool> xboxAxisUpFlag = new Dictionary<KeybindingActions, bool>();

    private List<KeybindingActions> dualShockReleaseList = new List<KeybindingActions>();
    private List<KeybindingActions> xboxReleaseList = new List<KeybindingActions>();

    private ConnectGamePad currentConnectGamepad;

    private const string keyBindingJsonDataPath ="/KeyBinding.json";

    private Dictionary<KeyCode, string> dualShockKeycodeTranslateDict = new Dictionary<KeyCode, string>();
    private Dictionary<KeyCode, string> xboxKeycodeTranslateDict = new Dictionary<KeyCode, string>();

    private bool[] additionaFlag = new bool[4] { false,false,false,false};

    private void Awake()
    {
        LoadTranslateData();
        LoadKeyBinding();
        if (null == instance)
        {         
            InitializeKeyBind_Toggle();

            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SaveKeyBinding()
    {
        string path = Application.streamingAssetsPath + keyBindingJsonDataPath;
        if (File.Exists(path) == false)
        {
            File.Create(path);
        }

        string keyData = JsonHelper.ToJson<KeybindingCheckToggle>(keyBindingsToggle.keybindingChecks, true);
        File.WriteAllText(path, keyData);
    }

    public void LoadKeyBinding()
    {
        string path = Application.streamingAssetsPath + keyBindingJsonDataPath;
        if (File.Exists(path) == false)
            return;

        KeybindingCheckToggle[] loadKey = JsonHelper.FromJson<KeybindingCheckToggle>(File.ReadAllText(path));
        for (int i = 0; i < loadKey.Length; i++)
        {
            keyBindingsToggle.keybindingChecks[i] = loadKey[i];
        }
    }

    public void LoadTranslateData()
    {
        for(int i = 0; i< dualshockTranslateData.keyTranslatePairs.Length;i++)
        {
            dualShockKeycodeTranslateDict.Add(dualshockTranslateData.keyTranslatePairs[i].keycode, dualshockTranslateData.keyTranslatePairs[i].translateString);
        }

        for (int i = 0; i < xboxTranslateData.keyTranslatePairs.Length; i++)
        {
            xboxKeycodeTranslateDict.Add(xboxTranslateData.keyTranslatePairs[i].keycode, xboxTranslateData.keyTranslatePairs[i].translateString);
        }
    }


    private void Update()
    {
        if (dualShockReleaseList.Count != 0)
        {
            for (int i = 0; i < dualShockReleaseList.Count; i++)
            {
                if (Input.GetAxis(actionData[dualShockReleaseList[i]].dualshock.axisName) == 0.0f)
                {
                    dualShockAxisDownFlag[dualShockReleaseList[i]] = false;
                    dualShockReleaseList.Remove(dualShockReleaseList[i]);
                    break;
                }
            }
        }
        
        if (xboxReleaseList.Count != 0)
        {
            for (int i = 0; i < xboxReleaseList.Count; i++)
            {
                if (Input.GetAxis(actionData[xboxReleaseList[i]].xbox.axisName) == 0.0f)
                {
                    xboxAxisDownFlag[xboxReleaseList[i]] = false;
                    xboxReleaseList.Remove(xboxReleaseList[i]);
                    break;
                }
            }
        }

        if(dualShockActions.Count != 0)
        {
            foreach(var actions in dualShockActions)
            {
                if(Input.GetAxis(actionData[actions].dualshock.axisName) != 0.0f)
                {
                    dualShockAxisUpFlag[actions] = true;
                }
            }
        }

        if (xboxAxisActions.Count != 0)
        {
            foreach (var actions in xboxAxisActions)
            {
                if (Input.GetAxis(actionData[actions].xbox.axisName) != 0.0f)
                {
                    xboxAxisUpFlag[actions] = true;
                }
            }
        }

        CheckAddtional();
    }

    private void FixedUpdate()
    {
        DetectConnectGamePad();
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

    public void InitializeKeyBind_Toggle()
    {
        actionBindingToggle.Clear();
        actionBindingDualShock.Clear();
        actionBindingXbox.Clear();
        actionData.Clear();
        dualShockAxisDownFlag.Clear();
        dualShockAxisUpFlag.Clear();
        xboxAxisDownFlag.Clear();
        xboxAxisUpFlag.Clear();
        dualShockReleaseList.Clear();
        xboxReleaseList.Clear();

        for (int count = 0; count < keyBindingsToggle.keybindingChecks.Length; count++)
        {
            actionData.Add(keyBindingsToggle.keybindingChecks[count].action, keyBindingsToggle.keybindingChecks[count]);

            InputSet keyboardInputSet = new InputSet();
            keyboardInputSet.GetInput += BindKeyboard_GetKeyDown;
            if (keyBindingsToggle.keybindingChecks[count].isToggle == false)
                keyboardInputSet.GetRelease += BindKeyboard_GetKeyUp;
            else
                keyboardInputSet.GetRelease += BindKeyboard_GetKeyDown;
            keyboardInputSet.GetKeep += BindKeyboard_GetKey;
            actionBindingToggle.Add(keyBindingsToggle.keybindingChecks[count].action, keyboardInputSet);

            InputSet dualShockInputSet = new InputSet();
            switch (keyBindingsToggle.keybindingChecks[count].dualshock.valueType)
            {
                case PadValueType.Button:
                    {
                        dualShockInputSet.GetInput += BindDualShock_GetKeyDown;
                        if (keyBindingsToggle.keybindingChecks[count].isToggle == false)
                            dualShockInputSet.GetRelease += BindDualShock_GetKeyUp;
                        else
                            dualShockInputSet.GetRelease += BindDualShock_GetKeyDown;
                        dualShockInputSet.GetKeep += BindDualShock_GetKey;
                    }
                    break;
                case PadValueType.Axis:
                    {
                        dualShockAxisDownFlag.Add(keyBindingsToggle.keybindingChecks[count].action, false);
                        dualShockActions.Add(keyBindingsToggle.keybindingChecks[count].action);
                        dualShockAxisUpFlag.Add(keyBindingsToggle.keybindingChecks[count].action, false);
                        dualShockInputSet.GetInput += BindDualShock_AxisDown;
                        if (keyBindingsToggle.keybindingChecks[count].isToggle == false)
                            dualShockInputSet.GetRelease += BindDualShock_AxisUp;
                        else
                            dualShockInputSet.GetRelease += BindDualShock_AxisDown;
                        dualShockInputSet.GetKeep += BindDualShock_AxisKeep;
                    }
                    break;
            }
            actionBindingDualShock.Add(keyBindingsToggle.keybindingChecks[count].action,dualShockInputSet);

            InputSet xboxInputSet = new InputSet();
            switch (keyBindingsToggle.keybindingChecks[count].xbox.valueType)
            {
                case PadValueType.Button:
                    {
                        xboxInputSet.GetInput += BindXbox_GetKeyDown;
                        if (keyBindingsToggle.keybindingChecks[count].isToggle == false)
                            xboxInputSet.GetRelease += BindXbox_GetKeyUp;
                        else
                            xboxInputSet.GetRelease += BindXbox_GetKeyDown;
                        xboxInputSet.GetKeep += BindXbox_GetKey;
                    }
                    break;
                case PadValueType.Axis:
                    {
                        xboxAxisDownFlag.Add(keyBindingsToggle.keybindingChecks[count].action, false);
                        xboxAxisActions.Add(keyBindingsToggle.keybindingChecks[count].action);
                        xboxAxisUpFlag.Add(keyBindingsToggle.keybindingChecks[count].action, false);
                        xboxInputSet.GetInput += BindXbox_AxisDown;
                        if (keyBindingsToggle.keybindingChecks[count].isToggle == false)
                            xboxInputSet.GetRelease += BindXbox_AxisUp;
                        else
                            xboxInputSet.GetRelease += BindXbox_AxisDown;
                        xboxInputSet.GetKeep += BindXboxShock_AxisKeep;
                    }
                    break;
            }
            actionBindingXbox.Add(keyBindingsToggle.keybindingChecks[count].action, xboxInputSet);      
        }
    }

    public void SetDefaultKeyBinding()
    {
        for(int i = 0; i<keyBindingsToggle.keybindingChecks.Length; i++)
        {
            keyBindingsToggle.keybindingChecks[i] = (KeybindingCheckToggle)defaultKeyBinding.keybindingChecks[i].Clone();
        }

        InitializeKeyBind_Toggle();
    }

    public bool GetInput(KeybindingActions actions)
    {
        if (inputBlock == true)
            return false;

        if (currentConnectGamepad == ConnectGamePad.None)
            return actionBindingToggle[actions].GetInput(actions);
        else if (currentConnectGamepad == ConnectGamePad.DualShock)
            return actionBindingToggle[actions].GetInput(actions) || actionBindingDualShock[actions].GetInput(actions);
        else
            return actionBindingToggle[actions].GetInput(actions) || actionBindingXbox[actions].GetInput(actions);
        //return (actionBindingToggle[actions].GetInput(actions)||actionBindingDualShock[actions].GetInput(actions)|| actionBindingXbox[actions].GetInput(actions));
    }

    public bool GetRelease(KeybindingActions actions)
    {
        if (inputBlock == true)
            return false;

        if (currentConnectGamepad == ConnectGamePad.None)
            return actionBindingToggle[actions].GetRelease(actions);
        else if (currentConnectGamepad == ConnectGamePad.DualShock)
            return actionBindingToggle[actions].GetRelease(actions) || actionBindingDualShock[actions].GetRelease(actions);
        else
            return actionBindingToggle[actions].GetRelease(actions) || actionBindingXbox[actions].GetRelease(actions);
        //return (actionBindingToggle[actions].GetRelease(actions)|| actionBindingDualShock[actions].GetRelease(actions)|| actionBindingXbox[actions].GetRelease(actions));
    }

    public bool GetKeep(KeybindingActions actions)
    {
        if (inputBlock == true)
            return false;

        if (currentConnectGamepad == ConnectGamePad.None)
            return actionBindingToggle[actions].GetKeep(actions);
        else if (currentConnectGamepad == ConnectGamePad.DualShock)
            return actionBindingToggle[actions].GetKeep(actions) || actionBindingDualShock[actions].GetKeep(actions);
        else
            return actionBindingToggle[actions].GetKeep(actions) || actionBindingXbox[actions].GetKeep(actions);

        //return (actionBindingToggle[actions].GetKeep(actions) || actionBindingDualShock[actions].GetKeep(actions) || actionBindingXbox[actions].GetKeep(actions));
    }

    public string GetBindingKeycode(KeybindingActions action,InputType inputType)
    {
        switch (inputType)
        {
            case InputType.Keyboard:
                return actionData[action].keyboard.key.ToString();
            case InputType.DualShock:
                {
                    if (actionData[action].dualshock.valueType == PadValueType.Button)
                        return dualShockKeycodeTranslateDict[actionData[action].dualshock.key];
                    else
                        return actionData[action].dualshock.axisName;
                }
            case InputType.XboxPad:
                {
                    if (actionData[action].xbox.valueType == PadValueType.Button)
                        return xboxKeycodeTranslateDict[actionData[action].xbox.key];
                    else
                    {
                        switch (actionData[action].xbox.axisName)
                        {
                            case "LeftTrigger_Xbox":
                                return "LT";
                            case "RightTrigger_Xbox":
                                return "RT";
                            default:
                                return "ERROR";
                        }
                    }
                }
            default:
                return actionData[action].keyboard.key.ToString();
        }
    }

    public string TranslateKeycode(KeyCode keyCode,InputType inputType)
    {
        if(inputType == InputType.DualShock)
        {
            return dualShockKeycodeTranslateDict[keyCode];
        }
        else
        {
            return xboxKeycodeTranslateDict[keyCode];
        }
    }

    public bool GetBindingIsToggle(KeybindingActions action)
    {
        return actionData[action].isToggle;
    }

    public void ChangeKeyBindings(KeybindingActions action, KeyCode keycode, InputType inputType)
    {
        foreach (var keybinding in  keyBindingsToggle.keybindingChecks)
        {
            if (keybinding.action == action)
            {
                switch (inputType)
                {
                    case InputType.Keyboard:
                        keybinding.keyboard.key = keycode;
                        return;
                    case InputType.DualShock:
                        keybinding.dualshock.valueType = PadValueType.Button;
                        keybinding.dualshock.key = keycode;
                        return;
                    case InputType.XboxPad:
                        keybinding.xbox.valueType = PadValueType.Button;
                        keybinding.xbox.key = keycode;
                        return;
                }
            }
        }
    }

    public void ChangeKeyBindings(KeybindingActions action, string axisName, InputType inputType)
    {
        foreach (var keybinding in keyBindingsToggle.keybindingChecks)
        {
            if (keybinding.action == action)
            {
                switch (inputType)
                {
                    case InputType.Keyboard:
                        return;
                    case InputType.DualShock:
                        keybinding.dualshock.valueType = PadValueType.Axis;
                        keybinding.dualshock.axisName = axisName;
                        return;
                    case InputType.XboxPad:
                        keybinding.xbox.valueType = PadValueType.Axis;
                        keybinding.xbox.axisName = axisName;
                        return;
                }
            }
        }
    }

    public void SetKeyToggle(KeybindingActions action, bool result)
    {
        foreach (var keybinding in  keyBindingsToggle.keybindingChecks)
        {
            if (keybinding.action == action)
            {
                keybinding.isToggle = result;
                break;
            }
        }
    }

    public float GetMoveAxisVertical()
    {
        if (inputBlock == true)
            return 0.0f;

        return Input.GetAxis("Vertical");
    }

    public float GetMoveAxisHorizontal()
    {
        if (inputBlock == true)
            return 0.0f;

        return Input.GetAxis("Horizontal");
    }

    public float GetCameraAxisX()
    {
        if (inputBlock == true)
            return 0.0f;

        if (currentConnectGamepad == ConnectGamePad.None)
            return Input.GetAxis("Mouse X");
        else if (currentConnectGamepad == ConnectGamePad.DualShock)
            return Input.GetAxis("Mouse X") + Input.GetAxis("RightStickX_DualShock");
        else
            return Input.GetAxis("Mouse X") + Input.GetAxis("RightStickX_Xbox");
    }

    public float GetCameraAxisY()
    {
        if (inputBlock == true)
            return 0.0f;

        if (currentConnectGamepad == ConnectGamePad.None)
            return Input.GetAxis("Mouse Y");
        else if(currentConnectGamepad == ConnectGamePad.DualShock)
            return Input.GetAxis("Mouse Y") + Input.GetAxis("RightStickY_DualShock");
        else
            return Input.GetAxis("Mouse Y") + Input.GetAxis("RightStickY_Xbox");
    }

    public bool GetAdditionalKey(AdditionalType type)
    {
        switch(currentConnectGamepad)
        {
            case ConnectGamePad.None:
                {
                    switch(type)
                    {
                        case AdditionalType.First:
                            return Input.GetKeyDown(KeyCode.Alpha1);
                        case AdditionalType.Second:
                            return Input.GetKeyDown(KeyCode.Alpha2);
                        case AdditionalType.Third:
                            return Input.GetKeyDown(KeyCode.Alpha3);
                        case AdditionalType.Forth:
                            return Input.GetKeyDown(KeyCode.Alpha4);
                    }
                }
                break;
            case ConnectGamePad.DualShock:
                {
                    switch (type)
                    {
                        case AdditionalType.First:
                            if (Input.GetAxis("DpadY_DualShock") == 1.0f && additionaFlag[0] == false)
                            {
                                additionaFlag[0] = true;
                                return true;
                            }
                            break; 
                        case AdditionalType.Second:
                            if (Input.GetAxis("DpadX_DualShock") == -1.0f && additionaFlag[1] == false)
                            {
                                additionaFlag[1] = true;
                                return true;
                            }
                            break;
                        case AdditionalType.Third:
                            if (Input.GetAxis("DpadY_DualShock") == -1.0f && additionaFlag[2] == false)
                            {
                                additionaFlag[2] = true;
                                return true;
                            }
                            break;
                        case AdditionalType.Forth:
                            if (Input.GetAxis("DpadX_DualShock") == 1.0f && additionaFlag[3] == false)
                            {
                                additionaFlag[3] = true;
                                return true;
                            }
                            break;
                    }
                }
                break;
            case ConnectGamePad.XboxPad:
                {
                    switch (type)
                    {
                        case AdditionalType.First:
                            if (Input.GetAxis("DpadY_Xbox") == 1.0f && additionaFlag[0] == false)
                            {
                                additionaFlag[0] = true;
                                return true;
                            }
                            break;
                        case AdditionalType.Second:
                            if (Input.GetAxis("DpadX_Xbox") == -1.0f && additionaFlag[1] == false)
                            {
                                additionaFlag[1] = true;
                                return true;
                            }
                            break;
                        case AdditionalType.Third:
                            if (Input.GetAxis("DpadY_Xbox") == -1.0f && additionaFlag[2] == false)
                            {
                                additionaFlag[2] = true;
                                return true;
                            }
                            break;
                        case AdditionalType.Forth:
                            if (Input.GetAxis("DpadX_Xbox") == 1.0f && additionaFlag[3] == false)
                            {
                                additionaFlag[3] = true;
                                return true;
                            }
                            break;
                    }
                }
                break;
        }

        return false;
    }

    private void CheckAddtional()
    {
        switch(currentConnectGamepad)
        {
            case ConnectGamePad.None:
                return;
            case ConnectGamePad.XboxPad:
                {
                    if(additionaFlag[0] == true && Input.GetAxis("DpadY_DualShock") != 1.0f)
                    {
                        additionaFlag[0] = false;
                    }
                    if (additionaFlag[1] == true && Input.GetAxis("DpadX_DualShock") != 1.0f)
                    {
                        additionaFlag[1] = false;
                    }
                    if (additionaFlag[2] == true && Input.GetAxis("DpadY_DualShock") != 1.0f)
                    {
                        additionaFlag[2] = false;
                    }
                    if (additionaFlag[3] == true && Input.GetAxis("DpadX_DualShock") != 1.0f)
                    {
                        additionaFlag[3] = false;
                    }
                }
                break;
            case ConnectGamePad.DualShock:
                {
                    if (additionaFlag[0] == true && Input.GetAxis("DpadY_Xbox") != 1.0f)
                    {
                        additionaFlag[0] = false;
                    }
                    if (additionaFlag[1] == true && Input.GetAxis("DpadX_Xbox") != 1.0f)
                    {
                        additionaFlag[1] = false;
                    }
                    if (additionaFlag[2] == true && Input.GetAxis("DpadY_Xbox") != 1.0f)
                    {
                        additionaFlag[2] = false;
                    }
                    if (additionaFlag[3] == true && Input.GetAxis("DpadX_Xbox") != 1.0f)
                    {
                        additionaFlag[3] = false;
                    }
                }
                break;
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
 
    private bool BindDualShock_AxisDown(KeybindingActions action)
    {
        bool downFlag = dualShockAxisDownFlag[action];

        if (downFlag == true)
            return false;

        if (Input.GetAxis(actionData[action].dualshock.axisName) != 1.0f)
            return false;
        
        dualShockAxisDownFlag[action] = true;
        dualShockReleaseList.Add(action);
        return true;
    }

    private bool BindDualShock_AxisUp(KeybindingActions action)
    {
        if (dualShockAxisUpFlag[action] == false)
            return false;

        if (Input.GetAxis(actionData[action].dualshock.axisName).Equals(0.0f) == false)
            return false;

        dualShockAxisUpFlag[action] = false;
        return true;
    }

    private bool BindDualShock_AxisKeep(KeybindingActions action)
    {
        if (Input.GetAxis(actionData[action].dualshock.axisName).Equals(1.0f) == true)
            return true;

        return false;
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
    
    private bool BindXbox_AxisDown(KeybindingActions action)
    {
        bool downFlag = xboxAxisDownFlag[action];

        if (downFlag == true)
            return false;

        if (Input.GetAxis(actionData[action].xbox.axisName).Equals( 1.0f) == false)
            return false;
        
        xboxAxisDownFlag[action] = true;
        xboxReleaseList.Add(action);
        return true;
    }

    private bool BindXbox_AxisUp(KeybindingActions action)
    {
        if (xboxAxisUpFlag[action] == false)
            return false;

        if (Input.GetAxis(actionData[action].xbox.axisName).Equals(0.0f) == false)
            return false;

        //xboxAxisDownFlag[action] = false;
        xboxAxisUpFlag[action] = false;
        return true;
    }

    private bool BindXboxShock_AxisKeep(KeybindingActions action)
    {
        if (Input.GetAxis(actionData[action].xbox.axisName).Equals(1.0f) == true)
            return true;

        return false;
    }
    #endregion

    #region 엑박 패드 진동
    public void GamePadSetVibrate(float time, float power)
    {
        //if (controlMode != ControlMode.XboxPad)
        //    return;

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

    private void DetectConnectGamePad()
    {
        string[] connected = Input.GetJoystickNames();

        if (connected.Length == 0)
        {
            currentConnectGamepad = ConnectGamePad.None;
            return;
        }

        if (Input.GetJoystickNames()[0] == "Wireless Controller")
        {
            currentConnectGamepad = ConnectGamePad.DualShock;
        }
        else
        {
            currentConnectGamepad = ConnectGamePad.XboxPad;
        }
    }
}
