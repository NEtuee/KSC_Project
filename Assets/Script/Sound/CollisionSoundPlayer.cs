using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundPlayer : UnTransfromObjectBase
{
    public int code;
    public int parameterCode;
    public float velocityFactor;
    public Rigidbody rig;

    private bool _frameCheck = false;

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("PlayerManager"));
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);
        _frameCheck = false;
    }

    public void OnCollisionEnter(Collision coll)
    {
        if (_frameCheck)
            return;
        _frameCheck = true;
        MD.AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>(); ;
        soundData.id = code; 
        soundData.localPosition = Vector3.up; 
        soundData.parent = transform; 
        soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
        
        MD.SetParameterData setParameterData = MessageDataPooling.GetMessageData<MD.SetParameterData>();
        setParameterData.soundId = code; 
        setParameterData.paramId = parameterCode; 
        setParameterData.value = rig.velocity.magnitude * velocityFactor;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), setParameterData);

    }
}
