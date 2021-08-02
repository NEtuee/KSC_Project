using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEngine.UIElements;

[CustomEditor(typeof(FunctionGraph), true)]
public class FunctionGraphAssetInspector : GraphInspector
{
    protected override void CreateInspector()
	{
		base.CreateInspector();

		root.Add(new Button(() => EditorWindow.GetWindow<FunctionGraphWindow>().InitializeGraph(target as FunctionGraph))
		{
			text = "Open function graph window"
		});
	}
}
