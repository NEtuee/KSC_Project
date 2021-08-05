using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Trigger Enter Entry")]
public class ObjectTriggerEnterEntryNode : EntryNode
{
    [Output(name = "Collider")]
    public Collider collider;

	public override string		name => "Object Trigger Enter Entry";
	public override string entryCode => "TriggerEnter";
}
