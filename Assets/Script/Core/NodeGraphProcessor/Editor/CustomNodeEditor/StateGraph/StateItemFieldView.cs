using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using System;
using GraphProcessor;

public class StateItemFieldView : BlackboardField
{
    protected BaseGraphView	graphView;
    protected StateGraphView stateGraphView;
    protected StateMachineGraph stateGraph;

	public StateMachineGraph.StateInfo parameter { get; private set; }

	public StateItemFieldView(BaseGraphView graphView, StateMachineGraph.StateInfo param) : base(null, param.name, "state")
	{
		this.graphView = graphView;
		parameter = param;
        stateGraph = graphView.graph as StateMachineGraph;
        stateGraphView = graphView as StateGraphView;

		this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
		// this.Q("icon").AddToClassList("parameter-" + param.shortType);
		// this.Q("icon").visible = true;

		(this.Q("textField") as TextField).RegisterValueChangedCallback((e) => {
			param.name = e.newValue;
			text = e.newValue;
			stateGraph.UpdateStateName(param, e.newValue);
		});
    }

	void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Rename", (a) => OpenTextEditor(), DropdownMenuAction.AlwaysEnabled);
        evt.menu.AppendAction("Delete", (a) => stateGraphView.RemoveState(parameter), DropdownMenuAction.AlwaysEnabled);

        evt.StopPropagation();
    }
}
