using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(FunctionStartNode))]
public class FunctionStartView : BaseNodeView
{
	FunctionStartNode endNode;

	public override void Enable()
	{
		endNode = nodeTarget as FunctionStartNode;

        // Create your fields using node's variables and add them to the controlsContainer
		endNode.onTitleChange += UpdateView;
		
		UpdateView();
	}

	void UpdateView()
    {
        title = endNode.title + " Start";
    }
}