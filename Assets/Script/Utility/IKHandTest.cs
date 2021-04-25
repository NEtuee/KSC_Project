using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandTest : MonoBehaviour
{
    public Transform target;
    public Animator animator;

    public bool lookOriginForward;

    private Vector3 originForward;

    void Start()
    {
        originForward = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // if (lookOriginForward == true)
        // {
        //     transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(originForward,transform.up),2f*Time.deltaTime );
        // }
    }

    private void FixedUpdate()
    {
        if (lookOriginForward == true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(originForward,transform.up),2f*Time.fixedDeltaTime );
        }    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.LeftHand, target.position);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
    }
}
