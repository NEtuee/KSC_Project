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
        if (playerUnit == null)
            return;

        GUILayout.Label("이동", EditorStyles.boldLabel);
        playerUnit.WalkSpeed = EditorGUILayout.FloatField("걷는 속도", playerUnit.WalkSpeed);
        playerUnit.RunSpeed = EditorGUILayout.FloatField("뛰는 속도", playerUnit.RunSpeed);
        playerUnit.AccelerateSpeed = EditorGUILayout.FloatField("가속 속도", playerUnit.AccelerateSpeed);

        GUILayout.Label("청강 대쉬", EditorStyles.boldLabel);
        playerUnit.DashSpeed = EditorGUILayout.FloatField("대쉬 속도", playerUnit.DashSpeed);
        playerUnit.DashTime = EditorGUILayout.FloatField("대쉬 시간", playerUnit.DashTime);
        playerUnit.DashCoolTime = EditorGUILayout.FloatField("대쉬 쿨타임", playerUnit.DashCoolTime);

        GUILayout.Label("퀵 스탠딩", EditorStyles.boldLabel);
        playerUnit.QuickStandCoolTime = EditorGUILayout.FloatField("퀵 스탠딩 쿨타임", playerUnit.QuickStandCoolTime);
    }

    private PlayerUnit playerUnit;
}
