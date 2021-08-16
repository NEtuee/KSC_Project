using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;
using UnityEditor;

[CustomEditor(typeof(GraphObjectBase),true),CanEditMultipleObjects]
public class GraphObjectBaseEditor : MessageReceiverEditor
{
    protected GraphObjectBase obj;

    public override void OnEnable()
    {
        base.OnEnable();
        obj = (GraphObjectBase)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(obj.graphOrigin != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Graph Name");
            obj.graphOrigin.name = EditorGUILayout.TextArea(obj.graphOrigin.name);
            GUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Open Graph"))
        {
            EditorWindow.GetWindow<StateGraphWindow>().InitializeGraph(obj.graphOrigin as FunctionGraph);
        }

        if(GUILayout.Button("Copy Graph From Origin"))
        {
            if(obj.graphOrigin == null)
            {
                Debug.Log("Graph origin is not exists");
                return;
            }

            var copy = ScriptableObject.Instantiate(obj.graphOrigin);

            obj.graphOrigin = copy;
        }

        if(GUILayout.Button("Create New Graph"))
        {
            obj.graphOrigin = ScriptableObject.CreateInstance<LevelObjectGraph>();
        }

        if(GUILayout.Button("Save to ScriptableObject File"))
        {
            if(obj.graphOrigin.name == "")
            {
                Debug.Log("Graph name is empty");
                return;
            }

            var graph = ScriptableObject.Instantiate(obj.graphOrigin);
		    ProjectWindowUtil.CreateAsset(graph, obj.graphOrigin.name + ".asset");
        }

        if(GUILayout.Button("Save Asset"))
        {
            EditorUtility.SetDirty(obj.graphOrigin);
			AssetDatabase.SaveAssets();
        }

    }
}
