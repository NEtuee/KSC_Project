using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/ObjectAssignEntry")]
public class ObjectAssignEntryNode : EntryNode
{
    public override string		name => "ObjectAssignEntry";
    public override string entryCode => "Assign";
}
