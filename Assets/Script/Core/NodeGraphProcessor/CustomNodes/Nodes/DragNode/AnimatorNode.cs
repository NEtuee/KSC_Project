using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("Reference/Animator")]
public class AnimatorNode : BaseNode, ICreateNodeFrom<Animator>
{
	[Output(name = "Out"), SerializeField]
	public Animator			output;

	public override string		name => "Animator";

	public bool InitializeNodeFromObject(Animator value)
	{
		output = value;
		return true;
	}
}