using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Custom/Set 3D Text")]
public class Set3DTextNode : LinearConditionalNode
{
	[Input(name = "Text Mesh")]
    public TextMesh                target;

	[Input(name = "Text")]
    public string                text;


	public override string		name => "Set 3D Text";

	protected override void Process()
	{
	    target.text = text;
	}
}
