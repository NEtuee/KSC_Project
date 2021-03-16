using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIKCtrl : MonoBehaviour
{
    private Animator animator;
    [SerializeField]private Vector3 rightHand_Effetor;
    [SerializeField]private Vector3 leftHand_Effetor;

    private Transform leftHandTr, rightHandTr;

    [SerializeField] private bool enableHandIK;
    [SerializeField] private bool enableLeftHandIk;
    [SerializeField] private bool enableRightHandIk;

    [SerializeField] private bool leftTrace;
    [SerializeField] private bool rightTrace;

    [SerializeField]private LayerMask climbingLayer;

    [SerializeField]private bool leftWeightMove = false;
    [SerializeField] private bool rightWeightMove = false;

    private Collider[] hitColliders = new Collider[10];

    [Header("NewType")]
    [SerializeField] private float leftWeight = 0.0f;
    [SerializeField] private float rightWeight = 0.0f;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private float sphereRadius = 0.13f;
    [SerializeField] private Transform LL;
    [SerializeField] private Transform LR;
    [SerializeField] private Transform RL;
    [SerializeField] private Transform RR;
    [SerializeField] private Transform UL;
    [SerializeField] private Transform UR;
    [SerializeField] private Transform DL;
    [SerializeField] private Transform DR;
    [SerializeField] private GameObject maker;

    [Header("LedgeIK")]
    [SerializeField] private bool ledgeIK;
    [SerializeField] private bool climbingMove;
    [SerializeField] private bool ledgeDetection = false;
    [SerializeField] private float horizonOffeset = 0.5f;
    [SerializeField] private float centerDetectionOffset = 0.3f;
    [SerializeField]private Vector3 leftHandPos;
    [SerializeField]private Vector3 rightHandPos;
    private Vector3 nextLeftHandPos;
    private Vector3 nextRightHandPos;
    [SerializeField] private Vector3 leftHandOffset;
    [SerializeField] private Vector3 rightHandOffset;
    [SerializeField] private float normalizedTime;
    [SerializeField] private float upSpeed = 32f;
    [SerializeField] private float downSpeed = 8f;
    [SerializeField] private Vector3 hangLedgeOffset;
    [SerializeField] private Vector3 upClimbingIKOffset;
    [SerializeField] private float centerDetectRadius = 0.3f;
    [SerializeField] private float insideLedgeRadius = 0.3f;
    [SerializeField] private float outSideLedgeRadius = 0.4f;
    [SerializeField] private float insideSurfaceRadius= 0.4f;
    [SerializeField] private float outsideSurfaceRadius = 0.4f;
    [SerializeField] private float outSideRotateDetectionAngle = 20.0f;

    [SerializeField] private AnimationCurve ledgeLeftLeftHandCurve;
    [SerializeField] private AnimationCurve ledgeLeftRightHandCurve;
    [SerializeField] private AnimationCurve ledgeRightLeftHandCurve;
    [SerializeField] private AnimationCurve ledgeRightRightHandCurve;
    [SerializeField] private AnimationCurve leftClimbingLeftHandCurve;
    [SerializeField] private AnimationCurve leftClimbingRightHandCurve;
    [SerializeField] private AnimationCurve rightClimbingLeftHandCurve;
    [SerializeField] private AnimationCurve rightClimbingRightHandCurve;
    [SerializeField] private AnimationCurve upClimbingLeftHandCurve;
    [SerializeField] private AnimationCurve upClimbingRightHandCurve;
    [SerializeField] private AnimationCurve downClimbingLeftHandCurve;
    [SerializeField] private AnimationCurve downClimbingRightHandCurve;



    [SerializeField] private bool leftSide;
    [SerializeField] private bool rightSide;
    private RaycastHit leftHit;
    private RaycastHit rightHit;

    private RaycastHit forwardLeftHit;
    private RaycastHit forwardRightHit;
    private RaycastHit llHit;
    private RaycastHit lrHit;
    private RaycastHit rlHit;
    private RaycastHit rrHit;
    private RaycastHit upLeftHit;
    private RaycastHit upRightHit;
    private RaycastHit downLeftHit;
    private RaycastHit downRightHit;
    [SerializeField] private bool ll_detect;
    [SerializeField] private bool lr_detect;
    [SerializeField] private bool rl_detect;
    [SerializeField] private bool rr_detect;

    void Start()
    {
        animator = GetComponent<Animator>();
        leftHandTr = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHandTr = animator.GetBoneTransform(HumanBodyBones.RightHand);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.5f, 0.6f)), transform.TransformDirection(new Vector3(-0.5f, -1.0f, 0.0f)), Color.green);
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.5f, 0.6f)), transform.TransformDirection(new Vector3(0.5f, -1.0f, 0.0f)), Color.green);

        if (enableHandIK == false)
        {
            return;
        }

        if(leftHandPos == Vector3.zero || rightHandPos == Vector3.zero)
        {
            leftWeight = rightWeight = 0.0f;
            return; 
        }

        normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float abs = (int)normalizedTime;
        normalizedTime = normalizedTime - abs;
        if (animator)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge.Ledge_Left"))
            {          
                leftWeight = ledgeLeftLeftHandCurve.Evaluate(normalizedTime);
                rightWeight = ledgeRightLeftHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge.Ledge_Right"))
            {             
                leftWeight = ledgeLeftRightHandCurve.Evaluate(normalizedTime);
                rightWeight = ledgeRightRightHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge.LedgeIdle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.ClimbingIdle"))
            {
                climbingMove = false;
                leftWeight = 1f;
                rightWeight = 1f;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Climb_Left"))
            {
                leftWeight = leftClimbingLeftHandCurve.Evaluate(normalizedTime);
                rightWeight = leftClimbingRightHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Climb_Right"))
            {
                leftWeight = rightClimbingLeftHandCurve.Evaluate(normalizedTime);
                rightWeight = rightClimbingRightHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Up_LtoR"))
            {
                leftWeight = upClimbingLeftHandCurve.Evaluate(normalizedTime);
                rightWeight = upClimbingRightHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Up_RtoL"))
            {
                leftWeight = upClimbingRightHandCurve.Evaluate(normalizedTime);
                rightWeight = upClimbingLeftHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Down_LtoR"))
            {
                leftWeight = downClimbingRightHandCurve.Evaluate(normalizedTime);
                rightWeight = downClimbingLeftHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Down_RtoL"))
            {
                leftWeight = downClimbingLeftHandCurve.Evaluate(normalizedTime);
                rightWeight = downClimbingRightHandCurve.Evaluate(normalizedTime);
            }

        }
    }

    private void FixedUpdate()
    {
        if (enableHandIK == false)
        {
            return;
        }

        LedgeIKPosDetection();
        enableLeftHandIk = true;
        enableRightHandIk = true;
        if (ledgeIK)
        {
            if (ledgeDetection == true)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.5f, 0.6f)), transform.TransformDirection(new Vector3(-0.5f, -1.0f, 0.0f)), out hit, 1f, climbingLayer))
                {
                    enableLeftHandIk = true;
                    leftHandPos = hit.point - transform.TransformDirection(leftHandOffset);
                }
                else
                    enableLeftHandIk = false;

                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.5f, 0.6f)), transform.TransformDirection(new Vector3(0.5f, -1.0f, 0.0f)), out hit, 1f, climbingLayer))
                {
                    enableRightHandIk = true;
                    rightHandPos = hit.point - transform.TransformDirection(rightHandOffset);
                }
                else
                    enableRightHandIk = false;
            }
            else
            {
                enableLeftHandIk = true;
                enableRightHandIk = true;
            }

        }
    }

    public void ActiveLedgeIK(bool result)
    {
        ledgeIK = result;
        ledgeDetection = result;
    }

    public void ActiveHandIK(bool result)
    {
        enableHandIK = result;
        if(result == false)
        {
            ledgeIK = result;
            ledgeDetection = result;
            leftHandPos = Vector3.zero;
            rightHandPos = Vector3.zero;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableHandIK == false)
            return;

        if (enableLeftHandIk)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);
        }

        if (enableRightHandIk)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightWeight);
        }
    }

    private void LeftTrace()
    {
        climbingMove = true;
        ledgeDetection = false;
        leftHandPos = llHit.point - transform.TransformDirection(hangLedgeOffset);
        nextRightHandPos = lrHit.point - transform.TransformDirection(hangLedgeOffset);
    }

    private void UpdateLeftHandPos()
    {
        leftHandPos = nextLeftHandPos;
    }

    private void RightTrace()
    {
        climbingMove = true;
        ledgeDetection = false;
        rightHandPos = rrHit.point - transform.TransformDirection(hangLedgeOffset);
        nextLeftHandPos = rlHit.point - transform.TransformDirection(hangLedgeOffset);
    }

    private void UpdateRightHandPos()
    {
        rightHandPos = nextRightHandPos;
    }

    public void DisableIK()
    {
        enableLeftHandIk = false;
        enableRightHandIk = false;
    }
    

    public void Maker(bool left)
    {
        RaycastHit hit;
        if (left)
        {
            if (Physics.Raycast(UL.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
            if (Physics.Raycast(UR.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
        }
        else
        {
            if (Physics.Raycast(DR.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
            if (Physics.Raycast(DL.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
        }
    }


    private void OnDrawGizmos()
    {
        DebugDetection();

        Gizmos.color = Color.red;
        if(leftHandTr != null)
            Gizmos.DrawWireSphere(leftHandTr.position, sphereRadius);
        if (rightHandTr != null)
            Gizmos.DrawWireSphere(rightHandTr.position, sphereRadius);
    }

    private void DebugDetection()
    {
        Vector3 start;
        Vector3 dir;
        bool isHit;
       
        /////////////////DetectLedgeIKPosition//////////////////////////////

        if (ledgeIK == true)
        {
            //RR
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.7f;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius,dir, 1.5f, climbingLayer, Color.white, Color.blue);
            if (isHit == false)
            {
                dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
                DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            }

            //RL
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.3f;
            dir = transform.forward;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, insideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);

            //LL
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.7f;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            if (isHit == false)
            {
                dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
                DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            }

            //LR
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.3f;
            dir = transform.forward;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, insideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
        }
        else
        {
            //RR
            start = RR.position;
            dir = transform.forward;
            isHit=DebugCastDetection.Instance.DebugSphereCastDetection(start, outsideSurfaceRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            if(isHit == false)
                dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, outsideSurfaceRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);

            //RL
            start = RL.position;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, insideSurfaceRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            
            //LL
            start = LL.position;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outsideSurfaceRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            if (isHit == false)
                dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, outsideSurfaceRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);

            start = LR.position;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, insideSurfaceRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
        }

    }
    

    private void LedgeIKPosDetection()
    {
        if (climbingMove == true)
            return;

        Vector3 start;
        Vector3 dir;

       
        /////////////////////////LdegeMoveDetect/////////////////////////////////////////////////////////////

        if (ledgeIK== true)
        {
            //Right_right
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.7f;
            //dir = -rightNormal;
            dir = transform.forward;
            if (Physics.SphereCast(start, outSideLedgeRadius, dir, out rrHit, 1.5f, climbingLayer) == false)
            {
                dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
                Physics.SphereCast(start, outSideLedgeRadius, dir, out rrHit, 1.5f, climbingLayer);
            }

            //Right_left
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.3f;
            //dir = -rightNormal;
            dir = transform.forward;
            rl_detect = Physics.SphereCast(start, insideLedgeRadius, dir, out rlHit, 1.5f, climbingLayer);

            //Left_left
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.7f;
            //dir = -leftNormal;
            dir = transform.forward;
            if (Physics.SphereCast(start, outSideLedgeRadius, dir, out llHit, 1.5f, climbingLayer) == false)
            {
                dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
                Physics.SphereCast(start, outSideLedgeRadius, dir, out llHit, 1.5f, climbingLayer);
            }

            //Left_left
            start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.3f;
            //dir = -leftNormal;
            dir = transform.forward;
            lr_detect = Physics.SphereCast(start, insideLedgeRadius, dir, out lrHit, 1.5f, climbingLayer);

            return;
        }
        /////////////////////////////////////UpMoveDetect//////////////////////////////////////////////////////

        //Up_Left
        start = UL.position;
        dir = transform.forward;
        Physics.SphereCast(start, insideLedgeRadius, dir, out upLeftHit, 1.5f, climbingLayer);

        //Up_Right
        start = UR.position;
        dir = transform.forward;
        Physics.SphereCast(start, insideLedgeRadius, dir, out upRightHit, 1.5f, climbingLayer);

        //Up_Left
        start = DL.position;
        dir = transform.forward;
        Physics.SphereCast(start, insideLedgeRadius, dir, out downLeftHit, 1.5f, climbingLayer);

        //Up_Right
        start = DR.position;
        dir = transform.forward;
        Physics.SphereCast(start, insideLedgeRadius, dir, out downRightHit, 1.5f, climbingLayer);

        ////////////////////////////////////HorizonMoveDetect//////////////////////////////////////////////////

        //Left_Left
        start = LL.position;
        dir = transform.forward;
        bool detect = Physics.SphereCast(start, outsideSurfaceRadius, dir, out llHit, 1.5f, climbingLayer);
        if (detect == false)
        {
            dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
            Physics.SphereCast(start, outsideSurfaceRadius, dir, out llHit, 1.5f, climbingLayer);
        }
        //Left_Right
        start = LR.position;
        if(detect == true)
            dir = transform.forward;
        Physics.SphereCast(start, insideSurfaceRadius, dir, out lrHit, 1.5f, climbingLayer);

        //Right_right
        start = RR.position;
        dir = transform.forward;
        detect = Physics.SphereCast(start, outsideSurfaceRadius, dir, out rrHit, 1.5f, climbingLayer);
        if(detect == false)
        {
            dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
            Physics.SphereCast(start, outsideSurfaceRadius, dir, out rrHit, 1.5f, climbingLayer);
        }

        start = RL.position;
        if(detect == true)
            dir = transform.forward;
        Physics.SphereCast(start, insideSurfaceRadius, dir, out rlHit, 1.5f, climbingLayer);
    }

    private void TraceUp(int left)
    {
        climbingMove = true;
        if(left == 1)
        {
            leftHandPos = upLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
            nextRightHandPos = upRightHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
        else
        {
            rightHandPos = upRightHit.point - transform.TransformDirection(upClimbingIKOffset);
            nextLeftHandPos = upLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
    }

    private void TraceDown(int left)
    {
        climbingMove = true;
        if(left == 1)
        {
            nextLeftHandPos = downLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
            nextRightHandPos = downRightHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
        else
        {
            nextRightHandPos = downRightHit.point - transform.TransformDirection(upClimbingIKOffset);
            nextLeftHandPos = downLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
    }

    public void EnableLeftTrace() { leftTrace = true; }
    public void EnableRightTrace() { rightTrace = true; }
    public void DisableLeftTrace() { leftTrace = false; }
    public void DisableRightTrace() { rightTrace = false; }
    public void EnableLeftHandIk() { enableLeftHandIk = true; }
    public void DisableLeftHandIk() { enableLeftHandIk = false; }
    public void EnableRightHandIk() { enableRightHandIk = true; }
    public void DisableRightHandIk() { enableRightHandIk = false; }
}
