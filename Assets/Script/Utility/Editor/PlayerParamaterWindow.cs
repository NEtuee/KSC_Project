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
        playerUnit.ChargeConsumeTime = EditorGUILayout.FloatField("풀차지 소요 시간", playerUnit.ChargeConsumeTime);

        GUILayout.Label("점프 관련", EditorStyles.boldLabel);
        playerUnit.ClimbingUpJumpPower = EditorGUILayout.FloatField("클라이밍 위 점프 파워", playerUnit.ClimbingUpJumpPower);
        playerUnit.ClimbingHorizonJumpPower = EditorGUILayout.FloatField("클라이밍 옆 점프 파워", playerUnit.ClimbingHorizonJumpPower);
        playerUnit.KeepClimbingUpJumpTime = EditorGUILayout.FloatField("클라이밍 위 점프 지속시간", playerUnit.KeepClimbingUpJumpTime);
        playerUnit.KeepClimbingHorizonJumpTime = EditorGUILayout.FloatField("클라이밍 옆 점프 지속시간", playerUnit.KeepClimbingHorizonJumpTime);

        GUILayout.Label("카메라 보정", EditorStyles.boldLabel);
        followTarget.RevisionSpeed = EditorGUILayout.FloatField("보정 회전 속도", followTarget.RevisionSpeed);
        followTarget.RevisionStartTime = EditorGUILayout.FloatField("보정 시작 시간", followTarget.RevisionStartTime);
    }

    private PlayerUnit playerUnit;
    private FollowTargetCtrl followTarget;
}
