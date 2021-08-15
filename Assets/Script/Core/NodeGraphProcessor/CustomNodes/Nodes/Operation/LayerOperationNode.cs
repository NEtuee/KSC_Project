using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Layer : Compare")]
public class LayerEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public int a;

    [Input(name = "B"),SerializeField]
	public int b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Layer Compare";

	protected override void Process()
	{
	    result = ((1 << a) & (1 << b)) != 0;
	}
}

[System.Serializable, NodeMenuItem("Operations/Layer Check")]
public class LayerCheckNode : BaseNode
{
	[Input(name = "GameObject")]
	public GameObject a;

    [SerializeField]
	public LayerMask layer;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Layer Check";

	protected override void Process()
	{
		result = ((a.layer) & (1 << layer.value)) != 0;
	    //result = ((1 << a) & (1 << b)) != 0;
	}
}