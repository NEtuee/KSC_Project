using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterChanger : UnTransfromObjectBase
{
    public int code;
    public int parameterCode;
    public float value;

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public void Change()
    {
        if(code == 0)
        {
            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = code;
            data.paramId = parameterCode;
            data.value = value;

            SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);
            //GameManager.Instance.soundManager.SetGlobalParam(parameterCode,value);
        }
        else
        {
            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = code;
            data.paramId = parameterCode;
            data.value = value;

            SendMessageEx(MessageTitles.fmod_setParam,GetSavedNumber("FMODManager"),data);

            //GameManager.Instance.soundManager.SetParam(code,parameterCode,value);
        }
        
    }
}
