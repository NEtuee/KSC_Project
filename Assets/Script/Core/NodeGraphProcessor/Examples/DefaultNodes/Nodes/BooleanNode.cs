using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("Primitives/Boolean")]
public class BooleanNode : BaseNode
{
    [Output]
	public bool		output;
	
    [SerializeField]
	public bool		input;

	public override string name => "Boolean";

	protected override void Process() => output = input;

    [CustomPortBehavior(nameof(output))]
	IEnumerable<PortData> GetOutputPort(List<SerializableEdge> edges)
	{
		yield return new PortData
		{
			identifier = "output",
			displayName = "",
			displayType = typeof(bool),
			acceptMultipleEdges = true
		};
	}
}