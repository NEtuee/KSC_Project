using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandIK : MonoBehaviour
{
    [SerializeField] private bool _enableHandIk = false;
    [SerializeField] private bool _enableLeftHandIk = false;
    [SerializeField]private bool _enableRightHandIk = false;

    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    [SerializeField] private Vector3 llOffset;
    [SerializeField] private Vector3 lrOffset;
    [SerializeField] private Vector3 rlOffset;
    [SerializeField] private Vector3 rrOffset;

    private Vector3 _llpoint;
    private Vector3 _lrpoint;
    private Vector3 _rlpoint;
    private Vector3 _rrpoint;

    private Animator _anim;
    private PlayerUnit _playerUnit;

    private Vector3 _leftEffectPosition;
    private Vector3 _rightEffectPosition;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _playerUnit = GetComponent<PlayerUnit>();

        if (_anim == null)
            Debug.LogWarning("Not Set Animator");
    }

    private void Start()
    {
        leftHandTransform = _anim.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHandTransform = _anim.GetBoneTransform(HumanBodyBones.RightHand);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_enableHandIk == false)
            return;

        if(_enableLeftHandIk)
        {
            _anim.SetIKPosition(AvatarIKGoal.LeftHand, _leftEffectPosition);
            _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        }

        if(_enableRightHandIk)
        {
            _anim.SetIKPosition(AvatarIKGoal.RightHand, _rightEffectPosition);
            _anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        }
    }

    private void FixLeftHand()
    {
        if (_playerUnit.InputHorizontal == 0.0)
            return;

        //_enableHandIk = true;
        _leftEffectPosition = _anim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        _enableLeftHandIk= true;
    }

    private void FixRightHand()
    {
        if (_playerUnit.InputHorizontal == 0.0)
            return;

        //_enableHandIk = true;
        _rightEffectPosition = _anim.GetBoneTransform(HumanBodyBones.RightHand).position;
        _enableRightHandIk = true;
    }

    private void ReleaseLeftHand()
    {
        if (_playerUnit.InputHorizontal == 0.0)
            return;

        _enableLeftHandIk = false;
    }

    private void ReleaseRightHand()
    {
        if (_playerUnit.InputHorizontal == 0.0)
            return;

        _enableRightHandIk = false;
    }

    public void DisableHandIK()
    {
        _enableHandIk = false;
        _enableLeftHandIk = false;
        _enableRightHandIk = false;
    }

    private void TracePoint()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.TransformDirection(llOffset),transform.forward, out hit, 1.5f))
        {
            _llpoint = hit.point;
        }

        if (Physics.Raycast(transform.position + transform.TransformDirection(lrOffset), transform.forward, out hit, 1.5f))
        {
            _lrpoint = hit.point;
        }

        if (Physics.Raycast(transform.position + transform.TransformDirection(rlOffset), transform.forward, out hit, 1.5f))
        {
            _rlpoint = hit.point;
        }

        if (Physics.Raycast(transform.position + transform.TransformDirection(rrOffset), transform.forward, out hit, 1.5f))
        {
            _rrpoint = hit.point;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_llpoint, 0.1f);
        Gizmos.DrawWireSphere(_lrpoint, 0.1f);
        Gizmos.DrawWireSphere(_rlpoint, 0.1f);
        Gizmos.DrawWireSphere(_rrpoint, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(llOffset), 0.1f);
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(lrOffset), 0.1f);
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(rlOffset), 0.1f);
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(rrOffset), 0.1f);
    }
#endif

}
