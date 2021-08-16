using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEngine.UIElements;

[CustomEditor(typeof(StateMachineGraph), true)]
public class StateGraphAssetInspector : GraphInspector
{
    protected override void CreateInspector()
	{
		base.CreateInspector();

		root.Add(new Button(() => EditorWindow.GetWindow<StateGraphWindow>().InitializeGraph(target as FunctionGraph))
		{
			text = "Open state graph window"
		});

		root.Add(new Button(() => {
			EditorUtility.SetDirty(target);
			AssetDatabase.SaveAssets();
		})
		{
			text = "Save"
		});
	}
}