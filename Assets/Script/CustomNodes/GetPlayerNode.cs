using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Get/Get : Player")]
public class GetPlayerNode : BaseNode
{
    [Input(name = "Graph Object")]
	public GraphObjectBase obj;

    [Output(name = "Player")]
	public PlayerUnit player;


	public override string		name => "Get Player";

	protected override void Process()
	{
	    obj.SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,UniqueNumberBase.GetSavedNumberStatic("PlayerManager"),null);

        var msg = obj.DequeueGraphMessage();
        if(msg != null)
        {
            player = (PlayerUnit)msg.data;
            MessagePool.ReturnMessage(msg);
        }
	}

}
