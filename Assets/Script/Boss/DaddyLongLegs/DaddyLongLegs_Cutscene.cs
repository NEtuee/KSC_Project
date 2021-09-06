using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DaddyLongLegs_Cutscene : IKBossBase
{
    public DaddyLongLegs_CutsceneMaster master;
    public GraphAnimator animator;
    public Transform body;
    public Transform humanSpine;
    public Transform graphicRoot;

    public float startTime = 0.1f;
    public bool randomStart = false;
    private bool _animate = false;
    private bool _prevActive = true;

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
        MoveForward(frontMoveSpeed,deltaTime);
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

    public override void Progress(float deltaTime)
    {
        var cam = Camera.main;
        var dir = (transform.position - cam.transform.position);
        var dist = MathEx.distance(transform.position.z, cam.transform.position.z);
        var angle = Vector3.Angle(cam.transform.forward, dir);
        var active = angle <= 90f;
        active = active ? dist <= 140f : dist <= 10f;

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
        

        if(master.isMove && master.culling)
        {
            angle = Vector3.Angle(Vector3.ProjectOnPlane(cam.transform.forward,Vector3.up), Vector3.ProjectOnPlane(transform.forward,Vector3.up));
            if(MathEx.abs(angle) >= 90f)
            {
                if (transform.position.z > cam.transform.position.z)
                {
                    foreach(var leg in legs)
                    {
                        leg.ik.SetParent(this.transform);
                    }
                    var pos = transform.position;
                    pos.z -= 135f;
                    transform.position = pos;

                    foreach(var leg in legs)
                    {
                        leg.SetStartPosition(leg.ik.position);
                        leg.ik.SetParent(null);
                    }
                }
            }
            else
            {
                if (transform.position.z > cam.transform.position.z + 135f)
                {
                    foreach(var leg in legs)
                    {
                        leg.ik.SetParent(this.transform);
                    }
                    var pos = transform.position;
                    pos.z -= 135f;
                    transform.position = pos;

                    foreach(var leg in legs)
                    {
                        leg.SetStartPosition(leg.ik.position);
                        leg.ik.SetParent(null);
                    }
                }
            }
        }
    }

    public override void AfterProgress(float deltaTime)
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

        if (!master.isMove)
            return;

        UpdateProcess(deltaTime);
    }
}
