using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBossAI : MonoBehaviour
{
    private float _movementSpeed;
    private float _turnAngle;

    private Vector3 _targetDirection;


    public void Phase_One()
    {

    }


    public void SetTarget(Vector3 target)
    {
        var direction = target - transform.position.normalized;
        _targetDirection = Vector3.ProjectOnPlane(direction,transform.up).normalized;
    }

    public void SetFastMovement()
    {
        SetMovementSpeed(1f);
        SetTurnAngle(1f);
    }

    public void SetSlowMovement()
    {
        SetMovementSpeed(1f);
        SetTurnAngle(1f);
    }

    public void SetMovementSpeed(float speed) {_movementSpeed = speed;}
    public void SetTurnAngle(float speed) {_turnAngle = speed;}
}
