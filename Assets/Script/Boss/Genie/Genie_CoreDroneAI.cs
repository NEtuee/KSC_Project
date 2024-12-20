using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_CoreDroneAI : Genie_BombDroneAI
{
    public bool mirror = true;
    public bool upside = true;
    public bool centerMove = true;

    public Transform coreTarget;

    private Vector3 _spawnPos;

    public override void Initialize()
    {
        base.Initialize();

        SetTargetOffset(Vector3.up * Random.Range(randomOffset.x,randomOffset.y));
        SetTargetDirectionUpdateTime(0.2f);
        maxSpeed = Random.Range(randomMaxSpeed.x,randomMaxSpeed.y);

        _spawnPos = transform.position;

        this.gameObject.SetActive(false);
    }

    public override void FixedProgress(float deltaTime)
    {
        if(mirror)
            SetMirrorSideTarget();
        
        if(coreTarget != null)
        {
            SetTarget(coreTarget);
        }
        
        if(GetTargetPosition().y > transform.position.y && upside)
        {
            var dist = MathEx.distance(GetTargetPosition().y, transform.position.y);
            AddForce(dist * 2f * Vector3.up * deltaTime);
        }
        else if(MathEx.distance(GetTargetPosition().y, transform.position.y) >= 1f && upside)
        {
            var dir = GetTargetPosition().y > transform.position.y ? 1f : -1f;
            var dist = MathEx.distance(GetTargetPosition().y, transform.position.y);
            AddForce(dist * dir * Vector3.up * deltaTime);
        }

        if(centerMove)
        {
            var centerDist = Vector3.Distance(centerPosition.position,transform.position);
            if(centerDist >= maxDistance)
            {
                var dir = (centerPosition.position - transform.position).normalized;
                AddForce(dir * maxSpeed * deltaTime * 3f);
            }
        }
        

        if(_target != null)
        {
            UpdateTargetDirection(deltaTime);

            if(_target.position.y <= 0f)
                _targetDirection.y = 0f;
        }
        else
        {
            _targetDirection = -(transform.position).normalized;
            _targetDirection.y = 0f;
        }

        if(directionRotation)
        {
            DirectionRotation();
        }

        UpdateVelocity(deltaTime);
    }

    public void SetMirrorSideTarget()
    {
        var player = gridControll.cubeGrid.GetCubeFromWorld(_player.transform.position);
        if(player == null)
        {
            _direction = (transform.position - _player.transform.position).normalized;
            return;
        }

        var mirror = gridControll.cubeGrid.GetCubeReflectMirror(player.cubePoint);
        if(mirror != null)
        {
            _target = mirror.transform;
        }
        else
        {
            _target = null;
            _direction = (_player.transform.position - transform.position).normalized;
        }
    }

    public void Respawn(bool launch)
    {
        Respawn(_spawnPos,launch);
    }
}
