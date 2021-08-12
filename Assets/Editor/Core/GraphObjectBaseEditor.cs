using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;
using UnityEditor;

[CustomEditor(typeof(GraphObjectBase),true)]
public class GraphObjectBaseEditor : Editor
{
    protected GraphObjectBase obj;

    public virtual void OnEnable()
    {
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

    }
}
