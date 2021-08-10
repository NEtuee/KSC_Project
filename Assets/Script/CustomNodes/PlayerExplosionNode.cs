using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Player Explosion")]
public class PlayerExplosionNode : LinearConditionalNode
{
    [Input(name = "Player")]
	public PlayerCtrl_Ver2 player;

    [Input(name = "Force"),SerializeField]
	public float force;

    [Input(name = "Direction"),SerializeField]
	public Vector3C direction;


	public override string		name => "Take Damage To Player";

	protected override void Process()
	{
        player.GetPlayerRagdoll()?.ExplosionRagdoll(force,(Vector3)direction);
	}

}
