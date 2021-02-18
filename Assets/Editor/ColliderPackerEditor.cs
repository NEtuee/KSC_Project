using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColliderPacker))]
public class ColliderPackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ColliderPacker coll = (ColliderPacker)target;
        if (GUILayout.Button("Pack"))
        {
            coll.Pack();
        }
    }
}
