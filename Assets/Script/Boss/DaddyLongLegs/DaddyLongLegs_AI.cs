using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DaddyLongLegs_AI : IKBossBase
{
    public GraphAnimator animator;
    public Transform body;
    public Transform humanSpine;
    public Transform graphicRoot;

    public float startTime = 0.1f;
    public bool randomStart = false;
    private bool _animate = false;
    private bool _prevActive = false;

    public override void Initialize()
    {
        base.Initialize();
        //SetLegHitGroundSound(1507);
        if(randomStart)
            startTime = Random.Range(0f,3f);

        _timeCounter.InitTimer("Start",0f,startTime);

        foreach(var leg in legs)
        {
            leg.Hold(true);
        }

        graphicRoot = transform.Find("Body");
    }

    private void UpdateProcess(float deltaTime)
    {
        if (!_animate)
        {
            _timeCounter.IncreaseTimer("Start", out var limit);
            if (limit)
            {
                animator.Play("UpDownLoop", body);
                animator.Play("HumanRotateLoop", humanSpine);
                animator.Play("WalkRotate", body);

                foreach (var leg in legs)
                {
                    leg.Hold(false);
                }

                _animate = true;
            }
        }

        var cam = Camera.main;
        var dir = (transform.position - cam.transform.position);
        var dist = MathEx.distance(transform.position.z, cam.transform.position.z);
        var active = (Vector3.Angle(cam.transform.forward, dir) <= 100f);
        active = active ? dist <= 150f : dist <= 10f;

        if(_prevActive != active)
        {
            graphicRoot.gameObject.SetActive(active);
            foreach(var leg in legs)
            {
                leg.ik.SetParent(active ? null : this.transform);
                if(active)
                {
                    leg.SetStartPosition(leg.ik.position);
                }
            }
            _prevActive = active;
        }

        MoveForward(frontMoveSpeed,deltaTime);

        
        if (transform.position.z <= -268.82f)
        {
            foreach(var leg in legs)
            {
                leg.ik.SetParent(this.transform);
            }
            var pos = transform.position;
            pos.z += 580.22f;
            transform.position = pos;

            foreach(var leg in legs)
            {
                leg.SetStartPosition(leg.ik.position);
                leg.ik.SetParent(null);
            }
        }
    }

   // private void OnTriggerEnter(Collider other)
   // {
        
       // if(other.gameObject.name=="DaddyLongLegEND")
       // {
           // var pos = transform.position;
            //pos.z += 590.22f;
            //transform.position = pos;
       // }

    //}

    public override void FixedProgress(float deltaTime)
    {
        UpdateProcess(deltaTime);
    }
}
