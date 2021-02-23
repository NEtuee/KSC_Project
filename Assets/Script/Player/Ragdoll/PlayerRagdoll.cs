using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    public enum RagdollState
    { Animated, Ragdoll ,BlendToAnim}
    private Animator anim;
    private Collider collider;
    private Rigidbody rigidbody;

    [SerializeField] public RagdollState state;
    [SerializeField] private Rigidbody leftHandRigidBody;
    [SerializeField] private Rigidbody rightHandRigidBody;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField]private List<Rigidbody> ragdollRigids = new List<Rigidbody>();
    [SerializeField]private List<TransformComponent> transforms = new List<TransformComponent>();

    [SerializeField] private Transform leftHandPoint;
    [SerializeField] private Transform rightHandPoint;

    [SerializeField] private string prevPlayAnimation;
    [SerializeField] private string standUpBackAnimation;
    [SerializeField] private string standUpBallyAnimation;
    private Transform bip;
    [SerializeField] private Transform pelvis;

    private bool isLeftHandFix;
    private bool isRightHandFix;
    private bool isFlyRagdoll;

    [SerializeField]private Transform hipTransform;

    private Vector3 storedHipsPosition;
    private Vector3 storedHipsPositionPrivAnim;
    private Vector3 storedHipsPositionPrivBlend;

    private float ragdollEndTime;
    private const float ragdollToAnimBlendTime = 0.5f;

    [Header("Velocity")]
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed;
    private Vector3 prevPos;
    [Range(0, 10000)] public float fource = 0.0f;

    private GameObject _ragdollContainer;
    private PlayerCtrl_Ver2 player;

    void Start()
    {
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        player = GetComponent<PlayerCtrl_Ver2>();

       
        hipTransform = anim.GetBoneTransform(HumanBodyBones.Hips);

        rightHandTransform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        rightHandRigidBody = rightHandTransform.GetComponent<Rigidbody>();
        leftHandTransform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        leftHandRigidBody = leftHandTransform.GetComponent<Rigidbody>();

        bip = transform.Find("Root_001");
        Rigidbody[] rigidBodies = bip.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigid in rigidBodies)
        {
            if (rigid.transform == transform)
            {
                continue;
            }

            ragdollRigids.Add(rigid);
        }

        foreach (var t in pelvis.GetComponentsInChildren<Transform>())
        {
            var trComp = new TransformComponent(t);
            transforms.Add(trComp);
        }

        _ragdollContainer = new GameObject("RagdollContainer " + gameObject.name);
        CreateHandPoint();
    }

    private void FixedUpdate()
    {
        if(isFlyRagdoll == true && hipTransform.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            ReturnAnimated();
        }

        if (state == RagdollState.Ragdoll && isLeftHandFix == false && isRightHandFix == false)
        {
            transform.position = hipTransform.position;
        }


        if (isLeftHandFix)
        {
            leftHandTransform.SetPositionAndRotation(leftHandPoint.position, leftHandPoint.rotation);
        }
        if (isRightHandFix)
        {
            rightHandTransform.SetPositionAndRotation(rightHandPoint.position, rightHandPoint.rotation);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if (state == RagdollState.Animated)
                ActiveRightHandFixRagdoll();
            else
                DisableFixRagdoll();
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    switch (state)
        //    {
        //        case RagdollState.Animated:

        //            ActiveRightHandFixRagdoll();

        //            break;
        //        case RagdollState.Ragdoll:

        //            DisableFixRagdoll();

        //            break;
        //    }
        //}
    }

    private void LateUpdate()
    {
        if (state == RagdollState.BlendToAnim)
        {
            float ragdollBlendAmount = 1f - Mathf.InverseLerp(ragdollEndTime, ragdollEndTime + ragdollToAnimBlendTime, Time.time);

            //if (storedHipsPositionPrivBlend != hipTransform.position)
            //{
            //    storedHipsPositionPrivAnim = hipTransform.position;
            //}
            //storedHipsPositionPrivBlend = Vector3.Lerp(storedHipsPositionPrivAnim, storedHipsPosition, ragdollBlendAmount);
            //hipTransform.position = storedHipsPositionPrivBlend;

            foreach (TransformComponent trComp in transforms)
            {
                if(trComp.Transform == pelvis)
                {
                    continue;
                }

                if (trComp.PrivRotation != trComp.Transform.localRotation)
                {
                    trComp.PrivRotation = Quaternion.Slerp(trComp.Transform.localRotation, trComp.StoredRotation, ragdollBlendAmount);
                    trComp.Transform.localRotation = trComp.PrivRotation;
                }

                if (trComp.PrivPosition != trComp.Transform.localPosition)
                {
                    trComp.PrivPosition = Vector3.Slerp(trComp.Transform.localPosition, trComp.StoredPosition, ragdollBlendAmount);
                    trComp.Transform.localPosition = trComp.PrivPosition;
                }
            }

            if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
            {
                state = RagdollState.Animated;
            }
        }
    }

    public void ActiveLeftHandFixRagdoll()
    {
        //GameManager.Instance.PauseControl(true);
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Ragdoll);
        FixLeftHand(true);
        ActiveRagdoll(true);
        SetRagdollContainer(true);
    }

    public void ActiveRightHandFixRagdoll()
    {
        //GameManager.Instance.PauseControl(true);
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Ragdoll);
        FixRightHand(true);
        ActiveRagdoll(true);
        SetRagdollContainer(true);
    }

    public void ReleaseFix()
    {
        FixRightHand(false);
        FixLeftHand(false);
    }

    public void ActiveBothHandFixRagdoll()
    {
        //GameManager.Instance.PauseControl(true);
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Ragdoll);
        FixLeftHand(true);
        FixRightHand(true);
        ActiveRagdoll(true);
        SetRagdollContainer(true);
    }

    public void DisableFixRagdoll()
    {
        ReturnAnimated();
        FixRightHand(false);
        FixLeftHand(false);
        player.BackPrevState();
    }

    public void FlyRagdoll()
    {
        //GameManager.Instance.PauseControl(true);
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Ragdoll);
        isFlyRagdoll = true;
        ActiveRagdoll(true);
        SetRagdollContainer(true);
    }

    public void SlidingRagdoll(Vector3 dir)
    {
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Ragdoll);
        isFlyRagdoll = true;
        ActiveRagdoll(true);
        SetRagdollContainer(true);
        anim.GetBoneTransform(HumanBodyBones.Head).GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
    }

    public void ExplosionRagdoll(float power,Vector3 exlosionPos, float radius)
    {
        //GameManager.Instance.PauseControl(true);
        isFlyRagdoll = true;
        ActiveRagdoll(true);
        SetRagdollContainer(true);
        hipTransform.GetComponent<Rigidbody>().velocity = (hipTransform.position - exlosionPos).normalized;
        hipTransform.GetComponent<Rigidbody>().AddForce(((hipTransform.position - exlosionPos).normalized+Vector3.up ).normalized*power,ForceMode.Impulse);
        //hipTransform.GetComponent<Rigidbody>().AddExplosionForce(power, exlosionPos, radius,100.0f);
        InputManager.Instance.GamePadSetVibrate(0.5f, 0.8f);
        if(player != null)
        {
            player.ChangeState(PlayerCtrl_Ver2.PlayerState.Ragdoll);
        }
    }

    public RagdollState GetRagdollState() { return state; }

    private void SetRagdollContainer(bool result)
    {
        if (_ragdollContainer == null)
            CreateRagdollContainer();

        if(result)
        {
            hipTransform.parent.SetParent(_ragdollContainer.transform);
        }
        else
        {
            hipTransform.parent.SetParent(bip);
        }
    }

    private void CreateRagdollContainer()
    {
        _ragdollContainer = new GameObject("RagdollContainer " + gameObject.name);
    }

    private void CreateHandPoint()
    {
        GameObject leftHandPointObject = new GameObject("LeftHandPoint");
        GameObject rightHandPointObject = new GameObject("RightHandPoint");
        leftHandPoint = leftHandPointObject.transform;
        rightHandPoint = rightHandPointObject.transform;
    }

    private void ActiveRagdoll(bool active)
    {
        anim.enabled = !active;
        collider.enabled = !active;

        foreach (Rigidbody rigid in ragdollRigids)
        {
            Collider collider = rigid.transform.GetComponent<Collider>();

            collider.isTrigger = !active;
            rigid.isKinematic = !active;
        }

        if(isLeftHandFix == true)
        {
            leftHandRigidBody.isKinematic = true;
        }
        if(isRightHandFix == true)
        {
            rightHandRigidBody.isKinematic = true;
        }

        if (active)
        {
            state = RagdollState.Ragdoll;
        }
    }
    private void FixLeftHand(bool value)
    {
        isLeftHandFix = value;

        if (isLeftHandFix)
        {
            leftHandRigidBody.isKinematic = value;
            leftHandPoint.SetPositionAndRotation(leftHandTransform.position, leftHandTransform.rotation);
            leftHandPoint.SetParent(transform.parent);
        }
    }

    private void FixRightHand(bool value)
    {
        isRightHandFix = value;

        if (isRightHandFix)
        {
            rightHandRigidBody.isKinematic = value;
            rightHandPoint.SetPositionAndRotation(rightHandTransform.position, rightHandTransform.rotation);
            rightHandPoint.SetParent(transform.parent);
        }
    }

    private void ReturnAnimated()
    {
        SetRagdollContainer(false);

        ragdollEndTime = Time.time;
        anim.enabled = true;
        state = RagdollState.BlendToAnim;
        storedHipsPositionPrivAnim = Vector3.zero;
        storedHipsPositionPrivBlend = Vector3.zero;

        storedHipsPosition = hipTransform.position;

        if (isLeftHandFix == false && isRightHandFix == false)
        {
            Vector3 shiftPos = hipTransform.position - transform.position;
            shiftPos.y = GetDistanceToFloor(shiftPos.y);

            MoveNodeWithoutChildren(shiftPos);
        }

        foreach(TransformComponent trComp in transforms)
        {
            trComp.StoredPosition = trComp.Transform.localPosition;
            trComp.PrivPosition = trComp.Transform.localPosition;

            trComp.StoredRotation = trComp.Transform.localRotation;
            trComp.PrivRotation = trComp.Transform.localRotation;
        }

        if (isFlyRagdoll == false)
        {
            anim.Play(prevPlayAnimation, 0, 0);
        }
        else
        {
            isFlyRagdoll = false;
            string getUpAnimation = CheckIfLieOnBack() ? standUpBackAnimation : standUpBallyAnimation;
            anim.Play(getUpAnimation, 0, 0);
        }
        ActiveRagdoll(false);
    }

    private void CopyAnimCharacterTransformToRagdoll(Transform origin, Transform rag)
    {
        for (int i = 0; i < origin.transform.childCount; i++)
        {
            if (origin.transform.childCount != 0)
            {
                CopyAnimCharacterTransformToRagdoll(origin.transform.GetChild(i), rag.transform.GetChild(i));
            }
            rag.transform.GetChild(i).localPosition = origin.transform.GetChild(i).localPosition;
            rag.transform.GetChild(i).localRotation = origin.transform.GetChild(i).localRotation;
        }
    }

    private bool CheckIfLieOnBack()
    {
        var left = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
        var right = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
        var hipsPos = hipTransform.parent.position;

        left -= hipsPos;
        left.y = 0f;
        right -= hipsPos;
        right.y = 0f;

        var q = Quaternion.FromToRotation(left, Vector3.right);
        var t = q * right;

        return t.z < 0f;
    }

    private Vector3 GetRagdollDirection()
    {
        Vector3 ragdolledFeetPosition = (
            anim.GetBoneTransform(HumanBodyBones.Hips).position);// +
                                                                  //_anim.GetBoneTransform(HumanBodyBones.RightToes).position) * 0.5f;
        Vector3 ragdolledHeadPosition = anim.GetBoneTransform(HumanBodyBones.Head).position;
        Vector3 ragdollDirection = ragdolledFeetPosition - ragdolledHeadPosition;
        ragdollDirection.y = 0;
        ragdollDirection = ragdollDirection.normalized;

        if (CheckIfLieOnBack())
            return ragdollDirection;
        else
            return -ragdollDirection;
    }

    private float GetDistanceToFloor(float currentY)
    {
        RaycastHit[] hits = Physics.RaycastAll(new Ray(hipTransform.position, Vector3.down));
        float distFromFloor = float.MinValue;

        foreach (RaycastHit hit in hits)
            if (!hit.transform.IsChildOf(transform))
                distFromFloor = Mathf.Max(distFromFloor, hit.point.y);

        if (Mathf.Abs(distFromFloor - float.MinValue) > Mathf.Epsilon)
            currentY = distFromFloor - transform.position.y;

        return currentY;
    }

    private void MoveNodeWithoutChildren(Vector3 shiftPos)
    {
        Vector3 ragdollDirection = GetRagdollDirection();

        hipTransform.position -= shiftPos;
        transform.position += shiftPos;

        Vector3 forward = transform.forward;
        transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * transform.rotation;
        hipTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * hipTransform.rotation;
    }
}

[Serializable]
sealed class TransformComponent
{
    public  Transform Transform;
    public Quaternion PrivRotation;
    public Quaternion StoredRotation;

    public Vector3 PrivPosition;
    public Vector3 StoredPosition;

    public TransformComponent(Transform t)
    {
        Transform = t;
    }
}
