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
        playerUnit.WalkSpeed = EditorGUILayout.FloatField("�ȴ� �ӵ�", playerUnit.WalkSpeed);

        GUILayout.Label("û�� �뽬", EditorStyles.boldLabel);
        playerUnit.DashSpeed = EditorGUILayout.FloatField("�뽬 �ӵ�", playerUnit.DashSpeed);
        playerUnit.DashTime = EditorGUILayout.FloatField("�뽬 �ð�", playerUnit.DashTime);
        playerUnit.DashCoolTime = EditorGUILayout.FloatField("�뽬 ��Ÿ��", playerUnit.DashCoolTime);

    }

    private PlayerUnit playerUnit;
}
