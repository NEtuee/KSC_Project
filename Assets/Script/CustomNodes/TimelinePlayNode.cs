using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Play Timeline")]
public class TimelinePlayNode : LinearConditionalNode
{
    [Input(name = "Timeline Player")]
	public LevelEdit_TimelinePlayer obj;

	public override string		name => "Play Timeline";

	protected override void Process()
	{
        obj.Play();
	}

}

[System.Serializable, NodeMenuItem("Reference/Timeline Player")]
public class TimelinePlayerNode : BaseNode, ICreateNodeFrom<LevelEdit_TimelinePlayer>
{
	[Output(name = "Out"), SerializeField]
	public LevelEdit_TimelinePlayer			output;

	public override string		name => "Timeline Player";

	public bool InitializeNodeFromObject(LevelEdit_TimelinePlayer value)
	{
		output = value;
		return true;
	}
}