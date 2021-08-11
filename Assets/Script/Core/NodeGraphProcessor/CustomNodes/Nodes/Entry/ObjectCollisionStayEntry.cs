using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Collision Stay Entry")]
public class ObjectCollisionStayEntryNode : EntryNode
{
    [Output(name = "Collision")]
    public Collision collision;

	public override string		name => "Object Collision Stay Entry";
	public override string entryCode => "CollisionStay";
}
