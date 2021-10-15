using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class B1_StageSequencer : ObjectBase
{
    public enum SequenceType
    {
        SpawnObstacle,
        SpawnPlatformSet,
        SpawnBoss,
        WaitSeconds,
        LoadNextScene,
        SwitchPlayerPlatform,
        DisconnectFence,
        TriggerFence,
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
    private Dictionary<int, bool> _triggerSet = new Dictionary<int, bool>();

    private B1_Platform _recentlyPlatform;
    private bool _process = false;

    public override void Assign()
    {
        base.Assign();
        CreateSequencer();

        AddAction(MessageTitles.customTitle_start,(x)=>{
            var item = MessageDataPooling.CastData<MD.IntData>(x.data);
            SetTrigger(item.value);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        SaveMyNumber("StageSequencer",true);
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

    public void SwitchMainPlatform()
    {
        if(_recentlyPlatform == null || !_recentlyPlatform.gameObject.activeInHierarchy)
        {
            Debug.Log("target is missing");
            return;
        }
        
        background.SwitchMainPlatform(playerPlatform,_recentlyPlatform);
        playerPlatform = _recentlyPlatform;
    }

    public void SetTrigger(int code)
    {
        if(!_triggerSet.ContainsKey(code))
        {
            Debug.LogError("Key does not exists");
            return;
        }

        _triggerSet[code] = true;
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
            else if(item.type == SequenceType.SpawnPlatformSet)
            {
                _timeCounter.AddSequence("Main",0f,null,(x)=>{
                    _recentlyPlatform = background.SpawnPlatformFront(item.code,(int)item.factor,playerPlatform);
                });
            }
            else if(item.type == SequenceType.SpawnBoss)
            {
                _timeCounter.AddSequence("Main",item.factor,null,null);
            }
            else if(item.type == SequenceType.WaitSeconds)
            {
                _timeCounter.AddSequence("Main",item.factor,null,null);
            }
            else if(item.type == SequenceType.SwitchPlayerPlatform)
            {
                _timeCounter.AddSequence("Main",item.factor,null,(x)=>{
                    SwitchMainPlatform();
                });
            }
            else if(item.type == SequenceType.DisconnectFence)
            {
                _timeCounter.AddFence("Main",()=>{return playerPlatform.IsDisconnected();});
            }
            else if(item.type == SequenceType.TriggerFence)
            {
                if(_triggerSet.ContainsKey(item.code))
                {
                    Debug.LogError("Trigger Already Exists");
                    return;
                }
                _triggerSet.Add(item.code,false);
                //_triggerEx.SetTrigger(item.code,false);
                _timeCounter.AddFence("Main",()=>{return _triggerSet[item.code];});
            }
        }
    }
}
