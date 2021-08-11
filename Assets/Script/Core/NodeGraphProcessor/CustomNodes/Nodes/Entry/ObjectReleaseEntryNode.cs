using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Release Entry")]
public class ObjectReleaseEntryNode : EntryNode
{
    public override string		name => "Object Release Entry";
    public override string entryCode => "Release";
}

