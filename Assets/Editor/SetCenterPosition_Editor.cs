using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetCenterPosition))]
public class SetCenterPosition_Editor : Editor
{
    SetCenterPosition controll;

	void OnEnable()
    {
        controll = (SetCenterPosition)target;
    }

    public override void OnInspectorGUI()
    {
		base.OnInspectorGUI();

        if(GUILayout.Button("Progress"))
        {
            controll.Progress();
        }
    }
}
