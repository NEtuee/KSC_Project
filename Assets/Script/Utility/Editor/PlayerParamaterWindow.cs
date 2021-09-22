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
        followTarget = GameObject.Find("FollowTarget").GetComponent<FollowTargetCtrl>();
    }

    private void OnGUI()
    {
        if (playerUnit == null)
            return;

        GUILayout.Label("�̵�", EditorStyles.boldLabel);
        playerUnit.WalkSpeed = EditorGUILayout.FloatField("�ȴ� �ӵ�", playerUnit.WalkSpeed);
        playerUnit.RunSpeed = EditorGUILayout.FloatField("�ٴ� �ӵ�", playerUnit.RunSpeed);
        playerUnit.AccelerateSpeed = EditorGUILayout.FloatField("���� �ӵ�", playerUnit.AccelerateSpeed);

        GUILayout.Label("û�� �뽬", EditorStyles.boldLabel);
        playerUnit.DashSpeed = EditorGUILayout.FloatField("�뽬 �ӵ�", playerUnit.DashSpeed);
        playerUnit.DashTime = EditorGUILayout.FloatField("�뽬 �ð�", playerUnit.DashTime);
        playerUnit.DashCoolTime = EditorGUILayout.FloatField("�뽬 ��Ÿ��", playerUnit.DashCoolTime);

        GUILayout.Label("�� ���ĵ�", EditorStyles.boldLabel);
        playerUnit.QuickStandCoolTime = EditorGUILayout.FloatField("�� ���ĵ� ��Ÿ��", playerUnit.QuickStandCoolTime);

        GUILayout.Label("Aim", EditorStyles.boldLabel);
        followTarget.crosshairMovingSpeed = EditorGUILayout.FloatField("Crosshair Moving Speed",followTarget.crosshairMovingSpeed);
        followTarget.aimMovingSpeed = EditorGUILayout.FloatField("Aim Moving Speed",followTarget.aimMovingSpeed);
        followTarget.aimLimitDist = EditorGUILayout.FloatField("Aim Limit Distance",followTarget.aimLimitDist);
    }

    private PlayerUnit playerUnit;
    private FollowTargetCtrl followTarget;
}
