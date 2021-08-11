using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Set Material Color")]
public class SetMaterialColorNode : LinearConditionalNode
{
	[Input(name = "MeshRenderer")]
    public MeshRenderer renderer;

	[Input(name = "Color"),SerializeField]
    public Color matColor;

	[Input(name = "Target Name"),SerializeField]
    public string targetName;

	public override string		name => "Set Material Color";

	protected override void Process()
	{
	    renderer.material.SetColor(targetName,matColor);
	}
}
