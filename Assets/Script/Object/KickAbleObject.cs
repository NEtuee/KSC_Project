using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickAbleObject : UnTransfromObjectBase
{
    private Rigidbody _rigidbody;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.object_kick, (msg) =>
        {
            PlayerUnit player = (PlayerUnit)msg.data;
            _rigidbody.AddForce(player.Transform.forward * 15000.0f, ForceMode.Impulse);
        });

        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }
}
