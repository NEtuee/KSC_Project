using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Object After Progress Entry")]
public class ObjectAfterProgressEntryNode : EntryNode
{
    public override string		name => "Object After Progress Entry";
    public override string entryCode => "AfterProgress";
}
