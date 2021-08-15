using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

public class StateGraphToolbarView : FunctionGraphToolbarView
{
    public StateGraphToolbarView(BaseGraphView graphView) : base(graphView) {}

	protected override void AddButtons()
	{
		base.AddButtons();

        bool processorVisible = graphView.GetPinnedElementStatus<StateItemView>() != Status.Hidden;
	    AddToggle("Show State List", processorVisible, (v) => graphView.ToggleView<StateItemView>());

		bool conditionalProcessorVisible = graphView.GetPinnedElementStatus< ConditionalProcessorView >() != Status.Hidden;
		AddToggle("Show Conditional Processor", conditionalProcessorVisible, (v) => graphView.ToggleView< ConditionalProcessorView>());

		AddButton("Save", () => 
		{
			EditorUtility.SetDirty(graphView.graph);
			AssetDatabase.SaveAssets();
		}, left: false);
	}
}