using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    private Transform root;
    [SerializeField] Transform head;
    [SerializeField] private LedgeCollider collider1;
    [SerializeField] private LedgeCollider collider2;
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

        foreach (GameObject obj in collider1.collidedObjects)
        {
            if(collider2.collidedObjects.Contains(obj) == false)
            {
                isDetectLedge = true;
                //ChangeValue(true);
                break;
            }
            else
            {
                isDetectLedge = false;
                //ChangeValue(false);
            }
        }

        if(collider1.collidedObjects.Count == 0)
        {
            //ChangeValue(true);
            isDetectLedge = true;
        }
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
}

