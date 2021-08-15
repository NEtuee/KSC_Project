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


    [Input(name = "GraphObject")]
	public GraphObjectBase graphObj;



    [Output(name = "SoundEmitter")]
	public FMODUnity.StudioEventEmitter emitter;



	public override string		name => "Sound Play";

	protected override void Process()
	{
        if(parent != null)
        {
            var data = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>();

            data.id = code;
            data.localPosition = (Vector3)soundPosition;
            data.parent = parent;
            data.returnValue = true;

            graphObj.SendMessageQuick(MessageTitles.fmod_attachPlay,UniqueNumberBase.GetSavedNumberStatic("FMODManager"),data);
        }
        else
        {
            var data = MessageDataPooling.GetMessageData<MD.SoundPlayData>();

            data.id = code;
            data.position = (Vector3)soundPosition;
            data.dontStop = false;
            data.returnValue = true;

            graphObj.SendMessageQuick(MessageTitles.fmod_play,UniqueNumberBase.GetSavedNumberStatic("FMODManager"),data);
        }

        var msg = graphObj.DequeueGraphMessage();
        if(msg != null)
        {
            emitter = (FMODUnity.StudioEventEmitter)msg.data;
            MessagePool.ReturnMessage(msg);
        }
	}

}