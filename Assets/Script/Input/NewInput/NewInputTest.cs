using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputTest : MonoBehaviour
{
    public void Run(InputAction.CallbackContext value)
    {
        if(value.performed)
        Debug.Log("Run");
    }

    public void Shot(InputAction.CallbackContext value)
    {
        if (value.performed)
            Debug.Log("Shot");
    }

    public void Grab(InputAction.CallbackContext value)
    {
        if (value.performed)
            Debug.Log("Grab");
    }

    public void EmpAim(InputAction.CallbackContext value)
    {
        if (value.performed)
            Debug.Log("EmpAim");
    }

    public void Scan(InputAction.CallbackContext value)
    {
        if (value.performed)
            Debug.Log("Scan");
    }

    public void UseHpPack(InputAction.CallbackContext value)
    {
        if (value.performed)
            Debug.Log("UseHpPack");
    }

    public void Jump(InputAction.CallbackContext value)
    {
        if (value.performed)
            Debug.Log("Jump");
    }
}
