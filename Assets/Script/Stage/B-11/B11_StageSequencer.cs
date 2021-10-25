using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B11_StageSequencer : ObjectBase
{
    [System.Serializable]
    public class SequenceItem
    {
        public enum EventEnum
        {
            SpawnSpider,
            SpawnSpiderRandomGrid,
            SpawnFlySpider,
            SpawnDrone,
            SpawnMedusa,
            WaitSeconds,
            AnnihilationFence,
            CutsceneFence,
            InvokeEvent,
            DroneAnnihilationFence,
            ActiveHPUI,
            DeactiveHPUI,
        };
        
        public string identifier;
        public EventEnum type;
        public int code;
        public int point;
        public float value;

        public UnityEngine.Events.UnityEvent eventSet;
    }

    [Header("Sequence Lists")]
    public List<SequenceItem> sequences = new List<SequenceItem>();
    public List<SequenceItem> loopSequences = new List<SequenceItem>();

    [Header("etc.")]
    public List<Transform> spawnPositions = new List<Transform>();
    public List<Transform> ceilingSpawnPositions = new List<Transform>();
    public B11_Database database;
    public HexCubeGrid cubeGrid;

    public Transform centerTransform;
    public Transform ceilingSpawnPosition;

    public int genieHP = 10;

    List<HexCube> _spawnCubeList = new List<HexCube>();
    List<HexCube> _medusaSpawnList = new List<HexCube>();
    List<HexCube> _medusaSpawnNear = new List<HexCube>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private int _hp;
    private float _fillAmountTarget = 1f;

    private bool _loopProcess = false;

    private PlayerUnit _player;

    public override void Assign()
    {
        base.Assign();

        FindGrids(4,7);
        CreateSequencer("process",ref sequences);
        CreateSequencer("loop",ref loopSequences);


        _timeCounter.AddSequence("process", 0f, null, (x) =>
        {
            var data = MessageDataPooling.GetMessageData<MD.TriggerData>();
            data.name = "BClear";
            data.trigger = true;

            SendMessageEx(MessageTitles.boolTrigger_setTrigger, UniqueNumberBase.GetSavedNumberStatic("GlobalTriggerManager"), data);
        });

        _hp = genieHP;

        AddAction(MessageTitles.customTitle_start + 10,(msg)=>{
            var drone = ((CommonDrone)msg.data);
            var dist = Vector3.Distance(drone.transform.position, centerTransform.position);
            if(drone.explosionDistance > dist)
            {
                DecreaseHP(1);
            }
        });

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        _timeCounter.ProcessSequencer("process",deltaTime);

        if(_loopProcess)
        {
            if(_timeCounter.ProcessSequencer("loop",deltaTime))
            {
                _timeCounter.InitSequencer("loop");
            }
        }
    }

    public void LoopStart()
    {
        _loopProcess = true;
    }

    public void LoopStop()
    {
        _loopProcess = false;
    }

    public void FindGrids(int min, int max)
    {
        for(int i = min; i <= max; ++i)
        {
            cubeGrid.GetCubeRing(ref _spawnCubeList,Vector3Int.zero,i);
        }

        cubeGrid.GetCubeRing(ref _medusaSpawnList,Vector3Int.zero,5);
    }

    public void CreateSequencer(string name, ref List<SequenceItem> targetList)
    {
        _timeCounter.CreateSequencer(name);

        foreach(var item in targetList)
        {
            if(item.type == SequenceItem.EventEnum.SpawnDrone)
            {
                if(item.value == 0)
                {
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            var target = database.SpawnCommonDrone();
                            target.SetTarget(centerTransform);
                            target.transform.position = GetDroneSpawnPosition(item.point).position;
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            var target = database.SpawnCommonDrone();
                            target.SetTarget(centerTransform);
                            target.transform.position = GetDroneSpawnPosition(item.point).position;
                        });
                    }
                }
                
                
            }
            else if(item.type == SequenceItem.EventEnum.SpawnFlySpider)
            {
                if(item.value == 0)
                {
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            var target = database.SpawnFlySpider();
                            target.transform.position = ceilingSpawnPosition.position;
                            target.transform.rotation = ceilingSpawnPosition.rotation;
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            var target = database.SpawnFlySpider();
                            target.transform.position = ceilingSpawnPosition.position;
                            target.transform.rotation = ceilingSpawnPosition.rotation;
                        });
                    }
                }
            }
            else if(item.type == SequenceItem.EventEnum.SpawnSpider)
            {
                if(item.value == 0)
                {
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            var target = database.SpawnSpider();
                            //target.target = centerTransform;
                            target.transform.position = GetSpawnPosition(item.point).position;
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            var target = database.SpawnSpider();
                            //target.target = centerTransform;
                            target.transform.position = GetSpawnPosition(item.point).position;
                        });
                    }
                }
            }
            else if(item.type == SequenceItem.EventEnum.SpawnSpiderRandomGrid)
            {
                if(item.value == 0)
                {
                    for(int i = 0; i < item.code; ++i)
                    {
                        var cube = GetRandomCube();
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            cube.SetMove(false,0f,1f,0.1f,()=>{
                                var target = database.SpawnSpider();
                                //target.target = centerTransform;
                                target.transform.position = cube.transform.position + Vector3.up * 1.5f;
                                target.transform.SetParent(cube.transform);
                            });
                            
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for(int i = 0; i < item.code; ++i)
                    {
                        var cube = GetRandomCube();
                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            cube.SetMove(false,0f,1f,0.1f,()=>{
                                var target = database.SpawnSpider();
                                //target.target = centerTransform;
                                target.transform.position = cube.transform.position + Vector3.up * 1.5f;
                                target.transform.SetParent(cube.transform);
                            });
                        });
                    }
                }
            }
            else if(item.type == SequenceItem.EventEnum.SpawnMedusa)
            {
                if(item.value == 0)
                {
                    for(int i = 0; i < item.code; ++i)
                    {
                        var cube = GetRandomMedusaCube();
        
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            _medusaSpawnNear.Clear();
                            cubeGrid.GetCubeRing(ref _medusaSpawnNear,cube.cubePoint,1);
                            foreach(var item in _medusaSpawnNear)
                            {
                                item.SetMove(false,0f,1f,0.1f);
                            }
                            
                            cube.SetMove(false,0f,1f,0.1f,()=>{
                                var target = database.SpawnMedusa();
                                target.transform.position = cube.transform.position + Vector3.up * .9f;
                                target.transform.SetParent(cube.transform);
                            });
                            
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for(int i = 0; i < item.code; ++i)
                    {
                        var cube = GetRandomMedusaCube();

                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            _medusaSpawnNear.Clear();
                            cubeGrid.GetCubeRing(ref _medusaSpawnNear,cube.cubePoint,1);
                            foreach(var item in _medusaSpawnNear)
                            {
                                item.SetMove(false,0f,1f,0.1f);
                            }

                            cube.SetMove(false,0f,1f,0.1f,()=>{
                                var target = database.SpawnMedusa();
                                target.transform.position = cube.transform.position + Vector3.up * .9f;
                                target.transform.SetParent(cube.transform);
                            });
                        });
                    }
                }
            }
            else if(item.type == SequenceItem.EventEnum.WaitSeconds)
            {
                _timeCounter.AddSequence(name,item.value,null,null);
            }
            else if(item.type == SequenceItem.EventEnum.AnnihilationFence)
            {
                _timeCounter.AddFence(name,()=>{
                    return database.updateCount == 0;
                });
            }
            else if(item.type == SequenceItem.EventEnum.InvokeEvent)
            {
                _timeCounter.AddSequence(name,item.value,null,(x)=>{
                    item.eventSet?.Invoke();
                });
            }
            else if(item.type == SequenceItem.EventEnum.CutsceneFence)
            {
                _timeCounter.AddFence(name,()=>{
                    return !LevelEdit_TimelinePlayer.CUTSCENEPLAY;
                });
            }
            else if (item.type == SequenceItem.EventEnum.DroneAnnihilationFence)
            {
                _timeCounter.AddFence(name, () => {
                    return database.droneCache.updateCount == 0;
                });
            }
            else if (item.type == SequenceItem.EventEnum.ActiveHPUI)
            {
                _timeCounter.AddSequence(name, 0f, null, (x)=>
                {
                    var data = MessageDataPooling.GetMessageData<MD.BoolData>();
                    data.value = true;
                    SendMessageEx(MessageTitles.uimanager_enableDroneStatusUi, GetSavedNumber("UIManager"), data);
                    
                });
            }
            else if (item.type == SequenceItem.EventEnum.DeactiveHPUI)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    var data = MessageDataPooling.GetMessageData<MD.BoolData>();
                    data.value = false;
                    SendMessageEx(MessageTitles.uimanager_enableDroneStatusUi, GetSavedNumber("UIManager"), data);
                });
            }
        }
    }

    public void DecreaseHP(int factor)
    {
        _hp -= factor;
        _hp = _hp < 0 ? 0 : _hp;
        var data = MessageDataPooling.GetMessageData<MD.IntData>();
        data.value = _hp;
        SendMessageEx(MessageTitles.uimanager_setDroneHpValue, GetSavedNumber("UIManager"), data);

        UpdateImageAmount();
        if(_hp <= 0)
        {
            _player.PlayerDead();
        }
    }

    public void UpdateImageAmount()
    {
        _fillAmountTarget = _hp / genieHP;
    }

    public HexCube GetRandomCube()
    {
        var cube = _spawnCubeList[Random.Range(0,_spawnCubeList.Count)];
        while(!cube.IsActive())
        {
            cube = _spawnCubeList[Random.Range(0,_spawnCubeList.Count)];
        }

        return cube;
    }

    public HexCube GetRandomMedusaCube()
    {
        var cube = _medusaSpawnList[Random.Range(0,_medusaSpawnList.Count)];
        while(!cube.IsActive())
        {
            cube = _medusaSpawnList[Random.Range(0,_medusaSpawnList.Count)];
        }

        return cube;
    }

    public Transform GetSpawnPosition(int code)
    {
        if(code >= spawnPositions.Count || 0 < spawnPositions.Count)
        {
            return spawnPositions[Random.Range(0,spawnPositions.Count)];
        }
        else
        {
            return spawnPositions[code];
        }
    }

    public Transform GetDroneSpawnPosition(int code)
    {
        if(code >= ceilingSpawnPositions.Count || 0 < ceilingSpawnPositions.Count)
        {
            return ceilingSpawnPositions[Random.Range(0,ceilingSpawnPositions.Count)];
        }
        else
        {
            return ceilingSpawnPositions[code];
        }
    }

}
