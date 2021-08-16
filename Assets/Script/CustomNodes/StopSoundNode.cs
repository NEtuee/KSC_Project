using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Stop Sound")]
public class StopSoundNode : LinearConditionalNode
{
    [Input(name = "SoundEmitter")]
	public FMODUnity.StudioEventEmitter emitter;

    [Input(name = "Fadeout"),SerializeField]
	public bool fade;

	public override string		name => "Stop Sound";

	protected override void Process()
	{
        emitter.AllowFadeout = fade;
        emitter.Stop();
	}

}

[System.Serializable, NodeMenuItem("Game/Stop All Sound")]
public class StopAllSoundNode : LinearConditionalNode
{
    [Input(name = "Sound ID"),SerializeField]
	public int id;

    [Input(name = "Fadeout"),SerializeField]
	public bool fade;

	public override string		name => "Stop All Sound";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.StopAllSoundData>();
        data.id = id;
        data.fade = fade;

        var msg = MessagePool.GetMessage();
        msg.title = MessageTitles.fmod_stopAll;
        msg.data = data;
        msg.target = UniqueNumberBase.GetSavedNumberStatic("FMODManager");
        msg.sender = null;

        MasterManager.instance.HandleMessage(msg);
	}

}