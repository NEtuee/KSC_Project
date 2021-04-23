using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyCustomItem_CantToggle : KeyCustomItem
{
    public TextMeshProUGUI holdKeyText;
    public KeyInputPanel holdKeyPanel;

    public bool waitHoldInput;
    void Start()
    {
        if (holdKeyPanel != null)
        {
            holdKeyPanel.whenOnClick += StartHoldSetting;
        }
    }

    void Update()
    {
        if (waitHoldInput == false)
            return;

        if (Input.anyKey)
        {
            foreach (KeyCode inputKeycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(inputKeycode))
                {
                    KeyCode inputKey = KeyCode.None;
                    inputKey = inputKeycode;
                    holdKeyText.text = inputKey.ToString();
                    
                    waitHoldInput = false;
                    InputManager.Instance.ChangeKeyBindings(action, inputKey, inputType);
                    break;
                }
            }
        }
    }

    private void StartHoldSetting()
    {
        holdKeyText.text = "";
        waitHoldInput = true;
    }

    public override void Initialize(InputType inputType)
    {
        this.inputType = inputType;
        holdKeyText.text = "";
        holdKeyText.text = InputManager.Instance.GetBindingKeycode(action, inputType).ToString();
    }
}
