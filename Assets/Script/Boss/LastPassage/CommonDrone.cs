using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDrone : DroneAIBase
{
    public Rigidbody rig;
    public Vector2 randomMaxSpeed;
    public Vector2 randomOffset;
    public EMPShield shield;

    public float damage = 10f;
    public float targetUpdateTime = 0.2f;
    public float lifeTime = 60f;
    public float launchTime = 2f;

    public float explosionDistance;

    protected float _lifeTime;

    private Transform _mainTarget;

    private Vector3 _spawnPos;
    private Quaternion _spawnRot;

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
        UpdateTargetDirection();

        this.gameObject.SetActive(false);

        _spawnPos = transform.position;
        _spawnRot = transform.rotation;
    }

    public override void FixedProgress(float deltaTime)
    {
        _lifeTime -= deltaTime;
        if(_lifeTime <= 0f)
        {
            shield.Hit();
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

        ExplosionCheck();
        
        base.FixedProgress(deltaTime);
    }

    public void ExplosionCheck()
    {
        if(_targetDistance <= explosionDistance)
        {
            shield.Hit();
        }
    }

    public void HitDamage()
    {
        var data = MessageDataPooling.GetMessageData<MD.FloatData>();
        data.value = damage;
	    SendMessageQuick(MessageTitles.playermanager_addDamageToPlayer,
                    UniqueNumberBase.GetSavedNumberStatic("PlayerManager"),data);
    }

    public bool IsDead()
    {
        return shield.isOver;
    }

    public void Respawn()
    {
        Respawn(_spawnPos);
        transform.rotation = _spawnRot;
        rig.rotation = _spawnRot;
        _direction = transform.forward;
        //AddForce(transform.forward * maxSpeed * 100f);
    }

    public virtual void Respawn(Vector3 spawnPosition)
    {
        _lifeTime = lifeTime;

        //_target = spawnTarget;

        InitVelocity();
        UpdateTargetDirection();
        shield.Reactive();

        transform.position = spawnPosition;
        _timeCounterEx.InitTimer("launch",0f,launchTime);
        var rand = Quaternion.Euler(0f,Random.Range(0f,360f),Random.Range(0f,360f)) * Vector3.forward;
        _direction = rand.normalized;
        //AddForce(rand.normalized * maxSpeed * 100f);
        shield.gameObject.SetActive(true);
        this.gameObject.SetActive(true);

        rig.position = spawnPosition;
        rig.velocity = Vector3.zero;
    }
}
