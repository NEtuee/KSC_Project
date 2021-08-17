using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Scanned Entry")]
public class ScannedEntryNode : EntryNode
{
	public override string		name => "Scanned Entry";
	public override string entryCode => "Scanned";
}
