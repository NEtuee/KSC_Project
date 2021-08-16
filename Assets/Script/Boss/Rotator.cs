using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : UnTransfromObjectBase
{
    public Vector3 speed;

    public bool play = true;
    public bool fall = false;
    public bool lerpSpeed = false;

    public float lerpFactor = 1f;
    public float startFactor = 0f;

    private List<SubRotator> _subRotators;
    private List<Rigidbody> _bodys = new List<Rigidbody>();

    public Vector3 _speed;
    private float _startFactor = 0f;

    private bool _start = false;
    public bool _stopMove = false;

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));

        _subRotators = new List<SubRotator>(transform.GetComponentsInChildren<SubRotator>(false));
        _bodys = new List<Rigidbody>(transform.GetComponentsInChildren<Rigidbody>(false));

        _speed = lerpSpeed ? Vector3.zero : speed;
    }

    public override void FixedProgress(float deltaTime)
    {
        // if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Update)
        //     return;

        // if (GameManager.Instance.PAUSE == true)
        //     return;

        if(play && !_start)
        {
            _startFactor += deltaTime;
            if(_startFactor >= startFactor)
            {
                _start = true;
            }
            else
            {
                return;
            }
        }

        if (play)
        {
            if(lerpSpeed)
            {
                if(!_stopMove)
                {
                    _speed += lerpFactor * speed.normalized * deltaTime;
                    if(_speed.magnitude >= speed.magnitude)
                    {
                        _speed = speed;
                        lerpSpeed = false;
                    }
                }
                else
                {
                    _speed -= lerpFactor * speed.normalized * deltaTime;
                    var angle = Vector3.Angle(_speed,speed);
                    if(angle >= 160f)
                    {
                        _speed = Vector3.zero;;
                        lerpSpeed = false;
                    }
                }
                
            }

            transform.rotation *= Quaternion.Euler(_speed * deltaTime);
            RotateSubRotators(deltaTime);
        }

        if(fall)
        {
            AddGravity(deltaTime);
        }
    }

    public void Fall()
    {
        fall = true;
        foreach(var rig in _bodys)
        {
            rig.gameObject.GetComponent<MeshCollider>().enabled = false;
            rig.isKinematic = false;
            rig.useGravity = true;
            rig.transform.parent = null;
            rig.constraints = RigidbodyConstraints.None;

            rig.AddTorque(MathEx.RandomVector3(-50f,50f,-50f,50f,-50f,50f));
            //rig.AddForce(MathEx.RandomVector3(0f,10f,0f,10f,0f,10f));
        }
    }

    public void SetStopMove(bool value)
    {
        _stopMove = value;
    }

    public void AddGravity(float deltaTime)
    {
        foreach(var rig in _bodys)
        {
            rig.AddForce(Vector3.down * 3f * deltaTime);
        }
    }

    public void RotateSubRotators(float deltaTime)
    {
        foreach(var rot in _subRotators)
        {
            rot.Spin(deltaTime);
        }
    }
}
