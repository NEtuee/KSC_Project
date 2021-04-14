using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boogie_AI : IKBossBase
{
    public enum State
    {
        CannonSearch,
        CannonLock,
        CannonShot,
        HitStep,
        HeadHit,
        
    };
    
    public GraphAnimator animator;
    public Transform rootTransform;
    public Transform cannonRoot;
    public Transform headIKHolder;

    public Collider headCollider;
    
    public CannonRotator cannon;
    public HeadRotator head;

    public List<Transform> headShields = new List<Transform>();

    public LayerMask cannonShotLayer;

    public float explosionDist = 3f;
    public float cannonRotateLerpFactor = 1f;
    public float cannonSearchTime = 8f;
    public float cannonLockTime = 3f;
    public float cannonShotSpeed = 100f;
    public float hitStepTime = 3f;
    public float hitStepSpeed = 5f;
    public float shieldOpenTime = 10f;
    
    public State currentState;

    private EffectManager _effectManager;
    private PlayerRagdoll _playerRagdoll;

    private RayEx _cannonRay;

    private Vector3 _moveDirection;

    private float _moveSpeed;

    private bool _headOpen = false;

    void Start()
    {
        Initialize();
        cannon.target = _target;
        head.target = _target;

        cannon.lerpFactor = cannonRotateLerpFactor;
        
        animator.Play("UpDown",rootTransform);
        animator.Play("UpDownRotator",rootTransform);

        _timeCounter.InitTimer("cannonShot", 0f,1f);
        _timeCounter.InitTimer("cannonSearch",0f,cannonSearchTime);
        _timeCounter.InitTimer("cannonLock",0f,cannonLockTime);
        _timeCounter.InitTimer("hitStep", 0f,hitStepTime);


        _effectManager = GameManager.Instance.effectManager;
        _playerRagdoll = GameManager.Instance.player.GetComponent<PlayerRagdoll>();

        _cannonRay = new RayEx(new Ray(Vector3.zero, Vector3.zero),100f,cannonShotLayer);
    }

    public void Update()
    {
        Progress(Time.deltaTime);
    }

    void Progress(float deltaTime)
    {
        if (currentState == State.CannonSearch)
        {
            if (cannon.targetInArea)
            {
                _timeCounter.IncreaseTimer("cannonSearch", out bool limit);
                if (limit)
                {
                    _timeCounter.InitTimer("cannonSearch",0f,cannonSearchTime);
                    ChangeState(State.CannonLock);
                }
            }
        }
        else if (currentState == State.CannonLock)
        {
            _timeCounter.IncreaseTimer("cannonLock", out bool limit);
            if (limit)
            {
                ChangeState(State.CannonShot);
            }
        }
        else if (currentState == State.CannonShot)
        {
            _timeCounter.IncreaseTimer("cannonShot", out bool limit);
            if (limit)
            {
                var dir = cannon.GetLookDirection();

                _cannonRay.SetDirection(dir);
                if (_cannonRay.Cast(cannonRoot.position, out var hit))
                {
                    var dist = Vector3.Distance(_target.position, hit.point);
                    if (dist <= explosionDist)
                    {
                        _playerRagdoll.ExplosionRagdoll(200f,hit.point,0f);
                    }
                    
                    _effectManager.Active("CannonExplosion",hit.point,Quaternion.identity);
                }
                    
                ChangeState(State.CannonSearch);
            }
            
        }
        else if (currentState == State.HitStep)
        {
            var time = _timeCounter.IncreaseTimer("hitStep", out bool limit);
            _moveSpeed = Mathf.Lerp(hitStepSpeed, 0f, time / hitStepTime);
            rotationSpeed = Mathf.Lerp(30f, 0f, time / hitStepTime);

            Move(_moveDirection, _moveSpeed, deltaTime);
            var angle = Vector3.SignedAngle(transform.forward,-_moveDirection,transform.up);

            if(MathEx.abs(angle) > 3f)
            {
                if(angle > 0)
                    Turn(true,this.transform,deltaTime);
                else
                    Turn(false,this.transform,deltaTime);
            }

            if (limit)
            {
                ChangeState(State.CannonSearch);
            }
        }


        if (_headOpen)
        {
            _timeCounter.IncreaseTimer("HitGroggy", out bool limit);
            if (limit)
            {
                _headOpen = false;
                headCollider.enabled = true;
            }
        }

    }
    
    public void ChangeState(State state)
    {
        if (state == State.CannonSearch)
        {
            cannon.rotateLock = false;
        }
        else if (state == State.CannonLock)
        {
            _timeCounter.InitTimer("cannonLock",0f,cannonLockTime);
            cannon.rotateLock = true;
        }
        else if (state == State.CannonShot)
        {
            var dist = Vector3.Distance(_target.position, transform.position);
            
            _timeCounter.InitTimer("cannonShot", 0f,dist / cannonShotSpeed);
            animator.Play("CannonShot_cannon",cannonRoot);
            
            var dir = cannon.GetLookDirection();
            _effectManager.Active("CannonShotExplosion",cannonRoot.position + dir * 3f,Quaternion.identity);
        }
        else if (state == State.HitStep)
        {
            _timeCounter.InitTimer("hitStep", 0f,hitStepTime);
            _moveDirection = Vector3.ProjectOnPlane((transform.position - _target.position), transform.up).normalized;
            _moveSpeed = hitStepSpeed;
            
            animator.Play("Hit",rootTransform);
        }
        else if (state == State.HeadHit)
        {
            animator.Play("HeadHit",headIKHolder);
            animator.Play("Hit",rootTransform);
            animator.Play("ShieldOpen",headShields);
            
            _headOpen = true;
            headCollider.enabled = false;
            _timeCounter.InitTimer("HitGroggy", 0f,shieldOpenTime);
            
            ChangeState(State.CannonSearch);
            return;
        }
        
        currentState = state;
    }

    public void HeadBombHit()
    {
        ChangeState(State.HeadHit);
        
    }

    public void HeadHit()
    {
        Debug.Log("HeadHit");
    }

    public void BellyHit()
    {
        ChangeState(State.HitStep);
    }
}
