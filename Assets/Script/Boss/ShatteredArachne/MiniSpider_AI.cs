using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSpider_AI : IKPathFollowBossBase
{
    public enum State
    {
        Idle,
        Go,
        Out,
        BombThorw,
        Dead
    };

    public State currentState;

    public BombCollector bombCollector;
    public IKFootPointRotator footPointRotator;
    public Transform bombPoint;

    public GameObject bombOrigin;
    public GraphAnimator animator;
    public Rigidbody body;

    public bool bombRefillContinuous = false;

    public float bombThrowPower = 10f;
    public float bombRefillTime = 10f;

    public string goPath;
    public string outPath;

    private EMPShield _bomb;
    private Rigidbody _bombRigidbody;
    private Vector3 _posOrigin;

    public void Start()
    {
        ChangeState(State.Idle);
        _pathLoop = false;
        _timeCounter.InitTimer("bombThrowTimer",0f,1f);
        _timeCounter.InitTimer("bombRefillTime",bombRefillTime,bombRefillTime);

        _posOrigin = transform.position;
    }

    public void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;


        if (currentState == State.Idle)
        {
            _timeCounter.IncreaseTimer("bombRefillTime",out bool limit);
            
            if(bombRefillContinuous)
            {
                if(limit)
                {
                    InitBomb();
                    ChangeState(State.Go);
                }
            }
            else
            {
                if(_bomb == null || _bomb.isOver)
                {
                    if(limit)
                    {
                        InitBomb();
                        ChangeState(State.Go);
                    }
                }
            }

        }
        else if(currentState == State.Go)
        {
            FollowPath();

            // if(_bomb == null || _bomb.isOver)
            // {
            //     ChangeState(State.Dead);
            // }

            if(_pathArrived)
            {
                ChangeState(State.BombThorw);
            }
        }
        else if(currentState == State.Out)
        {
            FollowPath();

            if(_pathArrived)
            {
                ChangeState(State.Idle);
            }
        }
        else if(currentState == State.BombThorw)
        {
            // if(_bomb == null || _bomb.isOver)
            // {
            //     ChangeState(State.Dead);
            // }

            _timeCounter.IncreaseTimer("bombThrowTimer",out bool limit);
            if(limit)
            {
                ChangeState(State.Out);
            }

            _timeCounter.IncreaseTimer("ThrowTimer",out limit);
            if(limit)
            {
                _timeCounter.InitTimer("ThrowTimer",-10f);
                BombThrow();
            }
            
        }
        else if(currentState == State.Dead)
        {
            _timeCounter.IncreaseTimer("deadTimer",out bool limit);
            if(limit)
            {
                transform.SetPositionAndRotation(_posOrigin,Quaternion.identity);
                body.isKinematic = true;
                footPointRotator.enabled = true;
                ChangeState(State.Idle);
            }
        }
    }

    public void ChangeState(State state)
    {
        if(state == State.Idle)
        {
            _timeCounter.InitTimer("bombRefillTime",0f,bombRefillTime);
        }   
        else if(state == State.Go)
        {
            GetPath(goPath,false);
        }
        else if(state == State.Out)
        {
            GetPath(outPath,false);
        }
        else if(state == State.BombThorw)
        {
            _timeCounter.InitTimer("bombThrowTimer",0f,1f);
            _timeCounter.InitTimer("ThrowTimer",0f,.6f);
            animator.Play("ThrowPosition",transform);
            animator.Play("ThrowRotation",transform);
        }
        else if(state == State.Dead)
        {
            body.isKinematic = false;
            footPointRotator.enabled = false;
            _timeCounter.InitTimer("deadTimer",0f,4f);

            if(_bomb != null && !_bomb.isOver && currentState != State.Out)
            {
                _bomb.Hit();
            }

            animator.Stop();
        }

        currentState = state;
    }

    public void BombThrow()
    {
        _bomb.transform.SetParent(null);
        _bombRigidbody.isKinematic = false;
        _bombRigidbody.AddForce(Vector3.ProjectOnPlane(transform.forward,Vector3.up) * bombThrowPower,ForceMode.Impulse);
        _bombRigidbody.AddForce(Vector3.up * bombThrowPower,ForceMode.Impulse);

        bombCollector.SetBomb(_bomb);
    }

    public void InitBomb_Reuse()
    {
        _bombRigidbody.isKinematic = true;
        _bomb.Reactive();
        _bomb.transform.SetPositionAndRotation(bombPoint.position,bombPoint.rotation);
        _bomb.transform.SetParent(transform);
    }

    public void InitBomb()
    {
        _bomb = Instantiate(bombOrigin,bombPoint.position,bombPoint.rotation).GetComponent<EMPShield>();
        _bomb.transform.SetParent(transform);
        _bomb.GetComponent<EMPBomb>().destroy = true;
        _bombRigidbody = _bomb.GetComponent<Rigidbody>();
        _bombRigidbody.isKinematic = true;
    }
}
