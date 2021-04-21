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
            toggleKeyText.text = InputManager.Instance.GetBindingKeycode(action, inputType).ToString();
        }
        else
        {
            holdKeyText.text = InputManager.Instance.GetBindingKeycode(action, inputType).ToString();
            toggleKeyText.text = "";
        }
    }

    void Update()
    {
        if (waitHoldInput == false && waitToggleInput == false)
            return;

        if (Input.anyKey)
        {
            foreach (KeyCode inputKeycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(inputKeycode))
                {
                    KeyCode inputKey = KeyCode.None;
                    inputKey = inputKeycode;
                    if (waitHoldInput == true)
                        holdKeyText.text = inputKey.ToString();
                    else
                    {
                        toggleKeyText.text = inputKey.ToString();
                        InputManager.Instance.SetKeyToggle(action, true);
                    }

                    waitHoldInput = false;
                    waitToggleInput = false;
                    InputManager.Instance.ChangeKeyBindings(action, inputKey, InputType.Keyboard);
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
