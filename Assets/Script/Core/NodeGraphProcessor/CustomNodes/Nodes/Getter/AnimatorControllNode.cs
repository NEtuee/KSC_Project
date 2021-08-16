using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Animator/Set Animator Trigger : Animator")]
public class SetAnimatorTriggerNode : LinearConditionalNode
{
    [Input(name = "Trigger Name"),SerializeField]
	public string triggerName;

	[Input(name = "Animator")]
	public Animator obj;

	public override string		name => "Set Animator Trigger";

	protected override void Process()
	{
        obj.SetTrigger(triggerName);
	}
}

[System.Serializable, NodeMenuItem("Animator/Set Animator Boolean : Animator")]
public class SetAnimatorBooleanNode : LinearConditionalNode
{
    [Input(name = "Boolean Name"),SerializeField]
	public string booleanName;

    [Input(name = "Boolean"),SerializeField]
	public bool boolean;

	[Input(name = "Animator")]
	public Animator obj;

	public override string		name => "Set Animator Boolean";

	protected override void Process()
	{
        obj.SetBool(booleanName,boolean);
	}
}

[System.Serializable, NodeMenuItem("Animator/Get Animator Boolean : Animator")]
public class GetAnimatorBooleanNode : BaseNode
{
    [Input(name = "Boolean Name"),SerializeField]
	public string booleanName;

	[Input(name = "Animator")]
	public Animator obj;

    [Output(name = "Boolean")]
	public bool boolean;

	public override string		name => "Set Animator Boolean";

	protected override void Process()
	{
        boolean = obj == null ? false : obj.GetBool(booleanName);
	}
}

[System.Serializable, NodeMenuItem("Animator/Set Animator Layer Weight : Animator")]
public class SetAnimatorLayerWeightNode : BaseNode
{
    [Input(name = "LayerIndex"),SerializeField]
	public int index;
    [Input(name = "Weight"),SerializeField]
	public int weight;


	[Input(name = "Animator")]
	public Animator obj;

    [Output(name = "Boolean")]
	public bool boolean;

	public override string		name => "Set Animator Layer Weight";

	protected override void Process()
	{
        obj.SetLayerWeight(index,weight);
	}
}

[System.Serializable, NodeMenuItem("Animator/Set Animator Trigger : Graph Object")]
public class SetAnimatorTriggerFromObjectNode : LinearConditionalNode
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

[System.Serializable, NodeMenuItem("Animator/Set Animator Boolean : Graph Object")]
public class SetAnimatorBooleanFromObjectNode : LinearConditionalNode
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

[System.Serializable, NodeMenuItem("Animator/Get Animator Boolean : Graph Object")]
public class GetAnimatorBooleanFromObjectNode : BaseNode
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

[System.Serializable, NodeMenuItem("Animator/Set Animator Layer Weight : Graph Object")]
public class SetAnimatorLayerWeightFromObjectNode : BaseNode
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