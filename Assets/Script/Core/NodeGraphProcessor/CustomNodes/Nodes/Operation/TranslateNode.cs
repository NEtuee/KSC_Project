using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Operations/Translate")]
public class TranslateNode : LinearConditionalNode
{
	[Input(name = "target")]
	public Transform a;

    [Input(name = "Value"),SerializeField]
	public Vector3C b = new Vector3C();


	public override string		name => "Equal";

	protected override void Process()
	{
	    a.position += (Vector3)b;
	}
}