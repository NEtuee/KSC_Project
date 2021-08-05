using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Trigger Exit Entry")]
public class ObjectTriggerExitEntryNode : EntryNode
{
    [Output(name = "Collider")]
    public Collider collider;

	public override string		name => "Object Trigger Exit Entry";
	public override string entryCode => "TriggerExit";
}
