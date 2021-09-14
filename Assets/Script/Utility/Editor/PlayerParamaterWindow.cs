using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerParamaterWindow : EditorWindow
{
    [MenuItem("CustomWindow/PlayerParamater")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PlayerParamaterWindow));
    }

    private void OnEnable()
    {
        playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    private void OnGUI()
    {
        playerUnit.WalkSpeed = EditorGUILayout.FloatField("걷는 속도", playerUnit.WalkSpeed);

        GUILayout.Label("청강 대쉬", EditorStyles.boldLabel);
        playerUnit.DashSpeed = EditorGUILayout.FloatField("대쉬 속도", playerUnit.DashSpeed);
        playerUnit.DashTime = EditorGUILayout.FloatField("대쉬 시간", playerUnit.DashTime);
        playerUnit.DashCoolTime = EditorGUILayout.FloatField("대쉬 쿨타임", playerUnit.DashCoolTime);

    }

    private PlayerUnit playerUnit;
}
