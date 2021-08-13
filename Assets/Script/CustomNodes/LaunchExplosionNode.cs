using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Custom/Launch Explosion")]
public class LaunchExplosionNode : LinearConditionalNode
{
	public LevelEdit_ExplosionPhysics target;

	public override string		name => "Launch Explosion";

	protected override void Process()
	{
	    target.Launch();
	}
}
