using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUtil : MonoBehaviour
{
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            Debug.Break();
        }
    }
}
