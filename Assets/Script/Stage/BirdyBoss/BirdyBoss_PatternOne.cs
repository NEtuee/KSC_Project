using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_PatternOne : ObjectBase
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
            HeadStemp,
            BirdyApear,
            SpawnFlySpiderBall,
            StartFog,
            EndFog,
            GenieHitGround,
            Giro
        };
        
        public string identifier;
        public EventEnum type;
        public int code;
        public int point;
        public float value;

        public UnityEngine.Events.UnityEvent eventSet;
    }

    [System.Serializable]
    public class LoopSequence
    {
        public string title;
        public List<SequenceItem> loopSequences = new List<SequenceItem>();
        public bool active = false;
    }


    [Header("Sequence Lists")]
    public List<SequenceItem> sequences = new List<SequenceItem>();
    public List<LoopSequence> loopSequences = new List<LoopSequence>();

    [Header("etc.")]
    public List<Transform> spawnPositions = new List<Transform>();
    public List<Transform> ceilingSpawnPositions = new List<Transform>();
    public BirdyBoss_Database database;
    public HexCubeGrid cubeGrid;

    public Transform ceilingSpawnPosition;
    public NewEmpShield empShield;

    public PlayerRePositionor respawn;

    public bool active = false;
    public int birdyHP = 3;

    [Header("Birdy Parts")]
    public BirdyBoss_HeadPattern headPattern;


    [Header("Fog")]
    public Genie_CoreDroneAI fogDrone;
    public float fogDensity = 0.09f;
    public float fogOutDensity = 0.01f;

    [Header("Genie")]
    public BridyBoss_GeniePlatform geniePlatform;

    [Header("GiroPattern")]
    public GiroPattern giroPattern;

    List<HexCube> _spawnCubeList = new List<HexCube>();
    List<HexCube> _medusaSpawnList = new List<HexCube>();
    List<HexCube> _medusaSpawnNear = new List<HexCube>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private int _hp;
    private float _fillAmountTarget = 1f;

    private bool _droneActive = false;
    private bool _respawn = false;
    private bool _fogIn = false;

    private bool _fogOutProcess = false;


    private PlayerUnit _player;
    private Drone _drone;
    private HexCube _respawnCube;

    public override void Assign()
    {
        base.Assign();

        CreateSequencer("process",ref sequences);

        for(int i = 0; i < loopSequences.Count; ++i)
        {
            CreateSequencer(loopSequences[i].title, ref loopSequences[i].loopSequences);
        }
        

        _timeCounter.CreateSequencer("Respawn");
        _timeCounter.AddSequence("Respawn",5f,null,(x)=>{
            _respawnCube.special = false;
        });

        _hp = birdyHP;

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            _drone = _player.Drone;

            SetActive(active);
        });

        _timeCounter.CreateSequencer("FogOut");
        _timeCounter.AddSequence("FogOut", 3f, (x) =>
        {
            if (!_fogIn)
                return;

            var factor = x / 3f;
            RenderSettings.fogDensity = Mathf.Lerp(fogDensity, fogOutDensity, factor);
        }, (x) => { _fogIn = false; });
    }

    public override void Initialize()
    {
        base.Initialize();
        FindGrids(4, 10);

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(_respawn)
        {
            _respawn = !_timeCounter.ProcessSequencer("Respawn",deltaTime);
        }

        if(_fogOutProcess)
        {
            _respawn = !_timeCounter.ProcessSequencer("FogOut", deltaTime);
        }
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!active)
            return;

        _timeCounter.ProcessSequencer("process",deltaTime);

        for (int i = 0; i < loopSequences.Count; ++i)
        {
            if (!loopSequences[i].active)
                continue;

            if(_timeCounter.ProcessSequencer(loopSequences[i].title, deltaTime))
            {
                _timeCounter.InitSequencer(loopSequences[i].title);
            }
        }

        if(_droneActive)
        {
            _drone.FollowPathStraight(deltaTime);
            _drone.transform.LookAt(_drone.targetTransform);
            empShield.transform.position = _drone.transform.position;
        }
    }

    public void FogOut()
    {
        _fogOutProcess = true;
        fogDrone.gameObject.SetActive(false);
        _timeCounter.InitSequencer("FogOut");
    }

    public void ActiveDrone()
    {
        _droneActive = true;
        _drone.StartDissolveEffect();
        _drone.SetPath("PatternOne_Head",true);
        _drone.moveSpeed = 3;
        empShield.gameObject.SetActive(true);
        empShield.Reactive();
    }

    public void SetActive(bool value)
    {
        active = value;

        if (active)
        {
            _drone.canMove = false;
            _drone.gameObject.SetActive(value);
            _drone.transform.SetParent(headPattern.transform);
            _drone.transform.localPosition = Vector3.zero;
        }
    }

    public void LoopStart(int target)
    {
        if (loopSequences[target].active)
            Debug.LogError("target is already activated");
        loopSequences[target].active = true;
    }

    public void LoopStop(int target)
    {
        if (!loopSequences[target].active)
            Debug.LogError("target is already deactivated");
        loopSequences[target].active = false;
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
                        
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            var cube = GetRandomCube();
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
                        
                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            var cube = GetRandomCube();
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
                        _timeCounter.AddSequence(name,0f,null,(x)=>{
                            var cube = GetRandomMedusaCube();
                            var target = database.SpawnMedusa();
                            target.transform.position = cube.transform.position + Vector3.up * .9f;

                            target.GetComponent<BirdyBoss_Medusa>().Launch();
                            
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for(int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
                            var cube = GetRandomMedusaCube();
                            var target = database.SpawnMedusa();
                            target.transform.position = cube.transform.position + Vector3.up * .9f;

                            target.GetComponent<BirdyBoss_Medusa>().Launch();

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
            else if (item.type == SequenceItem.EventEnum.HeadStemp)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    if(item.code == 0)
                    {
                        var cube = cubeGrid.GetCubeFromWorld(_player.Position, false);

                        if (cube != null)
                            headPattern.StempTarget(cube);
                    }
                    else
                    {
                        var cube = cubeGrid.GetCube(Vector3Int.zero, false);

                        if (cube != null)
                            headPattern.StempTarget(cube);
                    }
                    
                });
            }
            else if (item.type == SequenceItem.EventEnum.BirdyApear)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    ActiveDrone();
                });
            }
            else if (item.type == SequenceItem.EventEnum.SpawnFlySpiderBall)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    var cube = GetRandomMedusaCube();

                    if(cube != null)
                    {
                        var fly = database.SpawnFlySpiderBall();
                        fly.StempTarget(cube);
                    }
                });
            }
            else if (item.type == SequenceItem.EventEnum.StartFog)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    if (_fogIn)
                        return;

                    var factor = x / item.value;
                    RenderSettings.fogDensity = Mathf.Lerp(fogOutDensity,fogDensity,factor);

                    fogDrone.Respawn(headPattern.transform.position);

                    _fogIn = true;
                });
            }
            else if (item.type == SequenceItem.EventEnum.EndFog)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    if (!_fogIn)
                        return;
                    
                    var factor = x / item.value;
                    RenderSettings.fogDensity = Mathf.Lerp(fogDensity, fogOutDensity, factor);

                    fogDrone.gameObject.SetActive(false);

                    _fogIn = false;
                });
            }
            else if (item.type == SequenceItem.EventEnum.GenieHitGround)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    geniePlatform.Active();
                });
            }
            else if(item.type == SequenceItem.EventEnum.Giro)
            {
                _timeCounter.AddSequence(name, 0.0f, null, (x) =>
                 {
                     giroPattern.Appear();
                 });

                _timeCounter.AddSequence(name, 3.0f, null, (x) =>
                {
                    //Debug.Log("wait");
                });

                for (int i = 0; i < giroPattern.ObjectCount; i++)
                {
                    int count = i;
                    _timeCounter.AddSequence(name, 1f, null, (value) =>
                     {
                         giroPattern.Launch(count, _player.Transform.position, 5000f);
                     });
                }

                _timeCounter.AddSequence(name, 4.0f, null, null);
            }
        }
    }

    public void SetRandomRespawn()
    {
        if(!active)
            return;

        
        _respawnCube = cubeGrid.GetRandomActiveCube(true);
        _respawnCube.MoveToUp();
        _respawnCube.special = true;
        respawn.SetRespawnPoint(_respawnCube.transform);

        _timeCounter.InitSequencer("Respawn");

        _respawn = true;
    }

    public void UpdateImageAmount()
    {
        _fillAmountTarget = _hp / birdyHP;
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
        if(_medusaSpawnList.Count == 0)
            return null;

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
