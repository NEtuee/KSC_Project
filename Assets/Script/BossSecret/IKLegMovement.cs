using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegMovement : MonoBehaviour
{
    public IKLegMovement oppositeLeg;

    public ParticlePool particlePool;

    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public AnimationCurve planeMovementCurve;
    public AnimationCurve heightMovementCurve;

    public float legSpeed = 1f;
    public float heightFactor = 10f;
    public float rayDistance = 10f;
    public float limitDistance = 3f;

    public bool ikDetach = false;

    public bool isMove{get{return _isMove;}}

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
    private float _timer = 0f;

    private void Start()
    {
        _downRay = new RayEx(new Ray(Vector3.zero,Vector3.down),rayDistance,groundLayer);
        if(ikDetach)
        {
            ik.parent = null;
        }
    }

    private void LateUpdate()
    {
        if(_hold)
        {
            ik.transform.position = Vector3.Lerp(ik.transform.position,ikHolder.position,.2f);
            return;
        }

        _downRay.SetDirection(-rayPoint.up);

        if(_downRay.Cast(rayPoint.position,out RaycastHit hit))
        {
            float dist = Vector3.Distance(ik.position,hit.point);
            _targetPosition = hit.point;
            
            _targetRotation = Quaternion.LookRotation(hit.normal);

            if(dist >= limitDistance && !_isMove && !oppositeLeg.isMove)
            {
                _isMove = true;
                _stratPosition = ik.position;
                _startRotation = ik.rotation;
            }
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

                particlePool.Active(pos,Quaternion.LookRotation(new Vector3(0f,0f,1f),hit.normal));


            }
        }
        
    }

    public void Hold(bool value)
    {
        _hold = value;
    }

}
