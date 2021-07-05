using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(FromRightAppearButton))]
public class FromRightAppearButtonEditor : BaseAppearButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FromRightAppearButton t = (FromRightAppearButton)target;

        t.StartOffset = EditorGUILayout.Vector2Field("StartOffset", t.StartOffset);
        t.Duration = EditorGUILayout.FloatField("Duration", t.Duration);
        t.MiddleCallTiming = EditorGUILayout.Slider("MiddleCallTiming", t.MiddleCallTiming,0.0f,1.0f);

        var prop = serializedObject.FindProperty("onMiddle");
        EditorGUILayout.PropertyField(prop, true);

        serializedObject.ApplyModifiedProperties();
    }
}
