using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(FromRightAppearButton))]
public class FromRightAppearButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FromRightAppearButton t = (FromRightAppearButton)target;
    }
}
