using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_CoreDroneAI : Genie_BombDroneAI
{
    public new void Start()
    {
        Init();
        SetTarget(GameManager.Instance.player.transform);
        SetTargetOffset(Vector3.up * Random.Range(randomOffset.x,randomOffset.y));
        SetTargetDirectionUpdateTime(0.2f);
        maxSpeed = Random.Range(randomMaxSpeed.x,randomMaxSpeed.y);

        this.gameObject.SetActive(false);
    }

    public override void Progress(float deltaTime)
    {
        SetMirrorSideTarget();

        if(GetTargetPosition().y > transform.position.y)
        {
            var dist = MathEx.distance(GetTargetPosition().y, transform.position.y);
            AddForce(dist * 2f * Vector3.up * deltaTime);
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
        var player = gridControll.cubeGrid.GetCubeFromWorld(GameManager.Instance.player.transform.position);
        if(player == null)
            return;
        
        var mirror = gridControll.cubeGrid.GetCubeReflectMirror(player.cubePoint);
        if(mirror != null)
        {
            _target = mirror.transform;
        }
        else
        {
            _target = null;
            _direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
        }
    }
}
