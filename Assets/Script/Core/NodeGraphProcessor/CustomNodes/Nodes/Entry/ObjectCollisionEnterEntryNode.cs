using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Collision Enter Entry")]
public class ObjectCollisionEnterEntryNode : EntryNode
{
    [Output(name = "Collision")]
    public Collision collision;

	public override string		name => "Object Collision Enter Entry";
	public override string entryCode => "CollisionEnter";
}
