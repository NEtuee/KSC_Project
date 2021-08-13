using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public int code;

    public void Play()
    {
        var sound = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
        sound.id = code;
        sound.position = transform.position;
        sound.dontStop = false;
        sound.returnValue = false;

        var msg = MessagePool.GetMessage();
        msg.Set(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), sound,null);

        MasterManager.instance.HandleMessage(msg);
    }
}
