using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKCtrl : MonoBehaviour
{
    protected Animator animator;
    private PlayerCtrl_Ver2 player;
    private PlayerMovement movement;

    [Header("New")]
    [Range(0f, 1f)] public float distanceToGround;
    public LayerMask layerMask;

    [Header("Regacy")]
    private Vector3 rightFoot_Effetor;
    private Vector3 LeftFoot_Effetor;

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

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

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
        //AnimatorClipInfo[] clipInfos= animator.GetCurrentAnimatorClipInfo(0);
        //foreach(var clipInfo in clipInfos)
        //{
        //    if(clipInfo.clip.name.Equals("Ani_Character_Idle"))
        //    {
        //        enableFeetIk = true;
        //    }
        //    else
        //    {
        //        enableFeetIk = false;
        //    }
        //}

        if (enableFeetIk == false)
        {
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

        if (enableFeetIk == false)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }




        MovePelvisHeight();


        if (player.GetState() == PlayerCtrl_Ver2.PlayerState.Jump || player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab)
            return;

        float leftWeight = animator.GetFloat("LeftIkWeight");
        float rightWeight = animator.GetFloat("RightIkWeight");
        float leftRotWeight = animator.GetFloat("LeftRotationWeight");
        float rightRotWeight = animator.GetFloat("RightRotationWeight");

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);

        if (useProIkFeature)
        {
            //animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,animator.GetFloat(rightFootAnimVariableName));
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightRotWeight);
        }

        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);

        if (useProIkFeature)
        {
            //animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat(leftFootAnimVariableName));
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftRotWeight);
        }

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);



        //if(animator)
        //{
        //    if (player.GetState() == PlayerCtrl_Ver2.PlayerState.RunToStop)
        //    {
        //        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
        //        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
        //        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
        //        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
        //        return;
        //    }

        //    if (player.GetState() != PlayerCtrl_Ver2.PlayerState.Default)
        //    {
        //        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.0f);
        //        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0.0f);
        //        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.0f);
        //        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0.0f);
        //        return;
        //    }

        //    float leftWeight = animator.GetFloat("LeftIkWeight");
        //    float rightWeight = animator.GetFloat("RightIkWeight");
        //    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftWeight);
        //    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftWeight);
        //    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightWeight);
        //    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightWeight);

        //    RaycastHit hit;
        //    Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up,Vector3.down);
        //    if(Physics.Raycast(ray,out hit, distanceToGround + 1f, layerMask))
        //    {
        //        Debug.Log("left");
        //        //if(hit.transform.tag == "Enviroment")
        //        {
        //            Vector3 footPosition = hit.point;
        //            footPosition.y += distanceToGround;
        //            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
        //            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * animator.GetIKRotation(AvatarIKGoal.LeftFoot);
        //            //animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        //            animator.SetIKRotation(AvatarIKGoal.LeftFoot, targetRotation);
        //        }
        //    }

        //    ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        //    if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
        //    {
        //        Debug.Log("right");
        //        //if (hit.transform.tag == "Enviroment")
        //        {
        //            Vector3 footPosition = hit.point;
        //            footPosition.y += distanceToGround;
        //            animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
        //            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * animator.GetIKRotation(AvatarIKGoal.RightFoot);
        //            //animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        //            animator.SetIKRotation(AvatarIKGoal.LeftFoot, targetRotation);

        //        }
        //    }
        //}
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
        if (movement.isGrounded == false || player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab)
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
