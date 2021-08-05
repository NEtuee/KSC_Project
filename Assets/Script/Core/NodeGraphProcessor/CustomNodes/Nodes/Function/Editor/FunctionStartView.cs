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
		
		titleContainer.style.backgroundColor = new StyleColor(new Color(.5f,0.2f,0f));
		UpdateView();
	}

	void UpdateView()
    {
        title = endNode.title + " Start";
    }
}