using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Get/Try Get LevelObject : GameObject")]
public class GetLevelObjectFromGameObjectNode : BaseNode
{
	[Input(name = "GameObject")]
	public GameObject gameObject;


	[Output(name = "LevelObject")]
	public GraphObjectBase levelObject;

    [Output(name = "Result")]
	public bool result;

	public override string		name => "Try Get LevelObject";

	protected override void Process()
	{
	    result = gameObject.TryGetComponent<GraphObjectBase>(out levelObject);
	}
}