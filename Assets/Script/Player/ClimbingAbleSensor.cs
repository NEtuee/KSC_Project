using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingAbleSensor : MonoBehaviour
{
    [SerializeField] private bool isDetected;
    [SerializeField] private bool isCanMove;
    [SerializeField] private bool isFixed;
    private CapsuleCollider collider;
    private int climbingAbleLayer;

    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        climbingAbleLayer = LayerMask.NameToLayer("ClimbingAble");
    }

    private void Update()
    {
        if(isFixed == true)
        {
            transform.rotation = Quaternion.identity;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enviroment") || other.CompareTag("Env_Props"))
        isDetected = true;

        //Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
        if(other.gameObject.layer == climbingAbleLayer)
        {
            isCanMove = true;
        }
        else
        {
            isCanMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enviroment")|| other.CompareTag("Env_Props"))
            isDetected = false;
    }

    public bool GetIsDetected() { return isDetected; }

    public float GetCapsuleHeight() { return collider.height; }

    public bool GetIsCanMove() { return isCanMove; }

}
