using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTargetCharacter : MonoBehaviour
{
    [SerializeField] private bool activeCopy = false;
    private Transform _leftHandIKTarget;
    private Animator _anim;
    private Vector3 _targetForwardDirection;
    private CharacterJoint _chJoint;
    private Rigidbody _rigidbody;
    
    void Start()
    {
        _anim = GetComponent<Animator>();
        _chJoint = GetComponent<CharacterJoint>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
    }
    
    public void SetConnectBody(Rigidbody connectBody)
    {
        _leftHandIKTarget = connectBody.transform;
        //Vector3 relative = transform.InverseTransformDirection(_leftHandIKTarget.position);
        _chJoint.connectedBody = connectBody;
        //_chJoint.anchor = relative;
        _targetForwardDirection = transform.forward;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        
        _anim.SetTrigger("Hang");
        activeCopy = true;
    }

    private void FixedUpdate()
    {
        if (activeCopy == true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(_targetForwardDirection,transform.up),2f*Time.fixedDeltaTime );
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_anim == null || activeCopy == false)
            return;
        
        _anim.SetIKPosition(AvatarIKGoal.LeftHand,_leftHandIKTarget.position);
        _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,1f);
    }
}
