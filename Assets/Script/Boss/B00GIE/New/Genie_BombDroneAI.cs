using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_BombDroneAI : DroneAIBase
{
    public Boogie_GridControll gridControll;

    public Vector2 randomMaxSpeed;
    public Vector2 randomOffset;
    public EMPShield shield;

    public float targetUpdateTime = 0.2f;
    public float lifeTime = 60f;
    public float launchTime = 2f;

    public float explosionDistance;

    protected float _lifeTime;

    public void Start()
    {
        Init();
        SetTarget(GameManager.Instance.player.transform);
        SetTargetOffset(Vector3.up * Random.Range(randomOffset.x,randomOffset.y));
        SetTargetDirectionUpdateTime(targetUpdateTime);
        SetRandomRotate(new Vector3(0f,180f,0f));
        maxSpeed = Random.Range(randomMaxSpeed.x,randomMaxSpeed.y);

        _timeCounterEx.InitTimer("launch",0f,launchTime);

        this.gameObject.SetActive(false);
    }

    public override void Progress(float deltaTime)
    {
        ExplosionCheck();

        _lifeTime -= deltaTime;
        if(_lifeTime <= 0f)
        {
            shield.Hit();
        }

        _timeCounterEx.IncreaseTimerSelf("launch",out var limit, deltaTime);
        if(!limit)
        {
            UpdateVelocity(deltaTime);
            return;
        }

        if(GetTargetPosition().y > transform.position.y)
        {
            var dist = MathEx.distance(GetTargetPosition().y, transform.position.y);
            AddForce(dist * 2f * Vector3.up * deltaTime);
        }
        
        base.Progress(deltaTime);
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
                currCube.SetActive(false,false);
            }
        }
        
        gridControll.GetCube_Near(point,3,true);
        foreach(var cube in gridControll.GetTargetCubes())
        {
            var dist = MathEx.distance(transform.position.y,cube.transform.position.y);
            if(dist <= explosionDistance)
            {
                cube.SetActive(false,false);
            }
            
        }
    }

    public bool IsDead()
    {
        return shield.isOver;
    }

    public virtual void Respawn(Vector3 spawnPosition)
    {
        _lifeTime = lifeTime;

        InitVelocity();
        UpdateTargetDirection();
        shield.Reactive();

        transform.position = spawnPosition;
        _timeCounterEx.InitTimer("launch",0f,launchTime);
        var rand = Quaternion.Euler(0f,Random.Range(0f,360f),Random.Range(0f,360f)) * Vector3.forward;
        _direction = rand.normalized;
        AddForce(rand.normalized * maxSpeed * 100f);
        shield.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
    }
}
