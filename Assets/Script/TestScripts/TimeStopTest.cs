using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeStopTest : MonoBehaviour
{
    public Transform cubeTr;
    


    void Start()
    {
        
    }

    void Update()
    {
        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            Time.timeScale = 0.0f;
        }

        if(Keyboard.current.mKey.wasPressedThisFrame)
        {
            cubeTr.position += new Vector3(1, 0, 0);
        }

        Debug.Log(Time.timeScale);
    }
}
