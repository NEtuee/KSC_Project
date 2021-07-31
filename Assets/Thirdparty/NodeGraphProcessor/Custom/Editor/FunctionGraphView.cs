using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphProcessor;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[System.Serializable]
public class FunctionGraphView : BaseGraphView
{
    FunctionGraph functionGraph;

    public FunctionGraphView(EditorWindow window) : base(window) 
    {
        graphViewChanged += FunctionDeleteCheck;
        initialized += ()=>{
            functionGraph = (graph as FunctionGraph);
            functionGraph.onFunctionListChanged += UpdateSerializedProperties;
        };

        RegisterCallback<DragPerformEvent>(DragPerformedCallback);
        RegisterCallback<DragUpdatedEvent>(DragUpdatedCallback);
    }

	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		BuildStackNodeContextualMenu(evt);
		base.BuildContextualMenu(evt);
	}

	/// <summary>
	/// Add the New Stack entry to the context menu
	/// </summary>
	/// <param name="evt"></param>
	protected void BuildStackNodeContextualMenu(ContextualMenuPopulateEvent evt)
	{
		Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
		evt.menu.AppendAction("New Stack", (e) => AddStackNode(new BaseStackNode(position)), DropdownMenuAction.AlwaysEnabled);
	}

    public void AddFunction(Vector2 position)
    {
        var function = new FunctionGraph.FunctionInfo{
            name = "New Function " + functionGraph.functionID,
            uniqueID = ++functionGraph.functionID,
        };

        var startNode = BaseNode.CreateFromType<FunctionStartNode>(position);
        AddNode(startNode);

        var endNode = BaseNode.CreateFromType<FunctionEndNode>(position + new Vector2(10f,10f));
        AddNode(endNode);

        function.entryNode = startNode;
        function.endNode = endNode;

        function.UpdateNodeTitle();
        functionGraph.AddFunction(function);
        //functionGraph.onFunctionListChanged?.Invoke();
    }

    public void RemoveFunction(FunctionGraph.FunctionInfo function)
    {
        if(function.entryNode != null)
        {
            RemoveNode(function.entryNode);
        }

        if(function.entryNode != null)
        {
            RemoveNode(function?.endNode);
        }
        

        functionGraph.RemoveFunction(function);
    }

    public GraphViewChange FunctionDeleteCheck(GraphViewChange changes)
    {
        if (changes.elementsToRemove != null)
		{
			RegisterCompleteObjectUndo("Remove Graph Elements");

            changes.elementsToRemove.RemoveAll(e => {
                switch(e)
                {
                    case FunctionItemFieldView view:
                        var functionGraph = graph as FunctionGraph;
                        RemoveFunction(view.parameter);
                        UpdateSerializedProperties();
                        return true;
                }

                return false;
            });
        }

        return changes;
    }

    void DragPerformedCallback(DragPerformEvent e)
	{
		var mousePos = (e.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, e.localMousePosition);
		var dragData = DragAndDrop.GetGenericData("DragSelection") as List< ISelectable >;

		// Drag and Drop for elements inside the graph
		if (dragData != null)
		{
			var functionFieldViews = dragData.OfType<FunctionItemFieldView>();
			if (functionFieldViews.Any())
			{
				foreach (var functionFieldView in functionFieldViews)
				{
					RegisterCompleteObjectUndo("Create Parameter Node");
					var funcNode = BaseNode.CreateFromType< FunctionNode >(mousePos);
					//paramNode.parameterGUID = paramFieldView.parameter.guid;
                    funcNode.functionID = functionFieldView.parameter.uniqueID;
					AddNode(funcNode);
				}
			}
		}
    }

    void DragUpdatedCallback(DragUpdatedEvent e)
    {
        var dragData = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;
		var dragObjects = DragAndDrop.objectReferences;
        bool dragging = false;

        if (dragData != null)
        {
            // Handle drag from exposed parameter view
            if (dragData.OfType<FunctionItemFieldView>().Any())
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
