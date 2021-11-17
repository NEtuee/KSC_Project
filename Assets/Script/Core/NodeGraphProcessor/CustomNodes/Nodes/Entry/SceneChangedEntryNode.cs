using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Entry/Scene Changed Entry")]
public class SceneChangedEntryNode : EntryNode
{

    public override string name => "Scene Changed Entry";
    public override string entryCode => "SceneChanged";
}
