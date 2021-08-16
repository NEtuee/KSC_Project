using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_CoreDroneAI : Genie_BombDroneAI
{
    public override void Initialize()
    {
        base.Initialize();

        SetTargetOffset(Vector3.up * Random.Range(randomOffset.x,randomOffset.y));
        SetTargetDirectionUpdateTime(0.2f);
        maxSpeed = Random.Range(randomMaxSpeed.x,randomMaxSpeed.y);

        this.gameObject.SetActive(false);
    }

    public override void FixedProgress(float deltaTime)
    {
        SetMirrorSideTarget();

        if(GetTargetPosition().y > transform.position.y)
        {
            var dist = MathEx.distance(GetTargetPosition().y, transform.position.y);
            AddForce(dist * 2f * Vector3.up * deltaTime);
        }

        var centerDist = Vector3.Distance(centerPosition.position,transform.position);
        if(centerDist >= maxDistance)
        {
            var dir = (centerPosition.position - transform.position).normalized;
            AddForce(dir * maxSpeed * deltaTime * 3f);
        }

        if(_target != null)
            UpdateTargetDirection(deltaTime);

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
}
