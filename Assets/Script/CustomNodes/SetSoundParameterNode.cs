using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Set Parameter")]
public class SetSoundParameterNode : LinearConditionalNode
{
    [Input(name = "SoundID"),SerializeField]
	public int soundId;

    [Input(name = "ParameterID"),SerializeField]
	public int parameterId;

    [Input(name = "Value"),SerializeField]
	public float value;

	public override string		name => "Set Parameter";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
        data.soundId = soundId;
        data.paramId = parameterId;
        data.value = value;

        var msg = MessagePool.GetMessage();
        msg.title = MessageTitles.fmod_setParam;
        msg.data = data;
        msg.target = UniqueNumberBase.GetSavedNumberStatic("FMODManager");
        msg.sender = null;

        MasterManager.instance.HandleMessage(msg);
	}

}