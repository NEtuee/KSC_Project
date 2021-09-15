using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickTestObject : UnTransfromObjectBase
{
    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.object_kick, (msg) =>
        {
            Destroy(gameObject);
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }
}
