using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(CurrentStateIsNode))]
public class CurrentStateIsNodeView : BaseNodeView
{
	DropdownField dropdown;

	public override void Enable()
	{
		var node = nodeTarget as CurrentStateIsNode;
		UpdateItemList();

        // Create your fields using node's variables and add them to the controlsContainer

		titleContainer.style.backgroundColor = new StyleColor(new Color(0f,0.4f,0.2f));

		node.stateMachineGraph.onStateListChanged += UpdateItemList;
		node.stateMachineGraph.onStateNameChanged += UpdateItemList;
	}



	public void UpdateItemList()
	{
		var node = nodeTarget as CurrentStateIsNode;
		int defaultIndex = 0;
		if(dropdown != null && contentContainer.Contains(dropdown))
		{
			contentContainer.Remove(dropdown);
			defaultIndex = dropdown.index;
		}
		else if(node.stateInfo != null)
		{
			defaultIndex = node.stateMachineGraph.FindStateIndex(node.stateInfo.uniqueID);
		}
		
		defaultIndex = defaultIndex < 0 ? 0 : defaultIndex;

		dropdown = new DropdownField(node.stateMachineGraph.GetStateNames(),defaultIndex,(x)=>{
			if(node.stateMachineGraph.states.Count == 0)
				return x;
			
			node.stateID = node.stateMachineGraph.FindState(x).uniqueID;
			node.GetStateInfo();
			return x;
		},(x)=>{
			return x;
		});

		contentContainer.Add(dropdown);

	}


}