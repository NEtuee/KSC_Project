using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphProcessor;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class StateGraphView : FunctionGraphView
{
    StateMachineGraph stateGraph;

    public StateGraphView(EditorWindow window) : base(window) 
    {
        graphViewChanged += StateDeleteCheck;
        initialized += ()=>{
            stateGraph = (graph as StateMachineGraph);
            stateGraph.onFunctionListChanged += UpdateSerializedProperties;
        };

        // RegisterCallback<DragPerformEvent>(DragPerformedCallback);
        RegisterCallback<DragUpdatedEvent>(DragUpdatedCallback);
    }

	// public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	// {
	// 	BuildStackNodeContextualMenu(evt);
	// 	base.BuildContextualMenu(evt);
	// }

    public void AddState()
    {
        var state = new StateMachineGraph.StateInfo{
            name = "New State " + stateGraph.stateID,
            uniqueID = ++stateGraph.stateID,
            stateInitialize = AddFunction(Vector2.zero,name,true),
            stateProgress = AddFunction(new Vector2(20f,20f),name,true),
        };

        state.UpdateNodeTitle();
        stateGraph.AddState(state);
        //functionGraph.onFunctionListChanged?.Invoke();
    }

    public void RemoveState(StateMachineGraph.StateInfo state)
    {
        if(state.stateInitialize != null)
        {
            RemoveFunction(state.stateInitialize);
        }

        if(state.stateProgress != null)
        {
            RemoveFunction(state.stateProgress);
        }
        

        stateGraph.RemoveState(state);
    }

    public GraphViewChange StateDeleteCheck(GraphViewChange changes)
    {
        if (changes.elementsToRemove != null)
		{
			RegisterCompleteObjectUndo("Remove Graph Elements");

            changes.elementsToRemove.RemoveAll(e => {
                switch(e)
                {
                    case StateItemFieldView view:
                        var stateGraph = graph as StateMachineGraph;
                        RemoveState(view.parameter);
                        UpdateSerializedProperties();
                        return true;
                }

                return false;
            });
        }

        return changes;
    }

    // void DragPerformedCallback(DragPerformEvent e)
	// {
	// 	var mousePos = (e.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, e.localMousePosition);
	// 	var dragData = DragAndDrop.GetGenericData("DragSelection") as List< ISelectable >;

	// 	// Drag and Drop for elements inside the graph
	// 	if (dragData != null)
	// 	{
	// 		var functionFieldViews = dragData.OfType<FunctionItemFieldView>();
	// 		if (functionFieldViews.Any())
	// 		{
	// 			foreach (var functionFieldView in functionFieldViews)
	// 			{
	// 				RegisterCompleteObjectUndo("Create Parameter Node");
	// 				var funcNode = BaseNode.CreateFromType< FunctionNode >(mousePos);
	// 				//paramNode.parameterGUID = paramFieldView.parameter.guid;
    //                 funcNode.functionID = functionFieldView.parameter.uniqueID;
	// 				AddNode(funcNode);
	// 			}
	// 		}
	// 	}
    // }

    void DragUpdatedCallback(DragUpdatedEvent e)
    {
        var dragData = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;
		var dragObjects = DragAndDrop.objectReferences;
        bool dragging = false;

        if (dragData != null)
        {
            // Handle drag from exposed parameter view
            if (dragData.OfType<StateItemFieldView>().Any())
			{
                dragging = true;
			}
        }

		if (dragObjects.Length > 0)
			dragging = true;

        if (dragging)
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

		UpdateNodeInspectorSelection();
    }

}
