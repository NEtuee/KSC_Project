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
    private bool _activeIK = false;

    public delegate void WhenHangShake();
    public WhenHangShake whenEndHangShake;
    
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
        _activeIK = true;

        _rigidbody.AddForce(-_targetForwardDirection*2.0f, ForceMode.Impulse);

        StartCoroutine(Active());
    }

    private void FixedUpdate()
    {
        if (activeCopy == false)
            return;

        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(_targetForwardDirection,transform.up),2f*Time.fixedDeltaTime );

        //if (Vector3.Dot(Vector3.Cross(transform.up, Vector3.right), Vector3.forward) < 0)
        //{

        //}
    }

    IEnumerator Active()
    {
        float time = 0.0f;

        while (time < 1.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_targetForwardDirection, transform.up), 6f * Time.fixedDeltaTime);

            if(Vector3.Dot(Vector3.Cross(transform.up, Vector3.right), Vector3.forward) < 0)
            {
                time += Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }

        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        while (Quaternion.LookRotation(_targetForwardDirection, Vector3.up) != transform.rotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_targetForwardDirection, Vector3.up), 20f * Time.deltaTime);
        }

        _anim.SetTrigger("Back");
        //_activeIK = false;
    }

    IEnumerator AdjustWallDist()
    {
        RaycastHit hit;
        float time = 0.0f;

        while (time < 1.0f)
        {
            Vector3 startPos = transform.position + transform.up * (0.27f * 0.5f) + (-transform.forward * 1f);

            if (Physics.SphereCast(startPos, 0.27f, transform.forward, out hit, 3.0f))
            {
                float distToWall = (hit.point - (transform.position + transform.up * (1.7f * 0.5f))).magnitude;
                if (distToWall > 0.6f || distToWall < 0.35f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, (hit.point - transform.up * (1.7f * 0.5f)) + hit.normal * 0.35f, 15f * Time.deltaTime);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void Init()
    {
        StopAllCoroutines();
        _anim.SetTrigger("Init");
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_anim == null || activeCopy == false || _activeIK == false)
            return;
        
        _anim.SetIKPosition(AvatarIKGoal.LeftHand,_leftHandIKTarget.position);
        _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,1f);
    }

    private void EndHangShake()
    {
        whenEndHangShake?.Invoke();
    }
}
