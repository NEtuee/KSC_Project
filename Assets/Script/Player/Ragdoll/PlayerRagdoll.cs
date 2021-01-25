using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    public enum RagdollState
    { Animated, Ragdoll}
    private Animator anim;
    private Collider collider;
   
    [SerializeField] public RagdollState state;
    [SerializeField] private Rigidbody leftHandRigidBody;
    [SerializeField] private Rigidbody rightHandRigidBody;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField]private List<Rigidbody> ragdollRigids = new List<Rigidbody>();
    [SerializeField] private Transform leftFollow;
    [SerializeField] private Transform rightFollow;

    [SerializeField] private Transform leftHandPoint;
    [SerializeField] private Transform rightHandPoint;

    private bool isFixedHand;
    [SerializeField] private Vector3 leftHandFixedPosition;
    private Quaternion leftHandFixedRotation;
    [SerializeField] private Vector3 rightHandFixedPosition;
    private Quaternion rightHandFixedRotation;

    private Rigidbody rigidbody;

    private Vector3 prevLocalPosition;
    private Vector3 prevWorldPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            switch (state)
            {
                case RagdollState.Animated:
                    FixeHand(true);
                    ActiveRagdoll(true);
                    break;
                case RagdollState.Ragdoll:
                    FixeHand(false);
                    ActiveRagdoll(false);
                    break;
            }
        }

        if(isFixedHand)
        {
            //leftHandRigidBody.transform.SetPositionAndRotation(leftHandFixedPosition, leftHandFixedRotation);
            //rightHandRigidBody.transform.SetPositionAndRotation(rightHandFixedPosition, rightHandFixedRotation);
            //leftHandRigidBody.transform.position = leftHandFixedPosition;
            //rightHandRigidBody.transform.position = rightHandFixedPosition;
            leftHandTransform.SetPositionAndRotation(leftHandPoint.position, leftHandPoint.rotation);
            rightHandTransform.SetPositionAndRotation(rightHandPoint.position, rightHandPoint.rotation);
            //rigidbody.Sleep();
            //Debug.Log(rigidbody.centerOfMass);
            //transform.position = prevWorldPosition;
        }
    }

    private void LateUpdate()
    {
        if(isFixedHand)
        {
            transform.position = prevWorldPosition;
        }
    }

    private void ActiveRagdoll(bool active)
    {
        anim.enabled = !active;
        //collider.enabled = !active;
        collider.isTrigger = active;

        foreach(Rigidbody rigid in ragdollRigids)
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
        else
        {
            state = RagdollState.Animated;
            transform.localPosition = prevLocalPosition;
        }
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
            leftHandFixedPosition = leftHandTransform.position;
            leftHandFixedRotation = leftHandTransform.rotation;

            rightHandPoint.SetPositionAndRotation(rightHandTransform.position, rightHandTransform.rotation);
            rightHandFixedPosition = rightHandTransform.position;
            rightHandFixedRotation = rightHandTransform.rotation;

            leftHandPoint.SetParent(transform.parent);
            rightHandPoint.SetParent(transform.parent);
        }

    }
    
}
