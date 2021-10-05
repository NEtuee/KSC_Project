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

        GUILayout.Label("이동", EditorStyles.boldLabel);
        playerUnit.WalkSpeed = EditorGUILayout.FloatField("걷기 속도", playerUnit.WalkSpeed);
        playerUnit.RunSpeed = EditorGUILayout.FloatField("뛰기 속도", playerUnit.RunSpeed);
        playerUnit.AccelerateSpeed = EditorGUILayout.FloatField("가속 속도", playerUnit.AccelerateSpeed);

        GUILayout.Label("대쉬", EditorStyles.boldLabel);
        playerUnit.DashSpeed = EditorGUILayout.FloatField("대쉬 속도", playerUnit.DashSpeed);
        playerUnit.DashTime = EditorGUILayout.FloatField("대쉬 시간", playerUnit.DashTime);
        playerUnit.DashCoolTime = EditorGUILayout.FloatField("대쉬 쿨타임", playerUnit.DashCoolTime);

        GUILayout.Label("퀵 스탠딩", EditorStyles.boldLabel);
        playerUnit.QuickStandCoolTime = EditorGUILayout.FloatField("퀵스탠딩 쿨타임", playerUnit.QuickStandCoolTime);

        GUILayout.Label("Aim", EditorStyles.boldLabel);
        followTarget.crosshairMovingSpeed = EditorGUILayout.FloatField("Crosshair Moving Speed",followTarget.crosshairMovingSpeed);
        followTarget.aimMovingSpeed = EditorGUILayout.FloatField("Aim Moving Speed",followTarget.aimMovingSpeed);
        followTarget.aimLimitDist = EditorGUILayout.FloatField("Aim Limit Distance",followTarget.aimLimitDist);

        GUILayout.Label("EMP 건", EditorStyles.boldLabel);
        playerUnit.NoramlGunCost = EditorGUILayout.FloatField("노말 샷 코스트", playerUnit.NoramlGunCost);
        playerUnit.ChargeGunCost = EditorGUILayout.FloatField("차지 샷 코스트", playerUnit.ChargeGunCost);
    }

    private PlayerUnit playerUnit;
    private FollowTargetCtrl followTarget;
}
