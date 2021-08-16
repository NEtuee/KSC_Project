using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Custom/EMP Energy Set")]
public class EMPEnergySetNode : LinearConditionalNode
{
	[Input(name = "Player")]
    public PlayerCtrl_Ver2                player;

	[Input(name = "Energy")]
    public float                energy;


	public override string		name => "EMP Energy Set Node";

	protected override void Process()
	{
	    player.energy.Value = energy;
	}
}
