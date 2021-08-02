using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/ObjectAfterProgressEntry")]
public class ObjectAfterProgressEntryNode : EntryNode
{
    public override string		name => "ObjectAfterProgressEntry";
    public override string entryCode => "AfterProgress";
}
