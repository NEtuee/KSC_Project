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
    private Transform bip;
    [SerializeField] private Transform pelvis;
    private Vector3 prevPelvisPosition;

    private bool isFixedHand;
    private bool isLeftHandFix;
    private bool isRightHandFix;

    [SerializeField]private Transform hipTransform;

    private Vector3 prevLocalPosition;
    private Vector3 prevWorldPosition;
    private Quaternion prevRotation;

    private Vector3 storedHipsPosition;
    private Vector3 storedHipsPositionPrivAnim;
    private Vector3 storedHipsPositionPrivBlend;

    private float ragdollEndTime;

    private const float ragdollToAnimBlendTime = 0.5f;

    private Vector3 storedPos;

    private Vector3 storedRootPosition;

    [SerializeField]private Transform prevParent;

    [Header("Velocity")]
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed;
    private Vector3 prevPos;
    [Range(0, 10000)] public float fource = 0.0f;

    [Header("Ragdoll Object")]
    [SerializeField] private Transform ragdollObject;
    [SerializeField] private Transform ragdollHip;
    [SerializeField] private Transform ragdollLeftHand;
    [SerializeField] private Transform ragdollRightHand;
    [SerializeField] private List<GameObject> visual = new List<GameObject>();
    [SerializeField] private List<TransformComponent> ragDollTransforms = new List<TransformComponent>();
    [SerializeField] private string blendAnimation;
    [SerializeField] private Animator ragdollAnim;
    [SerializeField] private List<Rigidbody> ragdollObjectRigids = new List<Rigidbody>();

    private GameObject _ragdollContainer;

    void Start()
    {
        anim = GetComponent<Animator>();
        hipTransform = anim.GetBoneTransform(HumanBodyBones.Hips);
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        bip = transform.Find("Bip001");
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
        //transforms.RemoveAt(transforms.Count-1);

        rigidBodies = ragdollObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigid in rigidBodies)
        {
            ragdollObjectRigids.Add(rigid);
        }


        ragdollObject.gameObject.SetActive(false);

        foreach (var t in ragdollHip.GetComponentsInChildren<Transform>())
        {
            var trComp = new TransformComponent(t);
            ragDollTransforms.Add(trComp);
        }

        prevPos = transform.position;

        _ragdollContainer = new GameObject("RagdollContainer " + gameObject.name);
    }

    private void FixedUpdate()
    {
        //if(state == RagdollState.Ragdoll)
        //{
        //    for(int i = 0; i<ragdollRigids.Count;i++)
        //    {
        //        //ragdollRigids[i].velocity = -ragdollObjectRigids[i].velocity;
        //        ragdollRigids[i].transform.SetPositionAndRotation(ragdollObjectRigids[i].transform.position, ragdollObjectRigids[i].transform.rotation);

        //    }
        //}

        if (state == RagdollState.Ragdoll)
        {
            transform.position = hipTransform.position;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            hipTransform.GetComponent<Rigidbody>().AddForce(transform.forward * 10000.0f);
        }

        //if(Input.GetKeyDown(KeyCode.X))
        //{
        //    //Debug.Log(anim.rootPosition);
        //    anim.rootPosition = transform.position;
        //    //anim.applyRootMotion = true;
        //}

        if (Input.GetKeyDown(KeyCode.R))
        {
            switch (state)
            {
                case RagdollState.Animated:
                    //prevPelvisPosition = pelvis.position;
                    //FixeHand(true);

                    hipTransform.SetParent(_ragdollContainer.transform);

                    ActiveRagdoll(true);
                    //GameManager.Instance.PausePlayerControl();
                    
                    //ragdollObject.SetParent(transform.parent);
                    //ragdollObject.SetPositionAndRotation(transform.position, transform.rotation);
                    //CopyAnimCharacterTransformToRagdoll(hipTransform, ragdollHip);
                    //ragdollObject.gameObject.SetActive(true);

                    //FixeHand(true);
                    //ConvertRagdoll();
                    //state = RagdollState.Ragdoll;
                    break;
                case RagdollState.Ragdoll:
                    //FixeHand(false);
                    //ActiveRagdoll(false);

                    hipTransform.SetParent(bip);

                    ReturnAnimated();
                    ragdollObject.gameObject.SetActive(false);

                    //FixeHand(false);
                    //StartRagdollBlend();
                    break;
            }
        }

        if (isFixedHand)
        {
            //leftHandRigidBody.transform.SetPositionAndRotation(leftHandFixedPosition, leftHandFixedRotation);
            //rightHandRigidBody.transform.SetPositionAndRotation(rightHandFixedPosition, rightHandFixedRotation);
            //leftHandRigidBody.transform.position = leftHandFixedPosition;
            //rightHandRigidBody.transform.position = rightHandFixedPosition;

            //leftHandTransform.SetPositionAndRotation(leftHandPoint.position, leftHandPoint.rotation);
            //rightHandTransform.SetPositionAndRotation(rightHandPoint.position, rightHandPoint.rotation);

            //rigidbody.Sleep();
            //Debug.Log(rigidbody.centerOfMass);
            //transform.position = prevWorldPosition;

            //ragdollLeftHand.SetPositionAndRotation(leftHandPoint.position, leftHandPoint.rotation);
            ragdollRightHand.SetPositionAndRotation(rightHandPoint.position, rightHandPoint.rotation);
        }

        if(isLeftHandFix)
        {
            ragdollLeftHand.SetPositionAndRotation(leftHandPoint.position, leftHandPoint.rotation);
        }
        if (isRightHandFix)
        {
            ragdollRightHand.SetPositionAndRotation(rightHandPoint.position, rightHandPoint.rotation);
        }

        if (state == RagdollState.Ragdoll)
        {
            //for (int i = 0; i < ragdollRigids.Count; i++)
            //{
            //    //ragdollRigids[i].velocity = -ragdollObjectRigids[i].velocity;
            //    ragdollRigids[i].transform.SetPositionAndRotation(ragdollObjectRigids[i].transform.position, ragdollObjectRigids[i].transform.rotation);
            //}
        }
    }

    private void LateUpdate()
    {
        if (isFixedHand)
        {
            //transform.position = prevWorldPosition;
            //transform.rotation = prevRotation;
        }

        if (state == RagdollState.BlendToAnim)
        {
            //pelvis.position = prevPelvisPosition;

            float ragdollBlendAmount = 1f - Mathf.InverseLerp(ragdollEndTime, ragdollEndTime + ragdollToAnimBlendTime, Time.time);

            //if (storedHipsPositionPrivBlend != hipTransform.position)
            //{
            //    storedHipsPositionPrivAnim = hipTransform.position;
            //}
            //storedHipsPositionPrivBlend = Vector3.Lerp(storedHipsPositionPrivAnim, storedHipsPosition, ragdollBlendAmount);
            //hipTransform.position = storedHipsPositionPrivBlend;

            foreach (TransformComponent trComp in transforms)
            {
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

            //float ragdollBlendAmount = 1f - Mathf.InverseLerp(ragdollEndTime, ragdollEndTime + ragdollToAnimBlendTime, Time.time);

            //if (storedHipsPositionPrivBlend != ragdollHip.position)
            //{
            //    storedHipsPositionPrivAnim = ragdollHip.position;
            //}
            //storedHipsPositionPrivBlend = Vector3.Lerp(storedHipsPositionPrivAnim, storedHipsPosition, ragdollBlendAmount);
            //ragdollHip.position = storedHipsPositionPrivBlend;

            //foreach (TransformComponent trComp in ragDollTransforms)
            //{
            //    if (trComp.PrivRotation != trComp.Transform.localRotation)
            //    {
            //        trComp.PrivRotation = Quaternion.Slerp(trComp.Transform.localRotation, trComp.StoredRotation, ragdollBlendAmount);
            //        trComp.Transform.localRotation = trComp.PrivRotation;
            //    }

            //    if (trComp.PrivPosition != trComp.Transform.localPosition)
            //    {
            //        trComp.PrivPosition = Vector3.Slerp(trComp.Transform.localPosition, trComp.StoredPosition, ragdollBlendAmount);
            //        trComp.Transform.localPosition = trComp.PrivPosition;
            //    }
            //}

            //if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
            //{
            //    ragdollObject.gameObject.SetActive(false);

            //    transform.SetPositionAndRotation(ragdollObject.position, ragdollObject.rotation);

            //    foreach (var visualObject in visual)
            //    {
            //        visualObject.SetActive(true);
            //    }

            //    collider.enabled = true;



            //    state = RagdollState.Animated;
            //}
        }
    }

    private void ActiveRagdoll(bool active)
    {
        anim.enabled = !active;
        collider.enabled = !active;
        //collider.isTrigger = active;

        foreach (Rigidbody rigid in ragdollRigids)
        {
            Collider collider = rigid.transform.GetComponent<Collider>();

            collider.isTrigger = !active;
            rigid.isKinematic = !active;
        }

        //leftHandRigidBody.isKinematic = true;
        //rightHandRigidBody.isKinematic = true;

        if (active)
        {
            state = RagdollState.Ragdoll;
            prevLocalPosition = transform.localPosition;
            prevWorldPosition = transform.position;
            prevRotation = transform.rotation;
        }
        else
        {
            //state = RagdollState.Animated;
            //transform.localPosition = prevLocalPosition;
        }
    }

    private void ActiveRagdollParts(bool active)
    {
        anim.enabled = !active;
        collider.isTrigger = active;

        foreach (Rigidbody rigid in ragdollRigids)
        {
            Collider collider = rigid.transform.GetComponent<Collider>();

            collider.isTrigger = !active;
            rigid.isKinematic = !active;
        }

        leftHandRigidBody.isKinematic = true;
        rightHandRigidBody.isKinematic = true;

        if (active)
        {
            state = RagdollState.Ragdoll;
            prevLocalPosition = transform.localPosition;
            prevWorldPosition = transform.position;
        }
    }

    private void ConvertRagdoll()
    {
        collider.enabled = false;
        CopyAnimCharacterTransformToRagdoll(hipTransform, ragdollHip);
        ragdollObject.SetParent(transform.parent);
        ragdollObject.SetPositionAndRotation(transform.position, transform.rotation);
        ragdollObject.gameObject.SetActive(true);
        ragdollAnim.enabled = false;

        foreach(var visualObject in visual)
        {
            visualObject.SetActive(false);
        }

        GameManager.Instance.camCtrl.SetTarget(ragdollObject);
    }

    private void StartRagdollBlend()
    {
        ragdollEndTime = Time.time;
        ragdollAnim.enabled = true;
        state = RagdollState.BlendToAnim;
        storedHipsPositionPrivAnim = Vector3.zero;
        storedHipsPositionPrivBlend = Vector3.zero;

        storedHipsPosition = ragdollHip.position;

        ragdollAnim.Play(blendAnimation, 0, 0);

        foreach (TransformComponent trComp in ragDollTransforms)
        {
            trComp.StoredPosition = trComp.Transform.localPosition;
            trComp.PrivPosition = trComp.Transform.localPosition;

            trComp.StoredRotation = trComp.Transform.localRotation;
            trComp.PrivRotation = trComp.Transform.localRotation;
        }

        GameManager.Instance.camCtrl.SetTarget(transform);

        //ActiveRagdoll(false);
    }

    private void ResetTransform()
    {
        transform.localPosition = prevLocalPosition;
    }

    private void FixeHand(bool value)
    {
        isFixedHand = value;
        if(value)
        {
            //leftHandFixedPosition = leftHandRigidBody.transform.position;
            //leftHandFixedRotation = leftHandRigidBody.transform.rotation;
            //rightHandFixedPosition = rightHandRigidBody.transform.position;
            //rightHandFixedRotation = rightHandRigidBody.transform.rotation;
            leftHandPoint.SetPositionAndRotation(leftHandTransform.position, leftHandTransform.rotation);
          
            rightHandPoint.SetPositionAndRotation(rightHandTransform.position, rightHandTransform.rotation);
 
            leftHandPoint.SetParent(transform.parent);
            rightHandPoint.SetParent(transform.parent);
        }
    }

    private void FixLeftHand(bool value)
    {
        isLeftHandFix = value;

        if (isLeftHandFix)
        {
            ragdollLeftHand.GetComponent<Rigidbody>().isKinematic = value;
            leftHandPoint.SetPositionAndRotation(leftHandTransform.position, leftHandTransform.rotation);
            leftHandPoint.SetParent(transform.parent);
        }
    }

    private void FixRightHand(bool value)
    {
        isRightHandFix = value;

        if (isRightHandFix)
        {
            ragdollRightHand.GetComponent<Rigidbody>().isKinematic = value;
            rightHandPoint.SetPositionAndRotation(rightHandTransform.position, rightHandTransform.rotation);
            rightHandPoint.SetParent(transform.parent);
        }
    }

    private void ReturnAnimated()
    {
        //ResetTransform();

        ragdollEndTime = Time.time;
        anim.enabled = true;
        state = RagdollState.BlendToAnim;
        storedHipsPositionPrivAnim = Vector3.zero;
        storedHipsPositionPrivBlend = Vector3.zero;

        storedHipsPosition = hipTransform.position;

        Vector3 shiftPos = hipTransform.position - transform.position;
        shiftPos.y = GetDistanceToFloor(shiftPos.y);

        MoveNodeWithoutChildren(shiftPos);

        foreach(TransformComponent trComp in transforms)
        {
            trComp.StoredPosition = trComp.Transform.localPosition;
            trComp.PrivPosition = trComp.Transform.localPosition;

            trComp.StoredRotation = trComp.Transform.localRotation;
            trComp.PrivRotation = trComp.Transform.localRotation;
        }

        anim.Play(prevPlayAnimation, 0, 0);
        ActiveRagdoll(false);
        //ActiveRagdollParts(false);
    }

    IEnumerator Defferd()
    {
        yield return null;
        //ReturnAnimated();
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
        var hipsPos = hipTransform.position;

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

        //Debug.Log(transform.position);

        // shift character node position without children
        hipTransform.position -= shiftPos;
        transform.position += shiftPos;

        //Debug.Log(transform.position);

        anim.rootPosition = transform.position;
        //Debug.Log(anim.rootPosition);
        storedRootPosition = transform.position;

        // rotate character node without children
        Vector3 forward = transform.forward;
        transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * transform.rotation;
        hipTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * hipTransform.rotation;
    }

    //private void OnAnimatorMove()
    //{
        
    //}
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
