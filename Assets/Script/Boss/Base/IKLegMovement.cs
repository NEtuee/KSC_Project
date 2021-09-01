using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegMovement : MonoBehaviour
{
    public delegate void LegHitToGround(Vector3 v);
    
    public IKLegMovement oppositeLeg;
    public DitzelGames.FastIK.FastIKFabric iKFabric;

    public ParticlePool particlePool;
    public LegHitToGround legHitToGround = (v) => { };

    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public AnimationCurve planeMovementCurve;
    public AnimationCurve heightMovementCurve;

    public float legSpeed = 1f;
    public float heightFactor = 10f;
    public float rayDistance = 10f;
    public float limitDistance = 3f;

    public float ikHeight = 0f;

    public bool firstMove = false;
    public bool ikDetach = false;

    public bool isMove{get{return _isMove;}}
    public bool moving = false;

    public Transform ik;
    public Transform ikHolder;
    public Transform rayPoint;

    private RayEx _downRay;

    private Vector3 _stratPosition;
    private Vector3 _targetPosition;

    private Quaternion _startRotation;
    private Quaternion _targetRotation;

    private bool _isMove = false;
    private bool _hold = false;
    private bool _holdGroundCheck = false;
    private float _timer = 0f;

    private void Awake()
    {
        _downRay = new RayEx(new Ray(Vector3.zero,Vector3.down),rayDistance,groundLayer);
        if(ikDetach)
        {
            ik.rotation = Quaternion.identity;
            ik.parent = null;
        }
    }

    private void LateUpdate()
    {
        _downRay.SetDirection(-rayPoint.up);

        if(_hold)
        {
            if(ikHolder == null)
                return;
            if(_holdGroundCheck && _downRay.Cast(rayPoint.position,out RaycastHit check))
            {
                var rayDist = Vector3.Distance(ik.transform.position,check.point);
                var dist = Vector3.Distance(ik.transform.position,ikHolder.position);

                if(rayDist < dist)
                {
                    ik.transform.position = check.point;
                    ik.transform.SetParent(check.transform);
                    return;
                }

            }
            
            ik.transform.position = Vector3.Lerp(ik.transform.position,ikHolder.position,10f * Time.deltaTime);
            return;
        }

        if(_downRay.Cast(rayPoint.position,out RaycastHit hit))
        {
            float dist = Vector3.Distance(ik.position,hit.point);
            _targetPosition = hit.point + (hit.normal * ikHeight);
            _targetRotation = Quaternion.LookRotation(hit.normal);

            if(dist >= limitDistance && !_isMove && !oppositeLeg.isMove)
            {
                bool moveCheck = (firstMove || (oppositeLeg.firstMove == firstMove)) || oppositeLeg.moving;
                if (moveCheck)
                {
                    _isMove = true;
                    moving = true;
                    _stratPosition = ik.position;
                    _startRotation = ik.rotation;
                }
                
            }
        }

        if(!_isMove && !oppositeLeg.isMove)
        {
            moving = false;
        }

        if(_isMove)
        {
            _timer += legSpeed * Time.deltaTime;
            _timer = _timer >= 1f ? 1f : _timer;

            var pos = Vector3.Lerp(_stratPosition,_targetPosition,planeMovementCurve.Evaluate(_timer));
            //pos.y += heightMovementCurve.Evaluate(_timer);
            pos += ik.forward * heightMovementCurve.Evaluate(_timer);

            ik.position = pos;
            ik.rotation = Quaternion.Lerp(_startRotation,_targetRotation,planeMovementCurve.Evaluate(_timer));

            if(_timer >= 1f)
            {
                _timer = 0;
                _isMove = false;

                if(ReferenceEquals(particlePool, null) == false)
                    particlePool.Active(pos,Quaternion.LookRotation(new Vector3(0f,0f,1f),hit.normal));

                legHitToGround(pos);
            }
        }
        
    }

    public void SetStartPosition(Vector3 pos)
    {
        _stratPosition = pos;
    }

    public void SetIKActive(bool value)
    {
        iKFabric.enabled = value;
    }

    public void Hold(bool value, bool groundCheck = true)
    {
        _hold = value;
        _holdGroundCheck = groundCheck;
    }

}
