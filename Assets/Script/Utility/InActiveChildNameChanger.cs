#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InActiveChildNameChanger : MonoBehaviour
{
    public string changeName;

    public void Progress()
    {
        Progress(transform);
    }

    public void Progress(Transform tp)
    {
        int count = tp.childCount;
        for(int i = 0; i < count; ++i)
        {
            var child = tp.GetChild(i);
            if(!child.gameObject.activeSelf)
            {
                child.name = changeName;
            }

            Progress(child);
        }
    }
}

[CustomEditor(typeof(InActiveChildNameChanger)),CanEditMultipleObjects]
public class InActiveChildNameChangerEdit : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InActiveChildNameChanger changer = (InActiveChildNameChanger)target;
        if (GUILayout.Button("Change"))
        {
            changer.Progress();
        }
    }
}

#endif