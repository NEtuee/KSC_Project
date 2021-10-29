using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundObject : ObjectBase
{
    private Rigidbody rig;

    public override void Assign()
    {
        base.Assign();

        rig = GetComponent<Rigidbody>();

        AddAction(MessageTitles.object_kick,(x)=>{
            var dir = (transform.position - ((Component)x.data).transform.position).normalized;

            rig.isKinematic = false;
            rig.AddForce(dir * 10f,ForceMode.Impulse);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }
}
