using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Time/Increase Timer")]
public class TimeCounterIncreaseNode : LinearConditionalNode
{
    [Input(name = "Graph Object")]
	public GraphObjectBase obj;

    [Input(name = "Target"),SerializeField]
    public string target;

	[Input(name = "deltaTime")]
	public float deltaTime;

    [Output(name = "CurrentTime")]
	public float current;

    [Output(name = "Limit")]
	public bool limit;


	public override string		name => "Increase Timer";

	protected override void Process()
	{
	    current = obj.IncreaseTimer(target,deltaTime,out limit);
	}
}