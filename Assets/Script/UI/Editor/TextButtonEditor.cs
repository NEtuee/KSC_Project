using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

[CustomEditor(typeof(TextButton))]
public class TextButtonEditor : ButtonEditor
{
    public Object source;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextButton t = (TextButton)target;

        var prop = serializedObject.FindProperty("onSeleted");
        EditorGUILayout.PropertyField(prop, true);

        var prop1 = serializedObject.FindProperty("onDeseleted");
        EditorGUILayout.PropertyField(prop1, true);

        var prop2 = serializedObject.FindProperty("onEnter");
        EditorGUILayout.PropertyField(prop2, true);

        var prop3 = serializedObject.FindProperty("onExit");
        EditorGUILayout.PropertyField(prop3, true);

        serializedObject.ApplyModifiedProperties();
    }
}
