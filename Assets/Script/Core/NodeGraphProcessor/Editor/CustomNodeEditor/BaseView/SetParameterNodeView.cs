using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(SetParameterNode))]
public class SetParameterNodeView : BaseNodeView
{
	public override void Enable()
	{
		var node = nodeTarget as SetParameterNode;

        // Create your fields using node's variables and add them to the controlsContainer

		node.onParameterChanged += ()=>{
			title = "Set " + node.parameter?.name;
		};

		title = "Set " + node.parameter?.name;
	}
}