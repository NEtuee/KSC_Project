using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;


[System.Serializable, NodeMenuItem("Animation/Evaluate")]
public class AnimationCurveEvaluateNode : BaseNode
{
    [Input(name = "Curve")]
	public AnimationCurve curve;
    [Input(name = "Time")]
	public float time;

	[Output(name = "Value")]
	public float value;

	public override string		name => "Evaluate";

	protected override void Process()
	{
	    value = curve.Evaluate(time);
	}
}