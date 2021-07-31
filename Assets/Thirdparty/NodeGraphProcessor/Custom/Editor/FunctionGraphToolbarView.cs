using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

public class FunctionGraphToolbarView : ToolbarView
{
    public FunctionGraphToolbarView(BaseGraphView graphView) : base(graphView) {}

	protected override void AddButtons()
	{
		base.AddButtons();

        bool processorVisible = graphView.GetPinnedElementStatus<FunctionItemView>() != Status.Hidden;
	    AddToggle("Show Function List", processorVisible, (v) => graphView.ToggleView<FunctionItemView>());

		bool conditionalProcessorVisible = graphView.GetPinnedElementStatus< ConditionalProcessorView >() != Status.Hidden;
		AddToggle("Show Conditional Processor", conditionalProcessorVisible, (v) => graphView.ToggleView< ConditionalProcessorView>());
	}
}
