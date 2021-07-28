using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

public class TimeStopTest : MonoBehaviour
{
    public Transform cubeTr;
    public Image image;


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

        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            image.fillAmount = 0.1f;
        }

        Debug.Log(Time.timeScale);
    }
}
