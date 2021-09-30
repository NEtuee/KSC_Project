using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandIK : MonoBehaviour
{
    [SerializeField] private bool _enableHandIk = false;
    [SerializeField] private bool _enableLeftHandIk = false;
    [SerializeField]private bool _enableRightHandIk = false;

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

        _leftEffectPosition = _anim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        _enableLeftHandIk= true;
        //_enableHandIk = true;
    }

    private void FixRightHand()
    {
        if (_playerUnit.InputHorizontal == 0.0)
            return;

        _rightEffectPosition = _anim.GetBoneTransform(HumanBodyBones.RightHand).position;
        _enableRightHandIk = true;
        //_enableHandIk = true;
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
}
