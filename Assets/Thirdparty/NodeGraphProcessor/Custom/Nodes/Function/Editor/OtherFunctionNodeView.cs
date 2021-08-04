using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(OtherFunctionNodeView))]
public class OtherFunctionNodeView : BaseNodeView
{
	public override void Enable()
	{
		var node = nodeTarget as OtherFunctionNode;

        // Create your fields using node's variables and add them to the controlsContainer

		title = node.functionInfo.name;

		titleContainer.style.backgroundColor = new StyleColor(new Color(.5f,0.2f,0f));
		node.functionInfo.onNameChanged += (x)=>{title = x;};
	}
}