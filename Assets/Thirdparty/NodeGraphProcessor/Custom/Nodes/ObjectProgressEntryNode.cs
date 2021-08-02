using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/ObjectProgressEntry")]
public class ObjectProgressEntryNode : EntryNode
{
    public override string		name => "ObjectProgressEntry";
    public override string entryCode => "Progress";
}
