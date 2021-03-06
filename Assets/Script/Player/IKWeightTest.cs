using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeightTest : MonoBehaviour
{
    [SerializeField] private Transform leftHandPoint;
    [SerializeField] private Transform rightHandPoint;

    [Range(0f, 1f)] public float leftWeight = 1f;
    [Range(0f, 1f)] public float rightWeight = 1f;

    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPoint.position);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPoint.position);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightWeight);
    }
}
