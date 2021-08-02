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

public class FunctionItemView : PinnedElementView
{
    protected BaseGraphView	graphView;
    protected FunctionGraphView functionGraphView;

    protected FunctionGraph functionGraph;

	new const string title = " Functions";
    
    readonly string exposedParameterViewStyle = "GraphProcessorStyles/ExposedParameterView";

    List<Rect> blackboardLayouts = new List<Rect>();

    public FunctionItemView()
    {
        var style = Resources.Load<StyleSheet>(exposedParameterViewStyle);
        if (style != null)
            styleSheets.Add(style);
    }

    protected virtual void OnAddClicked()
    {
        functionGraphView.AddFunction(Vector2.zero);
    }

    protected string GetUniqueExposedPropertyName(string name)
    {
        // Generate unique name
        string uniqueName = name;
        int i = 0;
        while (graphView.graph.exposedParameters.Any(e => e.name == name))
            name = uniqueName + " " + i++;
        return name;
    }

    protected virtual IEnumerable< Type > GetExposedParameterTypes()
    {
        foreach (var type in TypeCache.GetTypesDerivedFrom<ExposedParameter>())
        {
            if (type.IsGenericType)
                continue ;

            yield return type;
        }
    }

    protected virtual void UpdateParameterList()
    {
        content.Clear();

        if(graphView.graph == null)
            Debug.Log("?");

        foreach (var param in functionGraph.functions)
        {
            if(param.hideInList)
                continue;
                
            var row = new BlackboardRow(new FunctionItemFieldView(graphView, param), null);
            // row.expanded = param.settings.expanded;
            // row.RegisterCallback<GeometryChangedEvent>(e => {
            //     param.settings.expanded = row.expanded;
            // });

            content.Add(row);
        }
    }

    protected override void Initialize(BaseGraphView graphView)
    {
		this.graphView = graphView;
		base.title = title;
		scrollable = true;

        //graphView.onExposedParameterListChanged += UpdateParameterList;
        graphView.initialized += UpdateParameterList;
        Undo.undoRedoPerformed += UpdateParameterList;

        RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
        RegisterCallback<MouseDownEvent>(OnMouseDownEvent, TrickleDown.TrickleDown);
        RegisterCallback<DetachFromPanelEvent>(OnViewClosed);

        functionGraph = graphView.graph as FunctionGraph;
        functionGraphView = graphView as FunctionGraphView;

        functionGraph.onFunctionListChanged += UpdateParameterList;

        UpdateParameterList();

        // Add exposed parameter button
        header.Add(new Button(OnAddClicked){
            text = "+"
        });
    }

    void OnViewClosed(DetachFromPanelEvent evt)
        => Undo.undoRedoPerformed -= UpdateParameterList;

    void OnMouseDownEvent(MouseDownEvent evt)
    {
        blackboardLayouts = content.Children().Select(c => c.layout).ToList();
    }

    int GetInsertIndexFromMousePosition(Vector2 pos)
    {
        pos = content.WorldToLocal(pos);
        // We only need to look for y axis;
        float mousePos = pos.y;

        if (mousePos < 0)
            return 0;

        int index = 0;
        foreach (var layout in blackboardLayouts)
        {
            if (mousePos > layout.yMin && mousePos < layout.yMax)
                return index + 1;
            index++;
        }

        return content.childCount;
    }

    void OnDragUpdatedEvent(DragUpdatedEvent evt)
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        int newIndex = GetInsertIndexFromMousePosition(evt.mousePosition);
        var graphSelectionDragData = DragAndDrop.GetGenericData("DragSelection");

        if (graphSelectionDragData == null)
            return;

        foreach (var obj in graphSelectionDragData as List<ISelectable>)
        {
            if (obj is FunctionItemFieldView view)
            {
                var blackBoardRow = view.parent.parent.parent.parent.parent.parent;
                int oldIndex = content.Children().ToList().FindIndex(c => c == blackBoardRow);
                // Try to find the blackboard row
                content.Remove(blackBoardRow);

                if (newIndex > oldIndex)
                    newIndex--;

                content.Insert(newIndex, blackBoardRow);
            }
        }
    }

    void OnDragPerformEvent(DragPerformEvent evt)
    {
        Debug.Log("draged");
        evt.StopImmediatePropagation();
        // bool updateList = false;

        // int newIndex = GetInsertIndexFromMousePosition(evt.mousePosition);
        // foreach (var obj in DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>)
        // {
        //     if (obj is FunctionItemFieldView view)
        //     {
        //         if (!updateList)
        //             graphView.RegisterCompleteObjectUndo("Moved parameters");

        //         int oldIndex = graphView.graph.exposedParameters.FindIndex(e => e == view.parameter);
        //         var parameter = graphView.graph.exposedParameters[oldIndex];
        //         graphView.graph.exposedParameters.RemoveAt(oldIndex);

        //         // Patch new index after the remove operation:
        //         if (newIndex > oldIndex)
        //             newIndex--;

        //         graphView.graph.exposedParameters.Insert(newIndex, parameter);

        //         updateList = true;
        //     }
        // }

        // if (updateList)
        // {
        //     evt.StopImmediatePropagation();
        //     UpdateParameterList();
        // }
    }
    
}
