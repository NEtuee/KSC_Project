using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Take Damage To Player")]
public class TakeDamageToPlayerNode : LinearConditionalNode
{
    [Input(name = "Graph Object")]
	public GraphObjectBase obj;

    [Input(name = "Damage"),SerializeField]
	public float damage;


	public override string		name => "Take Damage To Player";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.FloatData>();
        data.value = damage;
	    obj.SendMessageQuick(MessageTitles.playermanager_addDamageToPlayer,
                    UniqueNumberBase.GetSavedNumberStatic("PlayerManager"),data);
	}

}
