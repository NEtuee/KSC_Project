using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(StateProgressNode))]
public class StateProgressNodeView : BaseNodeView
{
	Label stateLabel;
	public override void Enable()
	{
		var node = nodeTarget as StateProgressNode;

		titleContainer.style.backgroundColor = new StyleColor(new Color(0f,0.4f,0.2f));
		stateLabel = new Label(" Progressed State : ");
		contentContainer.Add(stateLabel);

		node.onProcessed += ()=>{
			stateLabel.text = (" Progressed State : " + (node.stateInfo == null ? "-null-" : node.stateInfo.name));
		};

        // Create your fields using node's variables and add them to the controlsContainer

//		controlsContainer.Add(new Label("Hello World !"));
		//container.Remove(contentContainer);
//		Debug.Log(contentContainer.parent.name);
		//contentContainer.parent.Remove(contentContainer);
	}
}