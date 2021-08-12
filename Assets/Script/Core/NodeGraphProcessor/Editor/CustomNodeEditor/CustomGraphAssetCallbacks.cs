using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using System.IO;

public class CustomGraphAssetCallbacks
{
	[MenuItem("Assets/Create/GraphProcessor/BaseGraph", false, 10)]
	public static void CreateGraphPorcessor()
	{
		var graph = ScriptableObject.CreateInstance< BaseGraph >();
		ProjectWindowUtil.CreateAsset(graph, "GraphProcessor.asset");
	}

	[MenuItem("Assets/Create/GraphProcessor/FunctionGraph", false, 10)]
	public static void CreateFunctionGraphPorcessor()
	{
		var graph = ScriptableObject.CreateInstance< FunctionGraph >();
		ProjectWindowUtil.CreateAsset(graph, "FunctionGraphProcessor.asset");
	}

	[MenuItem("Assets/Create/GraphProcessor/StateMachineGraph", false, 10)]
	public static void CreateStateGraphPorcessor()
	{
		var graph = ScriptableObject.CreateInstance< StateMachineGraph >();
		ProjectWindowUtil.CreateAsset(graph, "StateGraphProcessor.asset");
	}

	[MenuItem("Assets/Create/GraphProcessor/LevelObjectGraph", false, 10)]
	public static void CreateLevelObjectGraphPorcessor()
	{
		var graph = ScriptableObject.CreateInstance< LevelObjectGraph >();
		ProjectWindowUtil.CreateAsset(graph, "LevelObject.asset");
	}

}
