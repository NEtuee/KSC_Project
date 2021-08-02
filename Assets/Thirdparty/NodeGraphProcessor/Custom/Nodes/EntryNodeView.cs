using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(EntryNode))]
public class EntryNodeView : BaseNodeView
{
	public override void Enable()
	{
		var node = nodeTarget as EntryNode;

        // Create your fields using node's variables and add them to the controlsContainer

		controlsContainer.Add(new Label("Hello World !"));
	}
}