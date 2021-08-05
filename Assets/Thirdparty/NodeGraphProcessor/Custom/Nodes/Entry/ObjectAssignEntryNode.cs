using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/ObjectAssignEntry")]
public class ObjectAssignEntryNode : EntryNode
{
    public override string		name => "Object Assign Entry";
    public override string entryCode => "Assign";
}
