using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Fixed Progress Entry")]
public class ObjectFixedProgressEntryNode : EntryNode
{
    [Output(name = "DeltaTime")]
    public float deltaTime;
    
    public override string		name => "Object Fixed Progress Entry";
    public override string entryCode => "FixedProgress";
}
