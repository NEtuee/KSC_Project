using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Set Sound Pause")]
public class SetSoundPauseNode : LinearConditionalNode
{
    [Input(name = "Emitter")]
	public FMODUnity.StudioEventEmitter emitter;

    [Input(name = "Pause"),SerializeField]
    public bool pause;


	public override string		name => "Set Sound Pause";

	protected override void Process()
	{
        emitter.EventInstance.setPaused(pause);
	}

}


[System.Serializable, NodeMenuItem("Game/Check Sound Pause")]
public class IsSoundPauseNode : BaseNode
{
    [Input(name = "Emitter")]
	public FMODUnity.StudioEventEmitter emitter;

    [Output(name = "Pause")]
    public bool pause;


	public override string		name => "Check Sound Pause";

	protected override void Process()
	{
        emitter.EventInstance.getPaused(out pause);
	}

}