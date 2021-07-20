using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(BaseAppearButton))]
public class BaseAppearButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BaseAppearButton t = (BaseAppearButton)target;

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
