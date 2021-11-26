using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdyBossSkip : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent skipEvent;

    void Update()
    {
        //if ((Mouse.current.rightButton.wasPressedThisFrame && Keyboard.current.leftAltKey.isPressed))
        //{
        //    skipEvent?.Invoke();
        //}
    }
}
