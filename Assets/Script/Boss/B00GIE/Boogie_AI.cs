using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boogie_AI : IKBossBase
{
    public enum State
    {
        CannonShot
    };
    
    public GraphAnimator animator;
    public Transform rootTransform;
    public Transform cannonRoot;

    public CannonRotator cannon;
    public HeadRotator head;

    public LayerMask cannonShotLayer;

    public float explosionDist = 3f;
    
    public State currentState;

    private EffectManager _effectManager;
    private PlayerRagdoll _playerRagdoll;

    private RayEx _cannonRay;

    private bool _shotCheck = false;
    
    void Start()
    {
        Initialize();
        cannon.target = _target;
        head.target = _target;
        
        animator.Play("UpDown",rootTransform);
        animator.Play("UpDownRotator",rootTransform);

        _timeCounter.InitTimer("cannonShot", 0f,8f);
        _timeCounter.InitTimer("cannonExplodeTime");
        _effectManager = GameManager.Instance.effectManager;
        _playerRagdoll = GameManager.Instance.player.GetComponent<PlayerRagdoll>();

        _cannonRay = new RayEx(new Ray(Vector3.zero, Vector3.zero),100f,cannonShotLayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.CannonShot)
        {
            if (cannon.targetInArea)
            {
                _timeCounter.IncreaseTimer("cannonShot", out bool limit);
                if (limit)
                {
                    _timeCounter.InitTimer("cannonShot", 0f,8f);
                    _timeCounter.InitTimer("cannonExplodeTime");

                    _shotCheck = true;
                    animator.Play("CannonShot_cannon",cannonRoot);
                }
            }

        }
        
        if (_shotCheck)
        {
            _timeCounter.IncreaseTimer("cannonExplodeTime", out bool limit);
            if (limit)
            {
                var dir = (_target.position - cannonRoot.position).normalized;
                _cannonRay.SetDirection(dir);

                if (_cannonRay.Cast(cannonRoot.position, out var hit))
                {
                    var dist = Vector3.Distance(_target.position, _target.position);
                    if (dist <= explosionDist)
                    {
                        _playerRagdoll.ExplosionRagdoll(200f,hit.point,0f);
                    }
                }

                _shotCheck = false;
            }
        }
        
    }

    public void ChangeState(State state)
    {
        if (state == State.CannonShot)
        {
            _timeCounter.InitTimer("cannonShot", 5f);
        }
        
        currentState = state;
    }
}
