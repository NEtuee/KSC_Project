using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundPlayer : UnTransfromObjectBase
{
    public int code;
    public int parameterCode;
    public float velocityFactor;
    public Rigidbody rig;


    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("PlayerManager"));
    }

    public void OnCollisionEnter(Collision coll)
    {
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
