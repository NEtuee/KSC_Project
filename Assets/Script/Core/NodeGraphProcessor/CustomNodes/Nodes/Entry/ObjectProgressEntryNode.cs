using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object Progress Entry")]
public class ObjectProgressEntryNode : EntryNode
{
    [Output(name = "DeltaTime")]
    public float deltaTime;
    
    public override string		name => "Object Progress Entry";
    public override string entryCode => "Progress";
}
