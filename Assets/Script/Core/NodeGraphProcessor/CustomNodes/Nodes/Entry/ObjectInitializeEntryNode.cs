using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Initialize Entry")]
public class ObjectInitializeEntryNode : EntryNode
{
	public override string		name => "Object Initialize Entry";
	public override string entryCode => "Initialize";
}
