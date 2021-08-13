using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(BooleanNode))]
public class BooleanNodeView : BaseNodeView
{
	public override void Enable(bool fromInspector = false)
    {
        var node = nodeTarget as BooleanNode;

        //controlsContainer.Add(accessorSelector);

        var check = new Toggle{value = node.input};
        check.RegisterValueChangedCallback((x)=>{
            node.input = x.newValue;
        });
        
        titleContainer.Add(check);
        //  Find and remove expand/collapse button
        titleContainer.Remove(titleContainer.Q("title-button-container"));
        //    Remove Port from the #content
        topContainer.parent.Remove(topContainer);
        //    Add Port to the #title
        titleContainer.Add(topContainer);
    }
}