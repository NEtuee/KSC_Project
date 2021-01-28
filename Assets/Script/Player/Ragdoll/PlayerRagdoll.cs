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
    [SerializeField] private Transform pelvis;
    private Vector3 prevPelvisPosition;

    private bool isFixedHand;

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

    void Start()
    {
        anim = GetComponent<Animator>();
        hipTransform = anim.GetBoneTransform(HumanBodyBones.Hips);
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        Rigidbody[] rigidBodies = transform.Find("Bip001").GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigid in rigidBodies)
        {
            if(rigid.transform == transform)
            {
                continue;
            }

            ragdollRigids.Add(rigid);
        }

        foreach(var t in pelvis.GetComponentsInChildren<Transform>())
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            switch (state)
            {
                case RagdollState.Animated:
                    //prevPelvisPosition = pelvis.position;
                    FixeHand(true);
                    ActiveRagdoll(true);

                    //CopyAnimCharacterTransformToRagdoll(hipTransform, ragdollHip);

                    ragdollObject.gameObject.SetActive(true);

                    //FixeHand(true);
                    //ConvertRagdoll();
                    //state = RagdollState.Ragdoll;
                    break;
                case RagdollState.Ragdoll:
                    //FixeHand(false);
                    //ActiveRagdoll(false);
                    ReturnAnimated();

                    //FixeHand(false);
                    //StartRagdollBlend();
                    break;
            }
        }

        if (state == RagdollState.Ragdoll)
        {
            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                //ragdollRigids[i].velocity = -ragdollObjectRigids[i].velocity;
                ragdollRigids[i].transform.SetPositionAndRotation(ragdollObjectRigids[i].transform.position, ragdollObjectRigids[i].transform.rotation);

            }
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
            transform.localPosition = prevLocalPosition;
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
    
    private void ReturnAnimated()
    {
        //ResetTransform();

        ragdollEndTime = Time.time;
        anim.enabled = true;
        state = RagdollState.BlendToAnim;
        storedHipsPositionPrivAnim = Vector3.zero;
        storedHipsPositionPrivBlend = Vector3.zero;

        storedHipsPosition = hipTransform.position;

        anim.Play(prevPlayAnimation,0,0);

        foreach(TransformComponent trComp in transforms)
        {
            trComp.StoredPosition = trComp.Transform.localPosition;
            trComp.PrivPosition = trComp.Transform.localPosition;

            trComp.StoredRotation = trComp.Transform.localRotation;
            trComp.PrivRotation = trComp.Transform.localRotation;
        }

        ActiveRagdoll(false);
        //ActiveRagdollParts(false);
    }

    IEnumerator Defferd()
    {
        yield return null;
        ReturnAnimated();
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
