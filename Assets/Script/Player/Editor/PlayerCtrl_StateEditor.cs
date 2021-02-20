using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCtrl_State))]
public class PlayerCtrl_StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerCtrl_State player = (PlayerCtrl_State)target;

        if (GUILayout.Button("Hitting"))
        {
            player.TakeDamage(10.0f);
        }
    }
}
