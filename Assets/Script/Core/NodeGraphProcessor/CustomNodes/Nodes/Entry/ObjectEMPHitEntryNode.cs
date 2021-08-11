using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object EMPHit Entry")]
public class ObjectEMPHitEntryNode : EntryNode
{
    [Output(name = "Damage")]
    public float damage;

	public override string		name => "Object EMPHit Entry";
	public override string entryCode => "EMPHit";
}
