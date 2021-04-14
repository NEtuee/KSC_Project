using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    private Transform root;
    [SerializeField] Transform head;
    [SerializeField] private LedgeCollider collider1;
    [SerializeField] private LedgeCollider collider2;

    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private Vector3 upCollisionOffset;
    [SerializeField] private Vector3 downCollisionOffset;
    [SerializeField] private Vector3 upCollisionSize;
    [SerializeField] private Vector3 downCollisionSize;

    [SerializeField] private bool upCollision = false;
    [SerializeField] private bool downCollision = false;

    private Collider[] collisionBuffer = new Collider[10];

    private bool preValue;
    private int checkCount;
    [SerializeField] private bool isDetectLedge;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private void Start()
    {
        root = transform.parent;
        head = root.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        //transform.parent = head;

        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        //transform.rotation = Quaternion.LookRotation(root.forward, root.up);
        //transform.localPosition = originalPos;
        //transform.localRotation = originalRot;

        int collisionCount = 0;

        collisionCount = Physics.OverlapBoxNonAlloc(transform.position + transform.TransformDirection(downCollisionOffset), downCollisionSize * 0.5f, collisionBuffer, transform.rotation, collisionLayer);
        downCollision = collisionCount == 0 ? true : false;

        collisionCount = Physics.OverlapBoxNonAlloc(transform.position + transform.TransformDirection(upCollisionOffset), upCollisionSize * 0.5f, collisionBuffer, transform.rotation, collisionLayer);
        upCollision = collisionCount == 0 ? true : false;

        isDetectLedge = false;

        if (downCollision == true)
        {
            if(upCollision == false)
            {
                isDetectLedge = true;
            }
        }


        //foreach (GameObject obj in collider1.collidedObjects)
        //{
        //    if(collider2.collidedObjects.Contains(obj) == false)
        //    {
        //        isDetectLedge = true;
        //        //ChangeValue(true);
        //        break;
        //    }
        //    else
        //    {
        //        isDetectLedge = false;
        //        //ChangeValue(false);
        //    }
        //}

        //if(collider1.collidedObjects.Count == 0)
        //{
        //    //ChangeValue(true);
        //    isDetectLedge = true;
        //}
    }

    public bool IsDetectedLedge()
    {
        return isDetectLedge;
    }

    private void ChangeValue(bool value)
    {
        if(isDetectLedge != value)
        {
            if(checkCount > 5)
            {
                checkCount = 0;
                isDetectLedge = value;
            }
            checkCount++;
        }
        else
        {
            checkCount = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.137f, 0.377f)), new Vector3(0.4f, 0.15f, 1f));
        //Gizmos.DrawWireCube(transform.position + transform.TransformDirection(new Vector3(0.0f, 1.3f, 0.377f)), new Vector3(0.4f, 0.15f, 1f));
        DebugCastDetection.Instance.DebugWireCube(transform.position + transform.TransformDirection(downCollisionOffset),transform, downCollisionSize,Color.red);
        DebugCastDetection.Instance.DebugWireCube(transform.position + transform.TransformDirection(upCollisionOffset),transform, upCollisionSize,Color.red);
    }
}

