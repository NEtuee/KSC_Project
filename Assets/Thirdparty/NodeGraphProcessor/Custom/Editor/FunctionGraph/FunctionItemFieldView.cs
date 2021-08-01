using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using GraphProcessor;

public class FunctionItemFieldView : BlackboardField
{
    protected BaseGraphView	graphView;
    protected FunctionGraphView functionGraphView;
    protected FunctionGraph functionGraph;

	public FunctionGraph.FunctionInfo parameter { get; private set; }

	public FunctionItemFieldView(BaseGraphView graphView, FunctionGraph.FunctionInfo param) : base(null, param.name, "function")
	{
		this.graphView = graphView;
		parameter = param;
        functionGraph = graphView.graph as FunctionGraph;
        functionGraphView = graphView as FunctionGraphView;

		this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
		// this.Q("icon").AddToClassList("parameter-" + param.shortType);
		// this.Q("icon").visible = true;

		(this.Q("textField") as TextField).RegisterValueChangedCallback((e) => {
			param.name = e.newValue;
			text = e.newValue;
			functionGraph.UpdateFunctionName(param, e.newValue);
		});
    }

	void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Rename", (a) => OpenTextEditor(), DropdownMenuAction.AlwaysEnabled);
        evt.menu.AppendAction("Delete", (a) => functionGraphView.RemoveFunction(parameter), DropdownMenuAction.AlwaysEnabled);

        evt.StopPropagation();
    }
}
