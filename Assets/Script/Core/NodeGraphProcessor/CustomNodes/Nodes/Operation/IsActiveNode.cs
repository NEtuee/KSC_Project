using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/IsActive : GameObject")]
public class IsActiveNode : BaseNode
{
	[Input(name = "GameObject"),SerializeField]
	public GameObject target;

	[Output(name = "Result")]
	public bool result;

	public override string		name => "IsActive";

	protected override void Process()
	{
	    result = target.activeInHierarchy;
	}
}
