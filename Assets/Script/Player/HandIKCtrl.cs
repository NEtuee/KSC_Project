using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIKCtrl : MonoBehaviour
{
    private Animator animator;
    [SerializeField]private Vector3 rightHand_Effetor;
    [SerializeField] private Quaternion rightRot;
    [SerializeField]private Vector3 leftHand_Effetor;

    [SerializeField]private Transform leftHandPosition, rightHandPosition;

    [SerializeField] private bool enableHandIK;
    [SerializeField] private bool enableLeftHandIk;
    [SerializeField] private bool enableRightHandIk;

    [SerializeField] private bool leftTrace;
    [SerializeField] private bool rightTrace;

    [SerializeField]private LayerMask climbingLayer;

    [SerializeField] private Transform rightHandObj;
    [SerializeField] private Transform rightHand;

    [SerializeField] private GameObject sphere;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        AdjustHandTarget(ref leftHandPosition, HumanBodyBones.LeftHand);
        AdjustHandTarget(ref rightHandPosition, HumanBodyBones.RightHand);

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    //rightHand_Effetor = animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        //    rightHand_Effetor = rightHand.position;
        //    //rightRot = animator.GetBoneTransform(HumanBodyBones.RightHand).rotation;
        //    rightRot = rightHand.rotation;
        //    enableRightHandIk = true;
        //    Instantiate(sphere, rightHand_Effetor, rightRot);
        //}

        if (enableHandIK == false)
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
            //animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            //animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand_Effetor);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand_Effetor);
            //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            //animator.SetIKRotation(AvatarIKGoal.RightHand, rightRot);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
        }

        if (enableRightHandIk == true)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand_Effetor);
            //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            //animator.SetIKRotation(AvatarIKGoal.RightHand, rightRot);

            //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            //animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
            //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            //animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        }
    }

    private void AdjustHandTarget(ref Transform handPosition, HumanBodyBones hand)
    {
        handPosition = animator.GetBoneTransform(hand);
    }

    public void EnableLeftTrace() { leftTrace = true; }
    public void EnableRightTrace() { rightTrace = true; }
    public void DisableLeftTrace() { leftTrace = false; }
    public void DisableRightTrace() { rightTrace = false; }
    public void EnableLeftHandIk() { enableLeftHandIk = true; }
    public void DisableLeftHandIk() { enableLeftHandIk = false; }
    public void EnableRightHandIk() { enableRightHandIk = true; }
    public void DisableRightHandIk() { enableRightHandIk = false; }

    public void DisableIK()
    {
        enableLeftHandIk = false;
        enableRightHandIk = false;
    }

    public void LeftHandIkTrace()
    {
        RaycastHit hit;
        //Physics.SphereCast(leftHandPosition, 2.0f, transform.forward, out hit, 0f, climbingLayer)
        //Physics.Raycast(leftHandPosition, transform.forward, out hit, 1f, climbingLayer)
        //Debug.Log("LeftHandIk");
        if (Physics.Raycast(leftHandPosition.position, transform.forward, out hit, 1f, climbingLayer))
        {
            //Debug.Log("LeftHandIkColl");
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
        //Physics.SphereCast(rightHandPosition, 2.0f, transform.forward, out hit, 0f,climbingLayer)
        //Physics.Raycast(rightHandPosition, transform.forward, out hit, 1f, climbingLayer)
        if (Physics.Raycast(rightHandPosition.position, transform.forward, out hit, 1f, climbingLayer))
        {
            //Debug.Log("RightColl");
            rightHand_Effetor = hit.point;
            enableRightHandIk = true;
            rightTrace = false;
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand_Effetor);
        }
    }
}
