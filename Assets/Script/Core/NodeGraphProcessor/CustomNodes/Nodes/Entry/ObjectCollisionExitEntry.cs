using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Collision Exit Entry")]
public class ObjectCollisionExitEntryNode : EntryNode
{
    [Output(name = "Collision")]
    public Collision collision;

	public override string		name => "Object Collision Exit Entry";
	public override string entryCode => "CollisionExit";
}
