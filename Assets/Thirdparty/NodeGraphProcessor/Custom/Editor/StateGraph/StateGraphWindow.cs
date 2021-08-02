using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;

public class StateGraphWindow : FunctionGraphWindow
{
    StateMachineGraph		stateGraph;
	StateGraphToolbarView	toolbarView;

	[MenuItem("Window/StateGraph")]
	public new static BaseGraphWindow OpenWithTmpGraph()
	{
		var graphWindow = CreateWindow< StateGraphWindow >();

		// When the graph is opened from the window, we don't save the graph to disk
		graphWindow.stateGraph = ScriptableObject.CreateInstance<StateMachineGraph>();
		graphWindow.stateGraph.hideFlags = HideFlags.HideAndDontSave;
		graphWindow.InitializeGraph(graphWindow.stateGraph);

		graphWindow.Show();

		return graphWindow;
	}

	protected override void OnDestroy()
	{
		graphView?.Dispose();
		DestroyImmediate(stateGraph);
	}

	protected override void InitializeWindow(BaseGraph graph)
	{
		titleContent = new GUIContent("State Graph");

		if (graphView == null)
		{
			graphView = new StateGraphView(this);
			toolbarView = new StateGraphToolbarView(graphView);
			graphView.Add(toolbarView);
            graphView.Add(new MiniMapView(graphView));
		}

		rootView.Add(graphView);
	}

	protected override void InitializeGraphView(BaseGraphView view)
	{
		// graphView.OpenPinned< ExposedParameterView >();
		// toolbarView.UpdateButtonStatus();
	}
}
