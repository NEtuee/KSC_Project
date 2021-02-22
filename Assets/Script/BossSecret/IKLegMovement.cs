using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegMovement : MonoBehaviour
{
    public IKLegMovement oppositeLeg;

    public LayerMask groundLayer;
    public AnimationCurve planeMovementCurve;
    public AnimationCurve heightMovementCurve;

    public float legSpeed = 1f;
    public float heightFactor = 10f;
    public float rayDistance = 10f;
    public float limitDistance = 3f;

    public bool ikDetach = false;

    public bool isMove{get{return _isMove;}}

    public Transform ik;
    public Transform rayPoint;

    private RayEx _downRay;

    private Vector3 _stratPosition;
    private Vector3 _targetPosition;

    private bool _isMove = false;
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
        _downRay.SetDirection(-rayPoint.up);
        if(_downRay.Cast(rayPoint.position,out RaycastHit hit))
        {
            float dist = Vector3.Distance(ik.position,hit.point);
            _targetPosition = hit.point;

            if(dist >= limitDistance && !_isMove && !oppositeLeg.isMove)
            {
                _isMove = true;
                _stratPosition = ik.position;
            }
        }

        if(_isMove)
        {
            _timer += legSpeed * Time.deltaTime;
            _timer = _timer >= 1f ? 1f : _timer;

            var pos = Vector3.Lerp(_stratPosition,_targetPosition,planeMovementCurve.Evaluate(_timer));
            pos.y += heightMovementCurve.Evaluate(_timer);

            ik.position = pos;

            if(_timer >= 1f)
            {
                _timer = 0;
                _isMove = false;
            }
        }
        
    }

}
