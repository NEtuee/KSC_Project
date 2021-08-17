using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Set Emission")]
public class SetEmissionNode : LinearConditionalNode
{
	[Input(name = "MeshRenderer"),SerializeField]
    public MeshRenderer renderer;

	[Input(name = "Color"),SerializeField]
    [ColorUsage(true,true)]
    public Color matColor;


	public override string		name => "Set Emission";

	protected override void Process()
	{
	    renderer.material.SetColor("_EmissionColor",matColor);
	}
}
