using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Effect Active")]
public class EffectActive : LinearConditionalNode
{
    [Input(name = "Code"),SerializeField]
	public string code;

    [Input(name = "Position"),SerializeField]
	public Vector3C targetPosition;



	public override string		name => "Effect Active";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

        data.key = code;
        data.position = (Vector3)targetPosition;
        data.parent = null;
        data.rotation = Quaternion.identity;

        var msg = MessagePool.GetMessage();
        msg.title = MessageTitles.effectmanager_activeeffect;
        msg.data = data;
        msg.target = UniqueNumberBase.GetSavedNumberStatic("EffectManager");
        msg.sender = null;

        MasterManager.instance.HandleMessage(msg);
	}

}