using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Animator/Set Animator Trigger")]
public class SetAnimatorTriggerNode : LinearConditionalNode
{
    [Input(name = "Trigger Name"),SerializeField]
	public string triggerName;

	[Input(name = "Level Object")]
	public GraphObjectBase obj;

	public override string		name => "Set Animator Trigger";

	protected override void Process()
	{
        obj.GetAnimator()?.SetTrigger(triggerName);
	}
}

[System.Serializable, NodeMenuItem("Animator/Set Animator Boolean")]
public class SetAnimatorBooleanNode : LinearConditionalNode
{
    [Input(name = "Boolean Name"),SerializeField]
	public string booleanName;

    [Input(name = "Boolean"),SerializeField]
	public bool boolean;

	[Input(name = "Level Object")]
	public GraphObjectBase obj;

	public override string		name => "Set Animator Boolean";

	protected override void Process()
	{
        obj.GetAnimator()?.SetBool(booleanName,boolean);
	}
}

[System.Serializable, NodeMenuItem("Animator/Get Animator Boolean")]
public class GetAnimatorBooleanNode : BaseNode
{
    [Input(name = "Boolean Name"),SerializeField]
	public string booleanName;

	[Input(name = "Level Object")]
	public GraphObjectBase obj;

    [Output(name = "Boolean")]
	public bool boolean;

	public override string		name => "Set Animator Boolean";

	protected override void Process()
	{
        boolean = obj.GetAnimator() == null ? false : obj.GetAnimator().GetBool(booleanName);
	}
}

[System.Serializable, NodeMenuItem("Animator/Set Animator Layer Weight")]
public class SetAnimatorLayerWeightNode : BaseNode
{
    [Input(name = "LayerIndex"),SerializeField]
	public int index;
    [Input(name = "Weight"),SerializeField]
	public int weight;


	[Input(name = "Level Object")]
	public GraphObjectBase obj;

    [Output(name = "Boolean")]
	public bool boolean;

	public override string		name => "Set Animator Layer Weight";

	protected override void Process()
	{
        obj.GetAnimator()?.SetLayerWeight(index,weight);
	}
}