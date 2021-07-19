using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

[CustomEditor(typeof(TextButtonWithBackGround))]
public class TextButtonWithBackGroundEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextButtonWithBackGround t = (TextButtonWithBackGround)target;

        t.backGroundImage = (Image)EditorGUILayout.ObjectField("BackGroundImage",t.backGroundImage, typeof(Image), true);
        t.selectColor = EditorGUILayout.ColorField("SelectColor", t.selectColor);
        t.enterColor = EditorGUILayout.ColorField("EnterColor", t.enterColor);
        t.deselectColor = EditorGUILayout.ColorField("DeselectColor", t.deselectColor);
        t.exitColor = EditorGUILayout.ColorField("ExitColor", t.exitColor);

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
