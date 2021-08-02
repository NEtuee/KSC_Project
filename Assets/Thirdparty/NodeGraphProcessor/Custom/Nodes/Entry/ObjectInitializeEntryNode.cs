using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/ObjectInitializeEntry")]
public class ObjectInitializeEntryNode : EntryNode
{
	public override string		name => "ObjectInitializeEntry";
	public override string entryCode => "Initialize";
}
