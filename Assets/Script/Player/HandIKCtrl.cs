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
    [SerializeField] private GameObject maker;

    [Header("LedgeIK")]
    [SerializeField] private bool ledgeIK;
    [SerializeField] private bool horizonMove;
    [SerializeField] private bool ledgeDetection = false;
    [SerializeField] private float horizonOffeset = 0.5f;
    [SerializeField] private float centerDetectionOffset = 0.3f;
    private Vector3 leftHandPos;
    private Vector3 rightHandPos;
    private Vector3 nextLeftHandPos;
    private Vector3 nextRightHandPos;
    [SerializeField] private Vector3 leftHandOffset;
    [SerializeField] private Vector3 rightHandOffset;
    [SerializeField] private float normalizedTime;
    [SerializeField] private float upSpeed = 32f;
    [SerializeField] private float downSpeed = 8f;
    [SerializeField] private Vector3 hangLedgeOffset;
    [SerializeField] private float centerDetectRadius = 0.3f;
    [SerializeField] private float detectSphereRadius = 0.3f;

    [SerializeField] private AnimationCurve leftCurve_left;
    [SerializeField] private AnimationCurve rightCurve_left;
    [SerializeField] private AnimationCurve leftCurve_right;
    [SerializeField] private AnimationCurve rightCurve_right;

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
    private RaycastHit llsHit;
    private RaycastHit rrsHit;
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
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    //rightHand_Effetor = animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        //    rightHand_Effetor = rightHand.position;
        //    //rightRot = animator.GetBoneTransform(HumanBodyBones.RightHand).rotation;
        //    rightRot = rightHand.rotation;
        //    enableRightHandIk = true;
        //    Instantiate(sphere, rightHand_Effetor, rightRot);
        //}
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.5f, 0.6f)), transform.TransformDirection(new Vector3(-0.5f, -1.0f, 0.0f)), Color.green);
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.5f, 0.6f)), transform.TransformDirection(new Vector3(0.5f, -1.0f, 0.0f)), Color.green);

        if (enableHandIK == false)
        {
            return;
        }

        //if(leftWeightMove == true)
        //{
        //    leftWeight = Mathf.MoveTowards(leftWeight, 1f, lerpSpeed * Time.deltaTime);
        //    if (leftWeight == 1f)
        //        leftWeightMove = false;
        //}

        //if (rightWeightMove == true)
        //{
        //    rightWeight = Mathf.MoveTowards(rightWeight, 1f, lerpSpeed * Time.deltaTime);
        //    if (rightWeight == 1f)
        //        rightWeightMove = false;
        //}
        normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float abs = (int)normalizedTime;
        normalizedTime = normalizedTime - abs;
        if (animator)
        {
            float valocity = 0.0f;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge.Ledge_Left"))
            {
                //if (normalizedTime < 0.25f)
                //{
                //    //leftWeight = Mathf.SmoothDamp(leftWeight, 0.0f, ref valocity, downSpeed * Time.deltaTime);
                //    //rightWeight = Mathf.SmoothDamp(rightWeight, 1.0f, ref valocity, upSpeed * Time.deltaTime);

                //    leftWeight = Mathf.MoveTowards(leftWeight, 0.0f, downSpeed * Time.deltaTime);
                //    rightWeight = Mathf.MoveTowards(rightWeight, 1.0f, upSpeed * Time.deltaTime);
                //}
                //else if (normalizedTime > 0.25f && normalizedTime < 0.46f)
                //{
                //    //leftWeight = Mathf.SmoothDamp(leftWeight, 1.0f, ref valocity, upSpeed * Time.deltaTime);
                //    //rightWeight = Mathf.SmoothDamp(rightWeight, 0.0f, ref valocity, downSpeed * Time.deltaTime);

                //    leftWeight = Mathf.MoveTowards(leftWeight, 1.0f, upSpeed * Time.deltaTime);
                //    rightWeight = Mathf.MoveTowards(rightWeight, 0.0f, downSpeed * Time.deltaTime);
                //}
                leftWeight = leftCurve_left.Evaluate(normalizedTime);
                rightWeight = leftCurve_right.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge.Ledge_Right"))
            {
                //if (normalizedTime < 0.25f)
                //{
                //    //rightWeight = Mathf.SmoothDamp(rightWeight, 0.0f, ref valocity, downSpeed * Time.deltaTime);
                //    //leftWeight = Mathf.SmoothDamp(leftWeight, 1.0f, ref valocity, upSpeed * Time.deltaTime);
                //    rightWeight = Mathf.MoveTowards(rightWeight, 0.0f, downSpeed * Time.deltaTime);
                //    leftWeight = Mathf.MoveTowards(leftWeight, 1.0f, upSpeed * Time.deltaTime);
                //}
                //else if (normalizedTime > 0.25f && normalizedTime < 0.46f)
                //{
                //    //rightWeight = Mathf.SmoothDamp(rightWeight, 1.0f, ref valocity, upSpeed * Time.deltaTime);
                //    //leftWeight = Mathf.SmoothDamp(leftWeight, 0.0f, ref valocity, downSpeed * Time.deltaTime);
                //    rightWeight = Mathf.MoveTowards(rightWeight, 1.0f, upSpeed * Time.deltaTime);
                //    leftWeight = Mathf.MoveTowards(leftWeight, 0.0f, downSpeed * Time.deltaTime);
                //}
                leftWeight = rightCurve_left.Evaluate(normalizedTime);
                rightWeight = rightCurve_right.Evaluate(normalizedTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge.LedgeIdle"))
            {
                horizonMove = false;
                leftWeight = 1f;
                rightWeight = 1f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (enableHandIK == false)
        {
            return;
        }

        if(ledgeIK)
        {
            LedgeIKPosDetection();
            SideDetect();
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

        if (leftTrace == true)
        {
            //LeftHandIKTrace();
        }

        if (rightTrace == true)
        {
            //RightHandIKTrace();
        }
    }

    public void ActiveLedgeIK()
    {
        ledgeIK = true;
        ledgeDetection = true;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(ledgeIK == true)
        {
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
            

            return;
        }

        if(enableLeftHandIk == true)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand_Effetor);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
        }

        if (enableRightHandIk == true)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand_Effetor);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        }
    }

    private void LeftTrace()
    {
        horizonMove = true;
        ledgeDetection = false;
        //leftHandPos = SetLedgeIKDetect(leftHandPos + (-transform.right) * horizonOffeset, true);
        //nextRightHandPos = SetLedgeIKNextHand(rightHandPos + (-transform.right) * horizonOffeset);

        leftHandPos = llHit.point - transform.TransformDirection(hangLedgeOffset);

        //if (ll_detect == true)
        //{
        //    leftHandPos = llHit.point - transform.TransformDirection(hangLedgeOffset);
        //}
        //else
        //{
        //    leftHandPos = llsHit.point - transform.TransformDirection(hangLedgeOffset);
        //}
        nextRightHandPos = lrHit.point - transform.TransformDirection(hangLedgeOffset);      
    }

    private void UpdateLeftHandPos()
    {
        leftHandPos = nextLeftHandPos;
    }

    private void RightTrace()
    {
        horizonMove = true;
        ledgeDetection = false;
        //rightHandPos = SetLedgeIKDetect(rightHandPos + (transform.right) * horizonOffeset, false);
        //if(rightSide == true)
        //{
        //    rightHandPos = rightHit.point;
        //}
        //else
        //{
        //    //rightHandPos = SetLedgeIKDetect(rightHandPos + (transform.right) * horizonOffeset, false);
        //    DetectIKPosition(false);
        //}

        //nextLeftHandPos = SetLedgeIKNextHand(leftHandPos + (transform.right) * horizonOffeset);
        rightHandPos = rrHit.point - transform.TransformDirection(hangLedgeOffset);
        //if (rr_detect == true)
        //{
        //    rightHandPos = rrHit.point - transform.TransformDirection(hangLedgeOffset);
        //}
        //else
        //{
        //    rightHandPos = rrsHit.point - transform.TransformDirection(hangLedgeOffset);
        //}
        nextLeftHandPos = rlHit.point - transform.TransformDirection(hangLedgeOffset);
    }

    private void UpdateRightHandPos()
    {
        rightHandPos = nextRightHandPos;
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

    public void LeftHandIKTrace()
    {
        RaycastHit hit;
        if (0!=Physics.OverlapSphereNonAlloc(leftHandTr.position, sphereRadius, hitColliders,climbingLayer))
        {
            Debug.Log("detectleft");
            if(Physics.Raycast(leftHandTr.position,transform.forward,out hit, 0.5f,climbingLayer))
            {
                leftHand_Effetor = hit.point;
                leftWeightMove = true;
            }
            leftTrace = false;
            leftWeight = 0.0f;
        }
    }

    public void RightHandIKTrace()
    {
        RaycastHit hit;
        if (0 != Physics.OverlapSphereNonAlloc(rightHandTr.position, sphereRadius, hitColliders, climbingLayer))
        {
            Debug.Log("detectright");
            if (Physics.Raycast(rightHandTr.position, transform.forward, out hit, 0.5f, climbingLayer))
            {
                rightHand_Effetor = hit.point;
                rightWeightMove = true;
            }
            rightTrace = false;
            rightWeight = 0.0f;
        }
    }

    public void LeftEnd()
    {
        RaycastHit hit;
        leftWeight = 0.0f;
        leftTrace = false;
        if (Physics.Raycast(leftHandTr.position, transform.forward, out hit, 1f, climbingLayer))
        {
            leftHand_Effetor = hit.point;
            leftWeightMove = true;
        }
    }

    public void RightEnd()
    {
        RaycastHit hit;
        rightWeight = 0.0f;
        rightTrace = false;
        if (Physics.Raycast(leftHandTr.position, transform.forward, out hit, 1f, climbingLayer))
        {
            rightHand_Effetor = hit.point;
            rightWeightMove = true;
        }
    }

    public void WeigtZero()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.0f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.0f);
    }

    public void SetIKPositionWeight(AvatarIKGoal goal, float weight)
    {
        switch(goal)
        {
            case AvatarIKGoal.LeftHand:
                {
                    leftWeight = weight;

                }
                break;
            case AvatarIKGoal.RightHand:
                {
                    rightWeight = weight;
                }
                break;
        }
    }

    

    public void Maker(bool left)
    {
        RaycastHit hit;
        if (left)
        {
            if (Physics.Raycast(LL.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
            if (Physics.Raycast(LR.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
        }
        else
        {
            if (Physics.Raycast(RL.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
            if (Physics.Raycast(RR.position, transform.forward, out hit, 0.5f, climbingLayer))
                Instantiate(maker, hit.point, Quaternion.identity);
        }
    }


    private void OnDrawGizmos()
    {
        DebugSphere();

        Gizmos.color = Color.red;
        if(leftHandTr != null)
            Gizmos.DrawWireSphere(leftHandTr.position, sphereRadius);
        if (rightHandTr != null)
            Gizmos.DrawWireSphere(rightHandTr.position, sphereRadius);
    }

    private void DebugSphere()
    {
        RaycastHit hit;

        Vector3 start = rightHandPos + transform.right * 0.5f + transform.forward * 0.0f + transform.up * 0.1f;
        //Vector3 start = rightHandPos + transform.TransformDirection(new Vector3(0.5f, 0.1f, 0.1f));
        Vector3 dir = -transform.right;
        bool isHit = Physics.SphereCast(start, 0.3f, dir, out hit, 0.5f, climbingLayer);
        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, 0.3f);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 0.5f);
        }

        start = leftHandPos + transform.right * -0.5f + transform.forward * 0.0f + transform.up * 0.1f;
        //Vector3 start = rightHandPos + transform.TransformDirection(new Vector3(0.5f, 0.1f, 0.1f));
        dir = transform.right;
        isHit = Physics.SphereCast(start, 0.3f, dir, out hit, 0.5f, climbingLayer);
        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, 0.3f);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 0.5f);
        }


        start =  transform.position + transform.up * 1.0f;
        //Vector3 start = rightHandPos + transform.TransformDirection(new Vector3(0.5f, 0.1f, 0.1f));
        dir = transform.forward;
        isHit = Physics.SphereCast(start, 0.3f, dir, out hit, 0.3f, climbingLayer);
        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, 0.3f);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 0.5f);
        }

        /////////////////DetectLedgeIKPosition//////////////////////////////

        Vector3 rightNormal = transform.forward;
        Vector3 leftNormal = transform.forward;

        start = transform.position + transform.forward *-0.5f+ transform.up * 1.0f + transform.right* centerDetectionOffset;
        dir = transform.forward;
        isHit = Physics.SphereCast(start, centerDetectRadius, dir, out hit, 1.0f, climbingLayer);
        Gizmos.color = Color.blue;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);
            Gizmos.color = Color.green;
            rightNormal = hit.normal;
            rightNormal.y = 0;
            rightNormal.Normalize();
            Gizmos.DrawRay(hit.point, rightNormal * 1f);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 1.0f);
        }

        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -centerDetectionOffset;
        dir = transform.forward;
        isHit = Physics.SphereCast(start, centerDetectRadius, dir, out hit, 1.0f, climbingLayer);
        Gizmos.color = Color.blue;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);
            Gizmos.color = Color.green;
            leftNormal = hit.normal;
            leftNormal.y = 0;
            leftNormal.Normalize();
            Gizmos.DrawRay(hit.point, leftNormal * 1f);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 1.0f);
        }

        //RR
        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.7f;
        //dir = -rightNormal;
        dir = transform.forward;
        isHit = Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer);
        Gizmos.color = Color.white;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);

        }
        else
        {
            dir = Quaternion.AngleAxis(-20f, transform.up) * dir;
            if (Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer))
            {
                Gizmos.DrawRay(start, dir * hit.distance);
                Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);
            }
            else
            {
                Gizmos.DrawRay(start, dir * 1.5f);
            }
        }

        //RR_Support
        //dir = Quaternion.AngleAxis(-30f, transform.up) * dir;
        //isHit = Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer);
        //Gizmos.color = Color.magenta;
        //if (isHit)
        //{
        //    Gizmos.DrawRay(start, dir * hit.distance);
        //    Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);

        //}
        //else
        //{
        //    Gizmos.DrawRay(start, dir * 1.5f);
        //}

        start = transform.position + transform.forward * -0.5f+transform.up * 1.0f + transform.right * 0.3f;
        //dir = -rightNormal;
        dir = transform.forward;
        isHit = Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer);
        Gizmos.color = Color.white;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);

        }
        else
        {
            Gizmos.DrawRay(start, dir * 1.5f);
        }

        //LL
        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.7f;
        //dir = -leftNormal;
        dir = transform.forward;
        isHit = Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer);
        Gizmos.color = Color.white;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 1.5f);
        }

        //LL_Support
        //dir = Quaternion.AngleAxis(30f, transform.up) * dir;
        //isHit = Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer);
        //Gizmos.color = Color.magenta;
        //if (isHit)
        //{
        //    Gizmos.DrawRay(start, dir * hit.distance);
        //    Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);

        //}
        //else
        //{
        //    Gizmos.DrawRay(start, dir * 1.5f);
        //}

        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.3f;
        //dir = -leftNormal;
        dir = transform.forward;
        isHit = Physics.SphereCast(start, detectSphereRadius, dir, out hit, 1.5f, climbingLayer);
        Gizmos.color = Color.white;
        if (isHit)
        {
            Gizmos.DrawRay(start, dir * hit.distance);
            Gizmos.DrawWireSphere(start + dir * hit.distance, detectSphereRadius);
        }
        else
        {
            Gizmos.DrawRay(start, dir * 1.5f);
        }
    }
    
    private void DetectIKPosition(bool left)
    {
        Vector3 leftPoint, rightPoint;
        if(left)
        {
            leftPoint = transform.position + transform.up * 1.0f + transform.right * -0.7f;
            rightPoint = transform.position + transform.up * 1.0f + transform.right * -0.3f;
            leftHandPos = leftHandPos + (-transform.right) * horizonOffeset;
            rightHandPos = rightHandPos + (-transform.right) * horizonOffeset;
        }
        else
        {
            leftPoint = transform.position + transform.up * 1.0f + transform.right * 0.3f;
            rightPoint = transform.position + transform.up * 1.0f + transform.right * 0.7f;
            leftHandPos = leftHandPos + (transform.right) * horizonOffeset;
            rightHandPos = rightHandPos + (transform.right) * horizonOffeset;
        }

        RaycastHit lefthit;
        if(Physics.SphereCast(leftPoint, 0.3f, transform.forward, out lefthit, 0.5f, climbingLayer))
        {
            Debug.Log("LeftDetect");
            leftHandPos = lefthit.point;
        }

        RaycastHit rightHit;
        if (Physics.SphereCast(rightPoint, 0.3f, transform.forward, out rightHit, 0.5f, climbingLayer))
        {
            Debug.Log("RightDetect");
            rightHandPos = rightHit.point;
        }

    }

    private Vector3 SetLedgeIKDetect(Vector3 targetPosition, bool left)
    {
        bool retry = (0 == Physics.OverlapSphereNonAlloc(targetPosition, sphereRadius, hitColliders, climbingLayer));
        
        if(retry == false)
            return targetPosition;

        RaycastHit hit;
        Vector3 start;
        Vector3 dir;
        if (left)
        {
           start = leftHandPos + transform.right * -0.5f + transform.forward * 0.0f + transform.up * 0.1f;
           dir = transform.right;
           if(Physics.SphereCast(start, 0.3f, dir, out hit, 0.5f, climbingLayer))
           {
                return hit.point;
           }
        }
        else
        {
            start = rightHandPos + transform.right * 0.5f + transform.forward * 0.0f + transform.up * 0.1f;
            dir = -transform.right;
            if (Physics.SphereCast(start, 0.3f, dir, out hit, 0.5f, climbingLayer))
            {
                return hit.point;
            }
        }
        return targetPosition;
    }

    private Vector3 SetLedgeIKNextHand(Vector3 targetPosition)
    {
        bool retry = (0 == Physics.OverlapSphereNonAlloc(targetPosition, sphereRadius, hitColliders, climbingLayer));

        if (retry == false)
            return targetPosition;

        RaycastHit hit;
        Vector3 start = transform.position + transform.up * 1.2f;
        Vector3 dir = transform.forward;
        if(Physics.SphereCast(start, 0.3f, dir, out hit, 0.3f, climbingLayer))
        {
            return hit.point;
        }
        return targetPosition;
    }

    private void SideDetect()
    {
        Vector3 start = rightHandPos + transform.right * 0.5f + transform.forward * 0.0f + transform.up * 0.1f;
        Vector3 dir = -transform.right;
        rightSide = Physics.SphereCast(start, 0.3f, dir, out rightHit, 0.5f, climbingLayer);
        
        start = leftHandPos + transform.right * -0.5f + transform.forward * 0.0f + transform.up * 0.1f;
        dir = transform.right;
        leftSide = Physics.SphereCast(start, 0.3f, dir, out leftHit, 0.5f, climbingLayer);
    }

    private void LedgeIKPosDetection()
    {
        if (horizonMove == true)
            return;

        Vector3 rightNormal = transform.forward;
        Vector3 leftNormal = transform.forward;

        Vector3 start;
        Vector3 dir;

        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * centerDetectionOffset;
        dir = transform.forward;
        if(Physics.SphereCast(start, centerDetectRadius, dir, out forwardRightHit, 1.0f, climbingLayer))
        {
            rightNormal = forwardRightHit.normal;
            rightNormal.y = 0;
            rightNormal.Normalize();
        }

        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -centerDetectionOffset;
        dir = transform.forward;
        if(Physics.SphereCast(start, centerDetectRadius, dir, out forwardLeftHit, 1.0f, climbingLayer))
        {
            leftNormal = forwardLeftHit.normal;
            leftNormal.y = 0;
            leftNormal.Normalize();
        }
       
        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.7f;
        //dir = -rightNormal;
        dir = transform.forward;
        //rr_detect = Physics.SphereCast(start, detectSphereRadius, dir, out rrHit, 1.5f, climbingLayer);
        if (Physics.SphereCast(start, detectSphereRadius, dir, out rrHit, 1.5f, climbingLayer) == false)
        {
            dir = Quaternion.AngleAxis(-25f, transform.up) * dir;
            Physics.SphereCast(start, detectSphereRadius, dir, out rrHit, 1.5f, climbingLayer);
        }
        
        //dir = Quaternion.AngleAxis(-20f, transform.up) * dir;
        //Physics.SphereCast(start, detectSphereRadius, dir, out rrsHit,1.5f, climbingLayer);
       
        
        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * 0.3f;
        //dir = -rightNormal;
        dir = transform.forward;
        rl_detect = Physics.SphereCast(start, detectSphereRadius, dir, out rlHit, 1.5f, climbingLayer);
        
        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.7f;
        //dir = -leftNormal;
        dir = transform.forward;
        if(Physics.SphereCast(start, detectSphereRadius, dir, out llHit, 1.5f, climbingLayer) == false)
        {
            dir = Quaternion.AngleAxis(25f, transform.up) * dir;
            Physics.SphereCast(start, detectSphereRadius, dir, out llHit, 1.5f, climbingLayer);
        }

        //dir = Quaternion.AngleAxis(20f, transform.up) * dir;
        //Physics.SphereCast(start, detectSphereRadius, dir, out llsHit, 1.5f, climbingLayer);

        start = transform.position + transform.forward * -0.5f + transform.up * 1.0f + transform.right * -0.3f;
        //dir = -leftNormal;
        dir = transform.forward;
        lr_detect = Physics.SphereCast(start, detectSphereRadius, dir, out lrHit, 1.5f, climbingLayer);
    }
}
