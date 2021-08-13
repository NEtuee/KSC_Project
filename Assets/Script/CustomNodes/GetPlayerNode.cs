using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Get/Get : Player")]
public class GetPlayerNode : LinearConditionalNode
{
    [Input(name = "Graph Object")]
	public GraphObjectBase obj;

    [Output(name = "Player")]
	public PlayerCtrl_Ver2 player;


	public override string		name => "Get Player";

	protected override void Process()
	{
	    obj.SendMessageQuick(MessageTitles.playermanager_getPlayer,UniqueNumberBase.GetSavedNumberStatic("PlayerManager"),null);

        var msg = obj.DequeueGraphMessage();
        if(msg != null)
        {
            player = (PlayerCtrl_Ver2)msg.data;
            MessagePool.ReturnMessage(msg);
        }
	}

}
