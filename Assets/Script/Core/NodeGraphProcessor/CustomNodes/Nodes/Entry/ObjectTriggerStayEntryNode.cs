using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Trigger Stay Entry")]
public class ObjectTriggerStayEntryNode : EntryNode
{
    [Output(name = "Collider")]
    public Collider collider;

	public override string		name => "Object Trigger Stay Entry";
	public override string entryCode => "TriggerStay";
}
