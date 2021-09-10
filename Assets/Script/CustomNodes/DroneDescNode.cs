using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Drone Desc")]
public class DroneDescNode : LinearConditionalNode
{
	[Input(name = "Player")]
    public PlayerUnit                player;

	[Input(name = "Text"),SerializeField]
    public string                text;


	public override string		name => "Drone Desc";

	protected override void Process()
	{
        Debug.Log("Check");
	    //player.GetDrone().DroneTextCall(text);
	}
}
