#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChildNameChanger : MonoBehaviour
{
    public string targetName;
    public string changeName;

    public void Progress()
    {
        int count = transform.childCount;
        for(int i = 0; i < count; ++i)
        {
            var child = transform.GetChild(i);
            if(child.name == targetName)
            {
                child.name = changeName;
            }
        }
    }
}

[CustomEditor(typeof(ChildNameChanger)),CanEditMultipleObjects]
public class ChildNameChangerEdit : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChildNameChanger changer = (ChildNameChanger)target;
        if (GUILayout.Button("Change"))
        {
            changer.Progress();
        }
    }
}

#endif