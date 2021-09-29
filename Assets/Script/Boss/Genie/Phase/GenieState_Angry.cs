using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_Angry : GenieStateBase
{
    public override string stateIdentifier => "Angry";

    public Genie_CoreDroneAI coreDroneAI;
    public Transform spawnPosition;

    public float patternStartTime = 3f;
    public float disapearStartTime = 1f;

    public float disapearTime = 1f;
    public int disapaerCount;

    private bool _pattern = false;
    private float _deltaTime;

    private List<HexCube> _disapearList = new List<HexCube>();

    public override void Assign()
    {
        base.Assign();
        _timeCounter.CreateSequencer("Start");
        _timeCounter.AddSequence("Start",patternStartTime,LookTarget,ChangeAngry);

        _timeCounter.CreateSequencer("Process");
        _timeCounter.AddSequence("Process",disapearTime,null,RandomDisapear);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitSequencer("Start");
        _timeCounter.InitSequencer("Process");
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        _deltaTime = deltaTime;

        LookTarget(target.body,target.targetTransform.position,deltaTime);

        if(_pattern)
        {
            if(_timeCounter.ProcessSequencer("Process",deltaTime))
            {
                _timeCounter.InitSequencer("Process");
            }
        }
        else
        {
            _pattern = _timeCounter.ProcessSequencer("Start",deltaTime);
        }
    }

    public void RandomDisapear(float t)
    {
        foreach(var item in _disapearList)
        {
            item.GetRenderer().material = target.gridControll.prev;
            item.SetMove(false,0f,1f,disapearTime);
        }

        _disapearList.Clear();

        for(int i = 0; i < disapaerCount; ++i)
        {
            //target.gridControll.cubeGrid.GetRandomActiveCube(true).SetActive(false,true,disapearTime);
            var cube = target.gridControll.cubeGrid.GetRandomActiveCube(true);
            //cube.SetMove(false,0f,1f,disapearTime);

            _disapearList.Add(cube);
        }

        foreach(var item in _disapearList)
        {
            item.GetRenderer().material = target.gridControll.curr;
        }
    }

    public void ChangeAngry(float t)
    {
        //ani change
        coreDroneAI.Respawn(spawnPosition.position,false);
    }

    public void LookTarget(float t)
    {
        LookTarget(target.body,target.targetTransform.position,_deltaTime);
    }
}
