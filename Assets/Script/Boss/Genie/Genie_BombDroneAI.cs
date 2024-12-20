using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_BombDroneAI : DroneAIBase
{
    public Boogie_GridControll gridControll;
    public Transform centerPosition;

    public Vector2 randomMaxSpeed;
    public Vector2 randomOffset;
    public EMPShield shield;

    public bool destroyGround = true;
    public bool DeactiveStart = true;
    public bool targetExplosion = true;

    public float targetUpdateTime = 0.2f;
    public float lifeTime = 60f;
    public float launchTime = 2f;
    public float maxDistance;
    public float maxRecogDistance;

    public float explosionDistance;

    protected float _lifeTime;

    public Transform _mainTarget;

    public override void Initialize()
    {
        base.Initialize();

        _mainTarget = _target;
        SetTargetOffset(Vector3.up * Random.Range(randomOffset.x,randomOffset.y));
        SetTargetDirectionUpdateTime(targetUpdateTime);
        SetRandomRotate(new Vector3(0f,180f,0f));
        maxSpeed = Random.Range(randomMaxSpeed.x,randomMaxSpeed.y);

        _lifeTime = lifeTime;

        _timeCounterEx.InitTimer("launch",0f,launchTime);

        if(DeactiveStart)
            this.gameObject.SetActive(false);
        else
        {
            Respawn(transform.position);
        }
    }

    public override void FixedProgress(float deltaTime)
    {
        _lifeTime -= deltaTime;
        if(_lifeTime <= 0f)
        {
            shield.Hit();
        }

        var centerTargetDist = Vector3.Distance(centerPosition.position,_mainTarget.position);
        if(centerTargetDist > maxRecogDistance)
        {
            if(_target == _mainTarget)
            {
                gridControll.GetCube_Range(5,centerPosition.position,false);
                _target = gridControll.GetRandomTargetCube().transform;
            }
        }
        else
        {
            if(_target != _mainTarget)
            {
                _target = _mainTarget;
            }

            if(targetExplosion)
                ExplosionCheck();
        }

        var centerDist = Vector3.Distance(centerPosition.position,transform.position);
        if(centerDist >= maxDistance)
        {
            var dir = (centerPosition.position - transform.position).normalized;
            AddForce(dir * maxSpeed * deltaTime * 3f);
        }

        _timeCounterEx.IncreaseTimerSelf("launch",out var limit, deltaTime);
        if(!limit)
        {
            UpdateVelocity(deltaTime);
            if(directionRotation)
                DirectionRotation();
            return;
        }

        if(GetTargetPosition().y > transform.position.y)
        {
            var dist = MathEx.distance(GetTargetPosition().y, transform.position.y);
            AddForce(dist * 2f * Vector3.up * deltaTime);
        }
        
        base.FixedProgress(deltaTime);
    }

    public void ExplosionCheck()
    {
        if(_targetDistance <= explosionDistance)
        {
            shield.Hit();
        }
    }

    public void DestroyGround()
    {
        var point = gridControll.cubeGrid.GetCubePointFromWorld(transform.position);
        var currCube = gridControll.cubeGrid.GetCube(point,true);
        if(currCube != null)
        {
            var dist = MathEx.distance(transform.position.y,currCube.transform.position.y);
            if(dist <= explosionDistance)
            {
                currCube.SetMove(false,0f,1.3f,2f);
            }
        }
        
        gridControll.GetCube_Near(point,3,true);
        foreach(var cube in gridControll.GetTargetCubes())
        {
            if(cube == null)
            {
                Debug.Log("WTF");
            }
            var dist = MathEx.distance(transform.position.y,cube.transform.position.y);
            if(dist <= explosionDistance)
            {
                cube.SetMove(false,0f,1.3f,2f);
            }
            
        }
    }

    public bool IsDead()
    {
        return shield.isOver;
    }

    public void ToMainTarget()
    {
        _target = _player.transform;
        _mainTarget = _player.transform;
    }

    public virtual void Respawn(Vector3 spawnPosition, bool launch = true)
    {
        _lifeTime = lifeTime;

        InitVelocity();
        UpdateTargetDirection();
        shield.Reactive();

        transform.position = spawnPosition;
        _timeCounterEx.InitTimer("launch",0f,launchTime);

        if(launch)
        {
            var rand = Quaternion.Euler(0f,Random.Range(0f,360f),Random.Range(0f,360f)) * Vector3.forward;
            _direction = rand.normalized;
            AddForce(rand.normalized * maxSpeed * 100f);
        }
        
        shield.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
    }
}
