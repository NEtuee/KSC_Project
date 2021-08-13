using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Sound Play")]
public class SoundPlayNode : LinearConditionalNode
{
    [Input(name = "Sound Code"),SerializeField]
	public int code;

    [Input(name = "Parent")]
	public Transform parent = null;

    [Input(name = "Position"),SerializeField]
	public Vector3C soundPosition;




	public override string		name => "Sound Play";

	protected override void Process()
	{
        if(parent != null)
        {
            var data = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>();

            data.id = code;
            data.localPosition = (Vector3)soundPosition;
            data.parent = parent;
            data.returnValue = false;

            var msg = MessagePool.GetMessage();
            msg.title = MessageTitles.fmod_attachPlay;
            msg.data = data;
            msg.target = UniqueNumberBase.GetSavedNumberStatic("FMODManager");
            msg.sender = null;

            MasterManager.instance.HandleMessage(msg);
        }
        else
        {
            var data = MessageDataPooling.GetMessageData<MD.SoundPlayData>();

            data.id = code;
            data.position = (Vector3)soundPosition;
            data.dontStop = false;
            data.returnValue = false;

            var msg = MessagePool.GetMessage();
            msg.title = MessageTitles.fmod_play;
            msg.data = data;
            msg.target = UniqueNumberBase.GetSavedNumberStatic("FMODManager");
            msg.sender = null;

            MasterManager.instance.HandleMessage(msg);
        }
	}

}