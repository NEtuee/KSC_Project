using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicClimbingRegister : ObjectBase
{
    public List<ClimbingLine> dynamicLines = new List<ClimbingLine>();

    public override void Assign()
    {
        base.Assign();

        foreach(var item in dynamicLines)
        {
            var message = MessagePack(MessageTitles.climbingLineManager_registerDynamicLine,
                                    GetSavedNumber("ClimbingLineManager"),item);    
            
            SendMessageEx(message);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void Release()
    {
        base.Release();
        foreach(var item in dynamicLines)
        {
            var message = MessagePack(MessageTitles.climbingLineManager_withdrawDynamicLine,
                                    GetSavedNumber("ClimbingLineManager"),item);    
            
            SendMessageEx(message);
        }
    }
}
