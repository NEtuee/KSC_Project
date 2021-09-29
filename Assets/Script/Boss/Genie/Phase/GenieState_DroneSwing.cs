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

    public float beforeGroundHitTime = 1f;
    public float groundHitTime = 1f;
    public float beforeDroneSummonTime = 1f;
    public float droneSummonTime = 2f;
    public float patternStartTime = 2f;


    public float groundCutAngle = 30f;
    public int groundCutThickness = 0;
    public float groundCutPatternTime = 5f;
    public float groundCutTime = 3f;

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
        _timeCounter.AddSequence("Process",droneSummonTime,DroneApear,null);
        _timeCounter.AddSequence("Process",patternStartTime,null,null);

        _timeCounter.CreateSequencer("GroundCut");
        _timeCounter.AddSequence("GroundCut",groundCutPatternTime,null,null);
        _timeCounter.AddSequence("GroundCut",groundCutTime,BeforeGroundCut,GroundCut);

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

#region GroundCut

    public void GroundCut(float t)
    {
        foreach(var item in _fallLine)
        {
            item.SetActive(false,true,3f);
            item.GetRenderer().material = target.gridControll.prev;
        }

        _fallLine.Clear();
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
        GetGridLine(dir,groundCutThickness);

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.curr;
        }
    }

#endregion

#region DroneStuff
    public void DroneExplosion()
    {
        droneAnimator.SetTrigger("Explosion");
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
            item.SetActive(false,true,beforeDroneSummonTime + droneSummonTime + 1);
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
        GetGridLine(target.body.right);
        GetGridLine(-target.body.right);

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.curr;
        }
    }
#endregion
    public void GetGridLine(Vector3 dir, int loop = 6)
    {
        var start = target.gridControll.cubeGrid.GetCubePointFromWorld(transform.position);
        var end = transform.position + dir * 50f;
        var endPoint = target.gridControll.cubeGrid.GetCubePointFromWorld(end);
        if(loop > 0)
            target.gridControll.cubeGrid.GetCubeLineHeavy(ref _fallLine, start,endPoint,0,loop);
        else
            target.gridControll.cubeGrid.GetCubeLine(ref _fallLine,start,endPoint);
    }
}
