using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Time/Initialize Timer")]
public class TimeInitializeNode : LinearConditionalNode
{
    [Input(name = "Graph Object")]
	public GraphObjectBase obj;

    [Input(name = "Target"),SerializeField]
    public string target;


    [Input(name = "Time Limit"),SerializeField]
	public float limit;

	public override string		name => "Initialize Timer";

	protected override void Process()
	{
        obj.InitTimer(target,limit);
	}
}