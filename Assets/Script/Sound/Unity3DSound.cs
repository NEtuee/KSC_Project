using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity3DSound : ObjectBase
{
    public AudioSource audioSource;
    private Transform _player;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = ((PlayerUnit)x.data).transform;
        });


    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("FMODManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);
        var dir = (transform.position - _player.transform.position).normalized;
        var side = Vector3.Dot(Camera.main.transform.up,Vector3.Cross(Camera.main.transform.forward,dir));

        audioSource.panStereo = side;
    }
}
