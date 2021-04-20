using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KeyInputChangeTest : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI keyText;
    public bool waitInput;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waitInput == false)
            return;

        if (Input.anyKey)
        {
            foreach (KeyCode inputKeycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(inputKeycode))
                {
                    KeyCode inputKey = KeyCode.None;
                    inputKey = inputKeycode;
                    keyText.text = inputKey.ToString();
                    waitInput = false;
                    break;
                }
            }
        }
    }

    private void StartSetting()
    {
        keyText.text = "";
        waitInput = true;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        StartSetting();
    }
}
