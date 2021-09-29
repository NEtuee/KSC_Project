using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_DroneSwing : GenieStateBase
{
    public override string stateIdentifier => "Swing";

    public Animator droneAnimator;
    public AnimationCurve droneApearCurve;
    public Transform droneLine;
    
    public Genie_CoreDroneAI coreDroneAI;
    public Transform droneTarget;
    public LevelEdit_ExplosionPhysics explosionPhysics;

    public float beforeGroundHitTime = 1f;
    public float groundHitTime = 1f;
    public float beforeDroneSummonTime = 1f;
    public float droneSummonTime = 2f;
    public float patternStartTime = 2f;


    public float groundCutAngle = 30f;
    public int groundCutThickness = 0;
    public float groundCutPatternTime = 5f;
    public float groundCutTime = 3f;
    public int groundCutRepeat = 1;
    public float groundCutRepeatTime = 1f;

    public float spinSpeed = 20f;

    private List<HexCube> _fallLine = new List<HexCube>();

    private bool _patternStart = false;

    private float _deltaTime;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("Process");
        _timeCounter.AddSequence("Process",beforeGroundHitTime,LookTarget,null);
        _timeCounter.AddSequence("Process",groundHitTime,GroundHit,GroundDisapear);
        _timeCounter.AddSequence("Process",beforeDroneSummonTime,null,null);
        _timeCounter.AddSequence("Process",droneSummonTime,DroneApear,SpawnCoreDrone);
        _timeCounter.AddSequence("Process",patternStartTime,null,null);

        _timeCounter.CreateSequencer("GroundCut");
        _timeCounter.AddSequence("GroundCut",groundCutPatternTime,null,null);
        _timeCounter.AddSequence("GroundCut",groundCutTime,BeforeGroundCut,GroundCut);
        for(int i = 0; i < groundCutRepeat; ++i)
        {
            _timeCounter.AddSequence("GroundCut",groundCutRepeatTime,BeforeGroundCut,GroundCut);
        }

        droneLine.gameObject.SetActive(false);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitSequencer("Process");
        _timeCounter.InitSequencer("GroundCut");

        _patternStart = false;
        droneLine.gameObject.SetActive(false);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        _deltaTime = deltaTime;

        if(!_patternStart)
            _patternStart = _timeCounter.ProcessSequencer("Process",deltaTime);
        else
        {
            Rotate(deltaTime);
            if(_timeCounter.ProcessSequencer("GroundCut",deltaTime))
            {
                _timeCounter.InitSequencer("GroundCut");
            }
        }
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);
        explosionPhysics.Launch();
        droneLine.gameObject.SetActive(false);
        foreach(var item in explosionPhysics.targets)
        {
            Destroy(item,5f);
        }
    }

#region GroundCut

    public void GroundCut(float t)
    {
        foreach(var item in _fallLine)
        {
            if(item.special || !item.IsActive())
                continue;

            item.SetMove(false,0f,1f,3f);
            //item.SetActive(false,true,3f);
            item.GetRenderer().material = target.gridControll.prev;
        }

        _fallLine.Clear();
    }

    public void BeforeGroundCutToPlayer(float t)
    {
        var dir = target.targetTransform.position - target.body.position;
        dir.y = 0f;

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.prev;
        }

        _fallLine.Clear();
        GetGridLine(ref _fallLine,dir,groundCutThickness);

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.curr;
        }
    }

    public void BeforeGroundCut(float t)
    {
        var dir = target.targetTransform.position - target.body.position;
        dir.y = 0f;
        dir = Quaternion.Euler(0f,groundCutAngle,0f) * dir.normalized;

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.prev;
        }

        _fallLine.Clear();
        GetGridLine(ref _fallLine,dir,groundCutThickness);

        foreach(var item in _fallLine)
        {
            if(item.special || !item.IsActive())
                continue;

            item.GetRenderer().material = target.gridControll.curr;
        }
    }

#endregion

#region DroneStuff
    public void DroneExplosion()
    {
        droneAnimator.SetTrigger("Explosion");
    }

    public void SpawnCoreDrone(float time)
    {
        coreDroneAI.SetTarget(droneTarget);
        coreDroneAI.Respawn(droneTarget.position,false);
    }

    public void Rotate(float deltaTime)
    {
        target.body.rotation *= Quaternion.Euler(0f,spinSpeed * deltaTime,0f);
    }

    public void LookTarget(float processTime)
    {
        LookTarget(target.body,target.targetTransform.position,_deltaTime);
    }

    public void DroneApear(float processTime)
    {
        var proc = processTime / droneSummonTime;
        droneLine.gameObject.SetActive(true);

        var pos = droneLine.transform.position;
        pos.y = droneApearCurve.Evaluate(proc);
        droneLine.transform.position = pos;
    }

    public void GroundDisapear(float processTime)
    {
        foreach(var item in _fallLine)
        {
            item.SetMove(false,0f,1f,beforeDroneSummonTime + droneSummonTime + 1);
            //item.SetActive(false,true,beforeDroneSummonTime + droneSummonTime + 1);
            item.GetRenderer().material = target.gridControll.prev;
        }
    }

    public void GroundHit(float processTime)
    {
        LookTarget(target.body,target.targetTransform.position,_deltaTime);

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.prev;
        }

        _fallLine.Clear();
        GetGridLine(ref _fallLine,target.body.right);
        GetGridLine(ref _fallLine,-target.body.right);

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.curr;
        }
    }
#endregion
}
