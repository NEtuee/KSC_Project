using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterChanger : MonoBehaviour
{
    public int code;
    public int parameterCode;
    public float value;


    public void Change()
    {
        if(code == 0)
        {
            GameManager.Instance.soundManager.SetGlobalParam(parameterCode,value);
        }
        else
        {
            GameManager.Instance.soundManager.SetParam(code,parameterCode,value);
        }
        
    }
}
