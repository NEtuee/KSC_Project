using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCtrl_Ver2))]
public class PlayerCtrl_StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerCtrl_Ver2 player = (PlayerCtrl_Ver2)target;

        if (GUILayout.Button("Hitting"))
        {
            player.TakeDamage(10.0f);
        }
    }
}
