using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_DroneSwingWave : GenieStateBase
{
    public override string stateIdentifier => "SwingWave";

    public Animator droneAnimator;
    public AnimationCurve droneApearCurve;
    public Transform droneLine;
    
    public Genie_CoreDroneAI coreDroneAI;
    public Transform droneTarget;
    public LevelEdit_ExplosionPhysics explosionPhysics;
    public LevelEdit_ExplosionPhysics leftExplosionPhysics;

    public bool randomPattern = false;
    public bool cutFirst = false;
    public int groundCutFreq = 1;
    public int groundWheelFreq = 1;


    [Header("Ground Hit")]
    public float beforeGroundHitTime = 1f;
    public float groundHitTime = 1f;

    [Header("Drone Summon")]
    public float beforeDroneSummonTime = 1f;
    public float droneSummonTime = 2f;
    public float patternStartTime = 2f;

    [Header("Ground Wheel")]
    public int groundWheelThickness = 0;
    public float groundWheelPatternTime = 5f;
    public float groundWheelTimingTime = 3f;
    public float groundWheelTime = 3f;
    public float groundWheelMoveSpeed = 1f;

    [Header("Ground Cut")]
    public float groundCutAngle = 30f;
    public int groundCutThickness = 0;
    public float groundCutPatternTime = 5f;
    public float groundCutTime = 3f;
    public float groundCutMoveTime = 0.5f;
    public int groundCutRepeat = 1;
    public float groundCutRepeatTime = 1f;


    public float spinSpeed = 20f;

    private List<HexCube> _fallLine = new List<HexCube>();

    private bool _patternStart = false;

    private bool _cut = false;
    private int _groundCutFreq = 1;
    private int _groundWheelFreq = 1;

    private float _deltaTime;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("Process");
        _timeCounter.AddSequence("Process",beforeGroundHitTime,LookTarget,BeforeGroundHit);
        _timeCounter.AddSequence("Process",groundHitTime,GroundHit,GroundDisapear);
        _timeCounter.AddSequence("Process",beforeDroneSummonTime,null,(value)=> 
        {
            var data = MessageDataPooling.GetMessageData<MD.DroneTextKeyAndDurationData>();
            data.key = "Birdy_A2_GenieCoreDrone02";
            data.duration = 5f;
            target.SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, UniqueNumberBase.GetSavedNumberStatic("PlayerManager"), data);
        });
        _timeCounter.AddSequence("Process",droneSummonTime,DroneApear,SpawnCoreDrone);
        _timeCounter.AddSequence("Process",patternStartTime,null,null);

        _timeCounter.CreateSequencer("GroundWheel");
        _timeCounter.AddSequence("GroundWheel",groundWheelPatternTime,null,null);
        _timeCounter.AddSequence("GroundWheel",groundWheelTime,BeforeGroundWheel,GroundWheel);

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
        _timeCounter.InitSequencer("GroundWheel");
        _timeCounter.InitSequencer("GroundCut");

        _patternStart = false;
        droneLine.gameObject.SetActive(false);

        _cut = cutFirst;
        _groundCutFreq = groundCutFreq;
        _groundWheelFreq = groundWheelFreq;

       
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
            if(_cut)
            {
                if(_timeCounter.ProcessSequencer("GroundCut",deltaTime))
                {
                    if(randomPattern)
                    {
                        int random = Random.Range(0,2);
                        _cut = random == 0;
                        _timeCounter.InitSequencer("GroundWheel");
                        _timeCounter.InitSequencer("GroundCut");
                        return;
                    }

                    if(--_groundCutFreq <= 0)
                    {
                        _timeCounter.InitSequencer("GroundWheel");
                        _groundWheelFreq = groundWheelFreq;
                        _cut = false;
                    }
                    else
                    {
                        _timeCounter.InitSequencer("GroundCut");
                    }
                    
                }
            }
            else
            {
                if(_timeCounter.ProcessSequencer("GroundWheel",deltaTime))
                {
                    if(randomPattern)
                    {
                        int random = Random.Range(0,2);
                        _cut = random == 0;
                        _timeCounter.InitSequencer("GroundWheel");
                        _timeCounter.InitSequencer("GroundCut");
                        return;
                    }

                    if(--_groundWheelFreq <= 0)
                    {
                        _timeCounter.InitSequencer("GroundCut");
                        _groundCutFreq = groundCutFreq;
                        _cut = true;
                    }
                    else
                    {
                        _timeCounter.InitSequencer("GroundWheel");
                    }
                }
            }

            
        }
    }

    public void LaunchLeftDrones()
    {
        if (!leftExplosionPhysics.launched)
        {
            explosionPhysics.Launch();

            foreach (var item in explosionPhysics.targets)
            {
                Destroy(item, 5f);
            }
        }
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);
        explosionPhysics.Launch();
        droneLine.gameObject.SetActive(false);

        foreach(var item in _fallLine)
        {
            item.GetRenderer().material = target.gridControll.prev;
        }

        foreach(var item in explosionPhysics.targets)
        {
            Destroy(item,5f);
        }

        LaunchLeftDrones();
    }

#region GroundCut
    public void GroundCut(float t)
    {
        foreach(var item in _fallLine)
        {
            if(item.special || !item.IsActive())
                continue;

            item.SetMove(false,Random.Range(0f,0.2f),1f,3f);
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

        if(t < groundCutMoveTime)
        {
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

        
    }

#endregion

#region GroundWheel

    public void GroundWheel(float t)
    {

        var count = target.gridControll.cubeGrid.mapSize * 3 - 3;
        var angleFactor = 360f / (float)count;

        for(int i = 0; i < count; ++i)
        {
            _fallLine.Clear();
            var right = (Quaternion.Euler(0f,angleFactor * (float)i,0f) * target.body.right).normalized;
            GetGridLine(ref _fallLine,right,groundWheelThickness);
            foreach(var item in _fallLine)
            {
                if(item.special)
                {
                    continue;
                }
                item.SetMove(false,((float)i) * groundWheelTimingTime + Random.Range(0f,0.2f),groundWheelMoveSpeed,groundWheelTime);
            }


        }
    }

    public void BeforeGroundWheel(float t)
    {

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
        coreDroneAI.Respawn(droneTarget.position - Vector3.up,false);

        _timeCounter.SkipSequencer("GroundWheel",groundWheelPatternTime - 1f);
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

    public void BeforeGroundHit(float t)
    {
        target.ChangeAnimation(6);
        target.CreateEyeLight();
    }

    public void GroundDisapear(float processTime)
    {
        foreach(var item in _fallLine)
        {
            item.SetMove(false,Random.Range(0f,0.2f),1f,beforeDroneSummonTime + droneSummonTime + 1);
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
