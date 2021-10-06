using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadConnectTest : MonoBehaviour
{
    private void Awake()
    {
        InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        Debug.Log("Added");
                        break;
                    case InputDeviceChange.Disconnected:
                        Debug.Log("Disconnected");
                        break;
                    case InputDeviceChange.Reconnected:
                        Debug.Log("Reconnected");
                        break;
                    case InputDeviceChange.Removed:
                        Debug.Log("Removed");
                        break;
                    case InputDeviceChange.UsageChanged:
                        Debug.Log("UsageChanged");
                        break;
                }
            };

        LogCurrent();
    }

    private void Update()
    {
        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            LogCurrent();
        }
    }

    private void LogCurrent()
    {
        var gamepad = Gamepad.current;

        Debug.Log(gamepad);
    }
}
