using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIKCtrl : MonoBehaviour
{
    public bool debugHandIK;
    public bool debugDetectSphere;
    private Animator animator;

    [SerializeField] private bool notUseHandIK;
    [SerializeField] private bool enableHandIK;
    [SerializeField] private bool enableLeftHandIk;
    [SerializeField] private bool enableRightHandIk;
    [SerializeField] private bool startLedgeIK;

    [SerializeField]private LayerMask climbingLayer;

    [SerializeField] private float leftWeight = 0.0f;
    [SerializeField] private float rightWeight = 0.0f;

    [SerializeField] private Transform LL;
    [SerializeField] private Transform LR;
    [SerializeField] private Transform RL;
    [SerializeField] private Transform RR;
    [SerializeField] private Transform UL;
    [SerializeField] private Transform UR;
    [SerializeField] private Transform DL;
    [SerializeField] private Transform DR;
    [SerializeField] private Transform ULR;
    [SerializeField] private Transform ULL;
    [SerializeField] private Transform URR;
    [SerializeField] private Transform URL;
    [SerializeField] private Transform DLL;
    [SerializeField] private Transform DLR;
    [SerializeField] private Transform DRL;
    [SerializeField] private Transform DRR;
    [SerializeField] private Transform center_L;
    [SerializeField] private Transform center_R;

    [SerializeField] private Vector3 leftLeftOffset;
    [SerializeField] private Vector3 leftRightOffset;
    [SerializeField] private Vector3 rightLeftOffset;
    [SerializeField] private Vector3 rightRightOffset;
    [SerializeField] private Vector3 upLeftOffset;
    [SerializeField] private Vector3 upRightOffset;
    [SerializeField] private Vector3 downLeftOffset;
    [SerializeField] private Vector3 downRightOffset;
    [SerializeField] private Vector3 upLeftRightOffset;
    [SerializeField] private Vector3 upLeftLeftOffset;
    [SerializeField] private Vector3 upRightLeftOffset;
    [SerializeField] private Vector3 upRightRightOffset;
    [SerializeField] private Vector3 downLeftLeftOffset;
    [SerializeField] private Vector3 downLeftRightOffset;
    [SerializeField] private Vector3 downRightLeftOffset;
    [SerializeField] private Vector3 downRightRightOffset;
    [SerializeField] private Vector3 centerLeftOffset;
    [SerializeField] private Vector3 centerRightOffset;

    [SerializeField] private GameObject maker;

    [SerializeField] private bool ledgeIK;
    [SerializeField] private bool climbingMove;
    [SerializeField] private bool ledgeDetection = false;
    [SerializeField]private Vector3 leftHandPos;
    [SerializeField]private Vector3 rightHandPos;
    [SerializeField] private Vector3 leftHandOffset;
    [SerializeField] private Vector3 rightHandOffset;
    [SerializeField] private float ledgeSphereUpOffset = 1.2f;
    [SerializeField] private float normalizedTime;
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
    [SerializeField] private AnimationCurve upDiagonalClimbingFirstHandCurve;
    [SerializeField] private AnimationCurve upDiagonalClimbingSecondHandCurve;
    [SerializeField] private AnimationCurve downDiagonalClimbingFirstHandCurve;
    [SerializeField] private AnimationCurve downDiagonalClimbingSecondHandCurve;

    private RaycastHit llHit;
    private RaycastHit lrHit;
    private RaycastHit rlHit;
    private RaycastHit rrHit;
    private RaycastHit upLeftHit;
    private RaycastHit upRightHit;
    private RaycastHit downLeftHit;
    private RaycastHit downRightHit;
    private RaycastHit upLeft_LeftHit;
    private RaycastHit upLeft_RightHit;
    private RaycastHit upRight_LeftHit;
    private RaycastHit upRight_RightHit;
    private RaycastHit downLeft_LeftHit;
    private RaycastHit downLeft_RightHit;
    private RaycastHit downRight_LeftHit;
    private RaycastHit downRight_RightHit;

    [SerializeField]private Transform leftHandPointObjet;
    [SerializeField]private Transform rightHandPointObject;
    [SerializeField]private Transform nextLeftHandPointObject;
    [SerializeField] private Transform nextRightHandPointObject;

    void Start()
    {
        animator = GetComponent<Animator>();

        leftHandPointObjet = new GameObject("LeftHandPoint").transform;
        rightHandPointObject = new GameObject("RightHandPoint").transform;
        nextLeftHandPointObject = new GameObject("NextLeftHandPoint").transform;
        nextRightHandPointObject = new GameObject("NextRightHandPoint").transform;
        leftHandPointObjet.SetParent(this.transform);
        leftHandPointObjet.position = Vector3.zero;
        rightHandPointObject.SetParent(this.transform);
        rightHandPointObject.position = Vector3.zero;
        nextRightHandPointObject.SetParent(this.transform);
        nextLeftHandPointObject.SetParent(this.transform);

        GenerateDetectPoint();
    }

    void Update()
    {
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 3.0f, 0.6f)), transform.TransformDirection(new Vector3(-0.5f, -1.0f, 0.0f)), Color.green);
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 3.0f, 0.6f)), transform.TransformDirection(new Vector3(0.5f, -1.0f, 0.0f)), Color.green);

        if(notUseHandIK == true)
        {
            return;
        }

        if (enableHandIK == false)
        {
            return;
        }

        if (leftHandPointObjet.position == Vector3.zero || rightHandPointObject.position == Vector3.zero)
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
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Climbing_UpLeft"))
            {
                leftWeight = upDiagonalClimbingFirstHandCurve.Evaluate(normalizedTime);
                rightWeight = upDiagonalClimbingSecondHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.Climbing_UpRight"))
            {
                leftWeight = upDiagonalClimbingSecondHandCurve.Evaluate(normalizedTime);
                rightWeight = upDiagonalClimbingFirstHandCurve.Evaluate(normalizedTime);
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.ClimbDownLeft"))
            {
                leftWeight = downDiagonalClimbingFirstHandCurve.Evaluate(normalizedTime);
                rightWeight = downDiagonalClimbingSecondHandCurve.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing.ClimbDownRight"))
            {
                leftWeight = downDiagonalClimbingSecondHandCurve.Evaluate(normalizedTime);
                rightWeight = downDiagonalClimbingFirstHandCurve.Evaluate(normalizedTime);
            }
        }
    }

    private void FixedUpdate()
    {
        if (notUseHandIK == true)
        {
            return;
        }

        if (enableHandIK == false)
        {
            return;
        }

        LedgeIKPosDetection();

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
        if(result == true)
        {
            startLedgeIK = true;
        }
    }

    public void ActiveHandIK(bool result)
    {
        enableHandIK = result;
        if (result == false)
        {
            ledgeIK = result;
            ledgeDetection = result;
            leftHandPos = Vector3.zero;
            rightHandPos = Vector3.zero;
        }
        else
        {
            TraceCenter();
        }
    }

    public void DisableHandIK()
    {
        enableHandIK = false;
        enableLeftHandIk = false;
        enableRightHandIk = false;
        ledgeIK = false;
        ledgeDetection = false;
        leftHandPos = Vector3.zero;
        rightHandPos = Vector3.zero;

        leftHandPointObjet.SetParent(this.transform);
        rightHandPointObject.SetParent(this.transform);
        nextLeftHandPointObject.SetParent(this.transform);
        nextRightHandPointObject.SetParent(this.transform);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableHandIK == false)
            return;

        if (enableLeftHandIk)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPointObjet.position);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);
        }

        if (enableRightHandIk)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPointObject.position);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightWeight);
        }
    }

    private void LeftTrace()
    {
        climbingMove = true;
        ledgeDetection = false;
        startLedgeIK = false;

        if (leftHandPointObjet == null)
            leftHandPointObjet = CreatePointObject("LeftHandPoint");
        leftHandPointObjet.SetParent(llHit.transform);
        leftHandPointObjet.position = llHit.point - transform.TransformDirection(hangLedgeOffset);

        if (nextRightHandPointObject == null)
            nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
        nextRightHandPointObject.SetParent(lrHit.transform);
        nextRightHandPointObject.position = lrHit.point - transform.TransformDirection(hangLedgeOffset);
    }

    private void UpdateLeftHandPos()
    {
        enableLeftHandIk = true;
        if (leftHandPointObjet == null)
            leftHandPointObjet = CreatePointObject("LeftHandPoint");
        leftHandPointObjet.SetParent(nextLeftHandPointObject.parent);
        leftHandPointObjet.position = nextLeftHandPointObject.position;
    }

    private void RightTrace()
    {
        enableRightHandIk = true;
        climbingMove = true;
        ledgeDetection = false;
        startLedgeIK = false;

        if (rightHandPointObject == null)
            rightHandPointObject = CreatePointObject("RightHandPoint");
        rightHandPointObject.SetParent(rrHit.transform);
        rightHandPointObject.position = rrHit.point - transform.TransformDirection(hangLedgeOffset);
        if (nextLeftHandPointObject == null)
            nextLeftHandPointObject = CreatePointObject("NextLeftHandPoint");
        nextLeftHandPointObject.SetParent(rlHit.transform);
        nextLeftHandPointObject.position = rlHit.point - transform.TransformDirection(hangLedgeOffset);
    }

    private void UpdateRightHandPos()
    {
        enableRightHandIk = true;
        if (rightHandPointObject == null)
            rightHandPointObject = CreatePointObject("RightHandPoint");
        rightHandPointObject.SetParent(nextRightHandPointObject.parent);
        rightHandPointObject.position = nextRightHandPointObject.position;
    }

    private void TraceLedge()
    {
        enableLeftHandIk = true;
        enableRightHandIk = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.0f, 3.0f, 0.6f)), transform.TransformDirection(new Vector3(-0.15f, -1.0f, 0.0f)), out hit, 3f, climbingLayer))
        {
            leftHandPointObjet.SetParent(hit.transform);
            leftHandPointObjet.position = hit.point - transform.TransformDirection(leftHandOffset);
        }

        if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.0f, 3.0f, 0.6f)), transform.TransformDirection(new Vector3(0.15f, -1.0f, 0.0f)), out hit, 3f, climbingLayer))
        {
            rightHandPointObject.SetParent(hit.transform);
           rightHandPointObject.position = hit.point - transform.TransformDirection(rightHandOffset);
        }
    }

    public void DisableIK()
    {
        enableLeftHandIk = false;
        enableRightHandIk = false;
    }
    

    public void Maker(int left)
    {
        RaycastHit hit;
        if (left == 1)
        {
            if (Physics.Raycast(ULL.position, transform.forward, out hit, 1.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
            if (Physics.Raycast(ULR.position, transform.forward, out hit, 1.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
        }
        else
        {
            if (Physics.Raycast(URR.position, transform.forward, out hit, 1.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
            if (Physics.Raycast(URL.position, transform.forward, out hit, 1.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
        }
    }


    private void OnDrawGizmos()
    {
        DebugDetection();
    }

    private void DebugDetection()
    {
        Vector3 start;
        Vector3 dir;
        bool isHit;

        if (leftHandPointObjet != null && rightHandPointObject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leftHandPointObjet.position, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rightHandPointObject.position, 0.3f);
        }

        if (debugDetectSphere == false)
            return;
        /////////////////DetectLedgeIKPosition//////////////////////////////
        if (startLedgeIK == true)
        {
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * -0.3f;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);

            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * 0.3f;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            return;
        }


        if (ledgeIK == true)
        {
            //RR
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * 0.7f;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius,dir, 1.5f, climbingLayer, Color.white, Color.blue);
            if (isHit == false)
            {
                dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
                DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            }

            //RL
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * 0.3f;
            dir = transform.forward;
            DebugCastDetection.Instance.DebugSphereCastDetection(start, insideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);

            //LL
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * -0.7f;
            dir = transform.forward;
            isHit = DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            if (isHit == false)
            {
                dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
                DebugCastDetection.Instance.DebugSphereCastDetection(start, outSideLedgeRadius, dir, 1.5f, climbingLayer, Color.white, Color.blue);
            }

            //LR
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * -0.3f;
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
        Vector3 start;
        Vector3 dir;

       
        /////////////////////////LdegeMoveDetect/////////////////////////////////////////////////////////////

        if(startLedgeIK == true)
        {
            RaycastHit hit;
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * -0.3f;
            dir = transform.forward;
            if (Physics.SphereCast(start, outSideLedgeRadius, dir, out hit, 1.5f, climbingLayer))
            {
                leftHandPointObjet.SetParent(hit.transform);
                leftHandPointObjet.position = hit.point - transform.TransformDirection(hangLedgeOffset);
            }

            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * 0.3f;
            dir = transform.forward;
            if (Physics.SphereCast(start, outSideLedgeRadius, dir, out hit, 1.5f, climbingLayer))
            {
                rightHandPointObject.SetParent(hit.transform);
                rightHandPointObject.position = hit.point - transform.TransformDirection(hangLedgeOffset);
            }
        }

        if (ledgeIK== true)
        {
            //Right_right
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * 0.7f;
            dir = transform.forward;
            if (Physics.SphereCast(start, outSideLedgeRadius, dir, out rrHit, 1.5f, climbingLayer) == false)
            {
                dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
                Physics.SphereCast(start, outSideLedgeRadius, dir, out rrHit, 1.5f, climbingLayer);
            }

            //Right_left
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * 0.3f;
            dir = transform.forward;
            Physics.SphereCast(start, insideLedgeRadius, dir, out rlHit, 1.5f, climbingLayer);

            //Left_left
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * -0.7f;
            dir = transform.forward;
            if (Physics.SphereCast(start, outSideLedgeRadius, dir, out llHit, 1.5f, climbingLayer) == false)
            {
                dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
                Physics.SphereCast(start, outSideLedgeRadius, dir, out llHit, 1.5f, climbingLayer);
            }

            //Left_left
            start = transform.position + transform.forward * -0.5f + transform.up * ledgeSphereUpOffset + transform.right * -0.3f;
            dir = transform.forward;
            Physics.SphereCast(start, insideLedgeRadius, dir, out lrHit, 1.5f, climbingLayer);

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

        //////////////////////////UPLeftAndRight///////////////////////////////////////////////////
        start = ULL.position;
        dir = transform.forward;
        detect = Physics.SphereCast(start, outsideSurfaceRadius, dir, out upLeft_LeftHit, 1.5f, climbingLayer);
        if (detect == false)
        {
            dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
            Physics.SphereCast(start, outsideSurfaceRadius, dir, out upLeft_LeftHit, 1.5f, climbingLayer);
        }

        start = ULR.position;
        if (detect == true)
            dir = transform.forward;
        Physics.SphereCast(start, insideSurfaceRadius, dir, out upLeft_RightHit, 1.5f, climbingLayer);

        start = URR.position;
        dir = transform.forward;
        detect = Physics.SphereCast(start, outsideSurfaceRadius, dir, out upRight_RightHit, 1.5f, climbingLayer);
        if (detect == false)
        {
            dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
            Physics.SphereCast(start, outsideSurfaceRadius, dir, out upRight_RightHit, 1.5f, climbingLayer);
        }

        start = URL.position;
        if (detect == true)
            dir = transform.forward;
        Physics.SphereCast(start, insideSurfaceRadius, dir, out upRight_LeftHit, 1.5f, climbingLayer);

        /////////////////////DownLeftAndRight////////////////////////////////////////////////////////
        start = DLL.position;
        dir = transform.forward;
        detect = Physics.SphereCast(start, outsideSurfaceRadius, dir, out downLeft_LeftHit, 1.5f, climbingLayer);
        if (detect == false)
        {
            dir = Quaternion.AngleAxis(outSideRotateDetectionAngle, transform.up) * dir;
            Physics.SphereCast(start, outsideSurfaceRadius, dir, out downLeft_LeftHit, 1.5f, climbingLayer);
        }

        start = DLR.position;
        if (detect == true)
            dir = transform.forward;
        Physics.SphereCast(start, insideSurfaceRadius, dir, out downLeft_RightHit, 1.5f, climbingLayer);

        start = DRR.position;
        dir = transform.forward;
        detect = Physics.SphereCast(start, outsideSurfaceRadius, dir, out downRight_RightHit, 1.5f, climbingLayer);
        if (detect == false)
        {
            dir = Quaternion.AngleAxis(-outSideRotateDetectionAngle, transform.up) * dir;
            Physics.SphereCast(start, outsideSurfaceRadius, dir, out downRight_RightHit, 1.5f, climbingLayer);
        }

        start = DRL.position;
        if (detect == true)
            dir = transform.forward;
        Physics.SphereCast(start, insideSurfaceRadius, dir, out downRight_LeftHit, 1.5f, climbingLayer);
    }

    private void TraceUp(int left)
    {
        climbingMove = true;
        startLedgeIK = false;
        if (left == 1)
        {
            enableLeftHandIk = true;
            if (leftHandPointObjet == null)
                leftHandPointObjet = CreatePointObject("LeftHandPoint");
            leftHandPointObjet.SetParent(upLeftHit.transform);
            leftHandPointObjet.position = upLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
            if (nextRightHandPointObject == null)
                nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
            nextRightHandPointObject.SetParent(upRightHit.transform);
            nextRightHandPointObject.position = upRightHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
        else
        {       
            enableRightHandIk = true;
            if (rightHandPointObject == null)
                rightHandPointObject = CreatePointObject("RightHandPoint");
            rightHandPointObject.SetParent(upRightHit.transform);
            rightHandPointObject.position = upRightHit.point - transform.TransformDirection(upClimbingIKOffset);
            if (nextLeftHandPointObject == null)
                nextLeftHandPointObject = CreatePointObject("NextLeftHandPoint");
            nextLeftHandPointObject.SetParent(upLeftHit.transform);
            nextLeftHandPointObject.position = upLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
    }

    private void TraceDown(int left)
    {
        climbingMove = true;
        enableLeftHandIk = true;
        enableLeftHandIk = true;
        startLedgeIK = false;
        if (left == 1)
        {
            if (nextLeftHandPointObject == null)
                nextLeftHandPointObject = CreatePointObject("NextLeftHandPoint");
            nextLeftHandPointObject.SetParent(downLeftHit.transform);
            nextLeftHandPointObject.position = downLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
            if (nextRightHandPointObject == null)
                nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
            nextRightHandPointObject.SetParent(downRightHit.transform);
            nextRightHandPointObject.position = downRightHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
        else
        {
            if (nextRightHandPointObject == null)
                nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
            nextRightHandPointObject.SetParent(downRightHit.transform);
            nextRightHandPointObject.position = downRightHit.point - transform.TransformDirection(upClimbingIKOffset);
            if (nextLeftHandPointObject == null)
                nextLeftHandPointObject = CreatePointObject("NextLeftHandPoint");
            nextLeftHandPointObject.SetParent(downLeftHit.transform);
            nextLeftHandPointObject.position = downLeftHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
    }

    private void TraceUpLeft()
    {
        climbingMove = true;
        enableLeftHandIk = true;
        startLedgeIK = false;
        if (leftHandPointObjet == null)
            leftHandPointObjet = CreatePointObject("LeftHandPoint");
        leftHandPointObjet.SetParent(upLeft_LeftHit.transform);
        leftHandPointObjet.position = upLeft_LeftHit.point - transform.TransformDirection(upClimbingIKOffset);
        if (nextRightHandPointObject == null)
            nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
        nextRightHandPointObject.SetParent(upLeft_RightHit.transform);
        nextRightHandPointObject.position = upLeft_RightHit.point - transform.TransformDirection(upClimbingIKOffset);
    }

    private void TraceUpRight()
    {
        climbingMove = true;
        enableRightHandIk = true;
        startLedgeIK = false;
        if (rightHandPointObject == null)
            rightHandPointObject = CreatePointObject("LeftHandPoint");
        rightHandPointObject.SetParent(upRight_RightHit.transform);
        rightHandPointObject.position = upRight_RightHit.point - transform.TransformDirection(upClimbingIKOffset);
        if (nextLeftHandPointObject == null)
            nextLeftHandPointObject = CreatePointObject("NextRightHandPoint");
        nextLeftHandPointObject.SetParent(upRight_LeftHit.transform);
        nextLeftHandPointObject.position = upRight_LeftHit.point - transform.TransformDirection(upClimbingIKOffset);
    }

    private void TraceDownLeft()
    {
        climbingMove = true;
        enableLeftHandIk = true;
        startLedgeIK = false;
        if (nextLeftHandPointObject == null)
            nextLeftHandPointObject = CreatePointObject("NextLeftHandPoint");
        nextLeftHandPointObject.SetParent(downLeft_LeftHit.transform);
        nextLeftHandPointObject.position = downLeft_LeftHit.point - transform.TransformDirection(upClimbingIKOffset);
        enableRightHandIk = true;
        if (nextRightHandPointObject == null)
            nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
        nextRightHandPointObject.SetParent(downLeft_RightHit.transform);
        nextRightHandPointObject.position = downLeft_RightHit.point - transform.TransformDirection(upClimbingIKOffset);
    }

    private void TraceDownRight()
    {
        climbingMove = true;
        enableLeftHandIk = true;
        startLedgeIK = false;
        if (nextRightHandPointObject == null)
            nextRightHandPointObject = CreatePointObject("NextRightHandPoint");
        nextRightHandPointObject.SetParent(downRight_RightHit.transform);
        nextRightHandPointObject.position = downRight_RightHit.point - transform.TransformDirection(upClimbingIKOffset);
        if (nextLeftHandPointObject == null)
            nextLeftHandPointObject = CreatePointObject("NextLeftHandPoint");
        nextLeftHandPointObject.SetParent(downRight_LeftHit.transform);
        nextLeftHandPointObject.position = downRight_LeftHit.point - transform.TransformDirection(upClimbingIKOffset);
    }

    public void TraceCenter()
    {
        bool detected = Physics.SphereCast(center_L.position, outsideSurfaceRadius, transform.forward, out llHit, 1.5f, climbingLayer);
        if(detected == true)
        {
            enableLeftHandIk = true;
            leftHandPointObjet.SetParent(llHit.transform);
            leftHandPointObjet.position = llHit.point - transform.TransformDirection(upClimbingIKOffset);
        }

        detected = Physics.SphereCast(center_R.position, outsideSurfaceRadius, transform.forward, out rrHit, 1.5f, climbingLayer);
        if(detected == true)
        {
            enableRightHandIk = true;
            rightHandPointObject.SetParent(rrHit.transform);
            rightHandPointObject.position = rrHit.point - transform.TransformDirection(upClimbingIKOffset);
        }
    }

    private Transform CreatePointObject(string name)
    {
        return new GameObject(name).transform;
    }

    private void GenerateDetectPoint()
    {
        GameObject llpoint = new GameObject("LeftLeft");
        llpoint.transform.SetParent(transform);
        llpoint.transform.position = transform.position + transform.TransformDirection(leftLeftOffset);
        LL = llpoint.transform;

        GameObject lrpoint = new GameObject("LeftRight");
        lrpoint.transform.SetParent(transform);
        lrpoint.transform.position = transform.position + transform.TransformDirection(leftRightOffset);
        LR = lrpoint.transform;

        GameObject rlpoint = new GameObject("RightLeft");
        rlpoint.transform.SetParent(transform);
        rlpoint.transform.position = transform.position + transform.TransformDirection(rightLeftOffset);
        RL = rlpoint.transform;

        GameObject rrpoint = new GameObject("RightRight");
        rrpoint.transform.SetParent(transform);
        rrpoint.transform.position = transform.position + transform.TransformDirection(rightRightOffset);
        RR = rrpoint.transform;

        GameObject ulpoint = new GameObject("UpLeft");
        ulpoint.transform.SetParent(transform);
        ulpoint.transform.position = transform.position + transform.TransformDirection(upLeftOffset);
        UL = ulpoint.transform;

        GameObject urpoint = new GameObject("UpRight");
        urpoint.transform.SetParent(transform);
        urpoint.transform.position = transform.position + transform.TransformDirection(upRightOffset);
        UR = urpoint.transform;

        GameObject dlpoint = new GameObject("DownLeft");
        dlpoint.transform.SetParent(transform);
        dlpoint.transform.position = transform.position + transform.TransformDirection(downLeftOffset);
        DL = dlpoint.transform;

        GameObject drpoint = new GameObject("DownRight");
        drpoint.transform.SetParent(transform);
        drpoint.transform.position = transform.position + transform.TransformDirection(downRightOffset);
        DR = drpoint.transform;

        GameObject ullpoint = new GameObject("UpLeftLeft");
        ullpoint.transform.SetParent(transform);
        ullpoint.transform.position = transform.position + transform.TransformDirection(upLeftLeftOffset);
        ULL = ullpoint.transform;

        GameObject ulrpoint = new GameObject("UpLeftRight");
        ulrpoint.transform.SetParent(transform);
        ulrpoint.transform.position = transform.position + transform.TransformDirection(upLeftRightOffset);
        ULR = ulrpoint.transform;

        GameObject urlPoint = new GameObject("UpRightLeft");
        urlPoint.transform.SetParent(transform);
        urlPoint.transform.position = transform.position + transform.TransformDirection(upRightLeftOffset);
        URL = urlPoint.transform;

        GameObject urrpoint = new GameObject("UpRightRight");
        urrpoint.transform.SetParent(transform);
        urrpoint.transform.position = transform.position + transform.TransformDirection(upRightRightOffset);
        URR = urrpoint.transform;

        GameObject dllpoint = new GameObject("DownLeftLeft");
        dllpoint.transform.SetParent(transform);
        dllpoint.transform.position = transform.position + transform.TransformDirection(downLeftLeftOffset);
        DLL = dllpoint.transform;

        GameObject dlrpoint = new GameObject("DownLeftRight");
        dlrpoint.transform.SetParent(transform);
        dlrpoint.transform.position = transform.position + transform.TransformDirection(downLeftRightOffset);
        DLR = dlrpoint.transform;

        GameObject drlpoint = new GameObject("DownRightLeft");
        drlpoint.transform.SetParent(transform);
        drlpoint.transform.position = transform.position + transform.TransformDirection(downRightLeftOffset);
        DRL = drlpoint.transform;

        GameObject drrpoint = new GameObject("DownRightRight");
        drrpoint.transform.SetParent(transform);
        drrpoint.transform.position = transform.position + transform.TransformDirection(downRightRightOffset);
        DRR = drrpoint.transform;

        GameObject clpoint = new GameObject("CenterLeft");
        clpoint.transform.SetParent(transform);
        clpoint.transform.position = transform.position + transform.TransformDirection(centerLeftOffset);
        center_L = clpoint.transform;

        GameObject crpoint = new GameObject("CenterRight");
        crpoint.transform.SetParent(transform);
        crpoint.transform.position = transform.position + transform.TransformDirection(centerRightOffset);
        center_R = crpoint.transform;
    }

    public void EnableLeftHandIk() { enableLeftHandIk = true; }
    public void DisableLeftHandIk() { enableLeftHandIk = false; }
    public void EnableRightHandIk() { enableRightHandIk = true; }
    public void DisableRightHandIk() { enableRightHandIk = false; }

    private void OnGUI()
    {
        if (debugHandIK == true)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10f, 320f, 100, 20), "HandIK : " + enableHandIK.ToString(), style);
            GUI.Label(new Rect(10f, 340f, 100, 20), "LeftHandIK : " + enableLeftHandIk.ToString(), style);
            GUI.Label(new Rect(10f, 360f, 100, 20), "RightHandIK : " + enableRightHandIk.ToString(), style);
            GUI.Label(new Rect(10f, 380f, 100, 20), "LeftWeight : " + leftWeight.ToString(), style); ;
            GUI.Label(new Rect(10f, 400f, 100, 20), "RightWeight : " + rightWeight.ToString(), style);
            GUI.Label(new Rect(10f, 420f, 100, 20), "LedgeMove : " + climbingMove.ToString(), style);
            GUI.Label(new Rect(10f, 440f, 100, 20), "StartLedgeIK : " + startLedgeIK.ToString(), style);
        }
    }
}
