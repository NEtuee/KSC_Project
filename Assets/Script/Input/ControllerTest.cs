using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTest : MonoBehaviour
{
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;
    [SerializeField] private float input_X;
    [SerializeField] private float input_Y;
    [SerializeField] private float l2;
    [SerializeField] private float r2;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Debug.Log("Square");
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            //Debug.Log("X");
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            Debug.Log("Circle");
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            Debug.Log("Triangle");
        }

        input_X = Input.GetAxis("RightStickX");
        input_Y = Input.GetAxis("RightStickY");
        l2 = Input.GetAxis("L2");
        r2 = Input.GetAxis("R2");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}
