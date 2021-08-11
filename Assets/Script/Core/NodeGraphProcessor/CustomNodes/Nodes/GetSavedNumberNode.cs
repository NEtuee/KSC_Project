using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Object/GetSavedNumber")]
public class GetSavedNumberNode : BaseNode
{
	[Input(name = "Name"),SerializeField]
	public string input;


	[Output(name = "Number")]
	public int output;

	public override string		name => "GetSavedNumber";

	protected override void Process()
	{
	    output = UniqueNumberBase.GetSavedNumberStatic(input);
	}
}
