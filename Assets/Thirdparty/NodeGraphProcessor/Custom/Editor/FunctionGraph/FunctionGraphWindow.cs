using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;


public class FunctionGraphWindow : BaseGraphWindow
{
    FunctionGraph		functionGraph;
	FunctionGraphToolbarView	toolbarView;

	[MenuItem("Window/FunctionGraph")]
	public static BaseGraphWindow OpenWithTmpGraph()
	{
		var graphWindow = CreateWindow< FunctionGraphWindow >();

		// When the graph is opened from the window, we don't save the graph to disk
		graphWindow.functionGraph = ScriptableObject.CreateInstance<FunctionGraph>();
		graphWindow.functionGraph.hideFlags = HideFlags.HideAndDontSave;
		graphWindow.InitializeGraph(graphWindow.functionGraph);

		graphWindow.Show();

		return graphWindow;
	}

	protected override void OnDestroy()
	{
		graphView?.Dispose();
		DestroyImmediate(functionGraph);
	}

	protected override void InitializeWindow(BaseGraph graph)
	{
		titleContent = new GUIContent("Function Graph");

		if (graphView == null)
		{
			graphView = new FunctionGraphView(this);
			toolbarView = new FunctionGraphToolbarView(graphView);
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
