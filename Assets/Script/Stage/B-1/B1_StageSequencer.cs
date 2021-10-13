using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class B1_StageSequencer : ObjectBase
{
    public enum SequenceType
    {
        SpawnObstacle,
        SpawnMonsterSet,
        SpawnMedusa,
        SpawnBoss,
        WaitSeconds,
        LoadNextScene,
        Fence
    };

    [System.Serializable]
    public class SequenceItemBase
    {
        public SequenceType type;
        public int code;
        public float factor;
    }

    public List<SequenceItemBase> sequences = new List<SequenceItemBase>();
    
    public B1_Platform playerPlatform;
    public B1_LoopBackground background;

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private bool _process = false;

    public override void Assign()
    {
        base.Assign();
        CreateSequencer();
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        _process = true;
        _timeCounter.InitSequencer("Main");
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            background.SpawnObstacle(0);
        }
        if(Keyboard.current.mKey.wasPressedThisFrame)
        {
            background.SpawnPlatformFront(0,0,playerPlatform);
        }

        if(_process)
        {
            _process = !_timeCounter.ProcessSequencer("Main",deltaTime);
        }
    }

    public void CreateSequencer()
    {
        _timeCounter.CreateSequencer("Main");

        foreach(var item in sequences)
        {
            if(item.type == SequenceType.SpawnObstacle)
            {
                _timeCounter.AddSequence("Main",item.factor,null,(x)=>{
                    background.SpawnObstacle(item.code);
                });
            }
            else if(item.type == SequenceType.SpawnMonsterSet)
            {
                _timeCounter.AddSequence("Main",0f,null,(x)=>{
                    background.SpawnPlatformFront(item.code,(int)item.factor,playerPlatform);
                });
            }
            else if(item.type == SequenceType.SpawnBoss)
            {
                _timeCounter.AddSequence("Main",item.factor,null,null);
            }
            else if(item.type == SequenceType.SpawnMedusa)
            {
                _timeCounter.AddSequence("Main",item.factor,null,null);
            }
            else if(item.type == SequenceType.WaitSeconds)
            {
                _timeCounter.AddSequence("Main",item.factor,null,null);
            }
            else if(item.type == SequenceType.Fence)
            {

            }
        }
    }
}
