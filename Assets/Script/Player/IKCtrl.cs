using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKCtrl : MonoBehaviour
{
    protected Animator animator;
    private PlayerCtrl_Ver2 player;
    private PlayerMovement movement;

    [Range(0f, 1f)] public float distanceToGround;
    public LayerMask layerMask;
    private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    [SerializeField] private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    public bool enableFeetIk = true;
    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask enviormentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [SerializeField] private float newPelvisOffset = 0f;
    [Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)] [SerializeField] private float feetToIkPositionSpeed = 0.5f;
    [SerializeField] private Vector3 positionToPelvisDifference = Vector3.zero;

    public bool useProIkFeature = false;
    public bool showSolverDebug = true;


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerCtrl_Ver2>();
        movement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        if (enableFeetIk == false)
        {
            leftFootIkPosition = Vector3.zero;
            rightFootIkPosition = Vector3.zero;
            //Debug.Log("Dddd");
            return;
        }

        if (animator == null)
        {
            return;
        }

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (positionToPelvisDifference == Vector3.zero)
        {
            positionToPelvisDifference = animator.bodyPosition - transform.position;
        }

        if (animator == null)
        {
            return;
        }

        if (Vector3.Distance(animator.bodyPosition, transform.position) >= 2.0f)
            InitPelvisHeight();

        float leftWeight = animator.GetFloat("LeftIkWeight");
        float rightWeight = animator.GetFloat("RightIkWeight");
        float leftRotWeight = animator.GetFloat("LeftRotationWeight");
        float rightRotWeight = animator.GetFloat("RightRotationWeight");

        if (leftWeight != 0.0f || rightWeight != 0.0f)
            MovePelvisHeight();
        else
        {
            lastPelvisPositionY = animator.bodyPosition.y;
        }

        if (enableFeetIk == false)
        {
            return;
        }

        if (player.GetState() == PlayerCtrl_Ver2.PlayerState.Jump || player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab || player.GetState() == PlayerCtrl_Ver2.PlayerState.HangLedge)
            return;

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightWeight);

        if (useProIkFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightWeight);
        }

        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftWeight);

        if (useProIkFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftWeight);
        }

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
    }

    public void EnableFeetIk() { enableFeetIk = true; }
    public void DisableFeetIk() { enableFeetIk = false; }

    void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
    {
        Vector3 targetIkPosition = animator.GetIKPosition(foot);
        
        if(positionIkHolder != Vector3.zero)
        {
            targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
            targetIkPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIkPosition = transform.TransformPoint(targetIkPosition);

            animator.SetIKRotation(foot, rotationIkHolder);
        }

        animator.SetIKPosition(foot, targetIkPosition);
    }

    private void MovePelvisHeight()
    {
        if (movement.isGrounded == false 
            || player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab
            || player.GetState() == PlayerCtrl_Ver2.PlayerState.ClimbingJump)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        if(rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIkPosition.y - transform.position.y;

        float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

        animator.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = animator.bodyPosition.y;
    }

    public void InitPelvisHeight()
    {
        lastPelvisPositionY = transform.position.y + positionToPelvisDifference.y;
    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
    {
        RaycastHit feetOutHit;

        if(showSolverDebug)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);
        }

        if(Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, enviormentLayer))
        {
            feetIkPositions = fromSkyPosition;
            feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIkPositions = Vector3.zero;
    }

    private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;
    }
}
