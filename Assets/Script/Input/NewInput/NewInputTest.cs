using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputTest : MonoBehaviour
{
    public Vector2 move;
    public Vector2 Camera;

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

    public void Move(InputAction.CallbackContext value)
    {
        move = value.ReadValue<Vector2>();
    }

    public void Cam(InputAction.CallbackContext value)
    {
        Camera = value.ReadValue<Vector2>();
    }
}
