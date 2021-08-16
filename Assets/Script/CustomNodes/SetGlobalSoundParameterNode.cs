using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Set Global Parameter")]
public class SetGlobalSoundParameterNode : LinearConditionalNode
{
    [Input(name = "ParameterID"),SerializeField]
	public int parameterId;

    [Input(name = "Value"),SerializeField]
	public float value;

	public override string		name => "Set Global Parameter";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
        data.soundId = 0;
        data.paramId = parameterId;
        data.value = value;

        var msg = MessagePool.GetMessage();
        msg.title = MessageTitles.fmod_setGlobalParam;
        msg.data = data;
        msg.target = UniqueNumberBase.GetSavedNumberStatic("FMODManager");
        msg.sender = null;

        MasterManager.instance.HandleMessage(msg);
	}

}