using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger :  ObjectBase
{
    public UnityEngine.Events.UnityEvent whenNormalHit;
    public UnityEngine.Events.UnityEvent whenEMPHit;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.player_EMPHit,(x)=>{
            whenEMPHit?.Invoke();
        });

        AddAction(MessageTitles.player_NormalHit,(x)=>{
            whenNormalHit?.Invoke();
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }
}
