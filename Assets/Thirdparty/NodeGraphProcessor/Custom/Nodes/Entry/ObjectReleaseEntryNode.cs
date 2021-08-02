using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/ObjectReleaseEntry")]
public class ObjectReleaseEntryNode : EntryNode
{
    public override string		name => "ObjectReleaseEntry";
    public override string entryCode => "Release";
}

