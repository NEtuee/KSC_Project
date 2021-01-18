using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIKCtrl : MonoBehaviour
{
    private Animator animator;
    [SerializeField]private Vector3 rightHand_Effetor;
    [SerializeField]private Vector3 leftHand_Effetor;

    [SerializeField]private Vector3 leftHandPosition, rightHandPosition;

    [SerializeField] private bool enableHandIK;
    [SerializeField] private bool enableLeftHandIk;
    [SerializeField] private bool enableRightHandIk;

    [SerializeField] private bool leftTrace;
    [SerializeField] private bool rightTrace;

    [SerializeField]private LayerMask climbingLayer;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AdjustHandTarget(ref leftHandPosition, HumanBodyBones.LeftHand);
        AdjustHandTarget(ref rightHandPosition, HumanBodyBones.RightHand);

        if(enableHandIK == false)
        {
            return;
        }

        if (leftTrace == true)
        {
            LeftHandIkTrace();
        }

        if(rightTrace == true)
        {
            RightHandIkTrace();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(enableLeftHandIk == true)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand_Effetor);
        }

        if (enableRightHandIk == true)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand_Effetor);
        }
    }

    private void AdjustHandTarget(ref Vector3 handPosition, HumanBodyBones hand)
    {
        handPosition = animator.GetBoneTransform(hand).position;
    }

    public void EnableLeftTrace() { leftTrace = true; }
    public void EnableRightTrace() { rightTrace = true; }

    public void EnableLeftHandIk() { enableLeftHandIk = true; }
    public void DisableLeftHandIk() { enableLeftHandIk = false; }
    public void EnableRightHandIk() { enableRightHandIk = true; }
    public void DisableRightHandIk() { enableRightHandIk = false; }

    public void LeftHandIkTrace()
    {
        RaycastHit hit;
        Debug.DrawRay(leftHandPosition, transform.forward * 1f);
        //Physics.SphereCast(leftHandPosition, 2.0f, transform.forward, out hit, 0f, climbingLayer)
        //Physics.Raycast(leftHandPosition, transform.forward, out hit, 1f, climbingLayer)
        if (Physics.Raycast(leftHandPosition, transform.forward, out hit, 1f, climbingLayer))
        {
            //Debug.Log("LeftColl");
            leftHand_Effetor = hit.point;
            enableLeftHandIk = true;
            leftTrace = false;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand_Effetor);
        }
    }

    public void RightHandIkTrace()
    {
        RaycastHit hit;
        Debug.DrawRay(rightHandPosition, transform.forward * 1f);
        //Physics.SphereCast(rightHandPosition, 2.0f, transform.forward, out hit, 0f,climbingLayer)
        //Physics.Raycast(rightHandPosition, transform.forward, out hit, 1f, climbingLayer)
        if (Physics.Raycast(rightHandPosition, transform.forward, out hit, 1f, climbingLayer))
        {
            //Debug.Log("RightColl");
            rightHand_Effetor = hit.point;
            enableRightHandIk = true;
            rightTrace = false;
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand_Effetor);
        }
    }
}
