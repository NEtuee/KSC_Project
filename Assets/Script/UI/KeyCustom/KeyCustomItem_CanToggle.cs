using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyCustomItem_CanToggle : KeyCustomItem
{
    public TextMeshProUGUI holdKeyText;
    public KeyInputPanel holdKeyPanel;
    public TextMeshProUGUI toggleKeyText;
    public KeyInputPanel toggleKeyPanel;
    public bool waitHoldInput;
    public bool waitToggleInput;

    void Start()
    {
        if (holdKeyPanel != null)
        {
            holdKeyPanel.whenOnClick += StartHoldSetting;
            toggleKeyPanel.whenOnClick += StartToggleSetting;
        }
    }

    public override void Initialize(InputType inputType)
    {
        if (InputManager.Instance.GetBindingIsToggle(action) == true)
        {
            holdKeyText.text = "";
            toggleKeyText.text = InputManager.Instance.GetBindingKeycode(action, inputType);
        }
        else
        {
            holdKeyText.text = InputManager.Instance.GetBindingKeycode(action, inputType);
            toggleKeyText.text = "";
        }
    }

    void Update()
    {
        if (waitHoldInput == false && waitToggleInput == false)
            return;

        if (inputType == InputType.XboxPad)
        {
            if (Input.GetAxis("LeftTrigger_Xbox") == 1f)
            {
                string keyString = "LT";

                if (waitHoldInput == true)
                {
                    holdKeyText.text = keyString;
                }
                else
                {
                    toggleKeyText.text = keyString;
                    InputManager.Instance.SetKeyToggle(action, true);
                }

                waitHoldInput = false;
                InputManager.Instance.ChangeKeyBindings(action, "LeftTrigger_Xbox", inputType);

                return;
            }

            if (Input.GetAxis("RightTrigger_Xbox") == 1f)
            {
                string keyString = "RT";

                if (waitHoldInput == true)
                {
                    holdKeyText.text = keyString;
                }
                else
                {
                    toggleKeyText.text = keyString;
                    InputManager.Instance.SetKeyToggle(action, true);
                }

                waitHoldInput = false;
                InputManager.Instance.ChangeKeyBindings(action, "RightTrigger_Xbox", inputType);
                return;
            }
        }

        if (Input.anyKey)
        {
            foreach (KeyCode inputKeycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(inputKeycode))
                {
                    KeyCode inputKey = KeyCode.None;
                    inputKey = inputKeycode;

                    string keyString = inputType == InputType.Keyboard ? inputKey.ToString() : InputManager.Instance.TranslateKeycode(inputKey, inputType);

                    if (waitHoldInput == true)
                    {
                        holdKeyText.text = keyString;
                    }
                    else
                    {
                        toggleKeyText.text = keyString;
                        InputManager.Instance.SetKeyToggle(action, true);
                    }

                    waitHoldInput = false;
                    waitToggleInput = false;
                    InputManager.Instance.ChangeKeyBindings(action, inputKey, inputType);
                    break;
                }
            }
        }
    }

    private void StartHoldSetting()
    {
        holdKeyText.text = "";
        toggleKeyText.text = "";
        waitHoldInput = true;
        InputManager.Instance.SetKeyToggle(action, false);
    }

    private void StartToggleSetting()
    {
        toggleKeyText.text = "";
        holdKeyText.text = "";
        waitToggleInput = true;
    }
}
