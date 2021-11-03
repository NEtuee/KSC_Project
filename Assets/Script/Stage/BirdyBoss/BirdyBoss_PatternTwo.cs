using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_PatternTwo : ObjectBase
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
            BirdyApear,
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
    public BirdyBoss_Database database;
    public HexCubeGrid cubeGrid;

    public Transform ceilingSpawnPosition;
    public NewEmpShield empShield;

    public PlayerRePositionor respawn;

    public bool active = false;
    public int birdyHP = 3;

    [Header("Birdy Parts")]
    public BirdyBoss_HeadPattern headPattern;
    public DissolveControl bodyGraphic;

    List<HexCube> _spawnCubeList = new List<HexCube>();
    List<HexCube> _medusaSpawnList = new List<HexCube>();
    List<HexCube> _medusaSpawnNear = new List<HexCube>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private int _hp;
    private float _fillAmountTarget = 1f;

    private bool _droneActive = false;
    private bool _loopProcess = false;
    private bool _respawn = false;

    private PlayerUnit _player;
    private Drone _drone;
    private HexCube _respawnCube;

    public override void Assign()
    {
        base.Assign();

        CreateSequencer("process",ref sequences);
        CreateSequencer("loop",ref loopSequences);

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
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!active)
            return;

        _timeCounter.ProcessSequencer("process",deltaTime);

        if(_loopProcess)
        {
            if(_timeCounter.ProcessSequencer("loop",deltaTime))
            {
                _timeCounter.InitSequencer("loop");
            }
        }

        if(_droneActive)
        {
            _drone.FollowPathStraight(deltaTime);
            _drone.transform.LookAt(_drone.targetTransform);
            empShield.transform.position = _drone.transform.position;
        }
    }

    public void ActiveDrone()
    {
        _droneActive = true;
        _drone.StartDissolveEffect();
        _drone.SetPath("PatternTwo_Head",true);
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
            _drone.transform.SetParent(bodyGraphic.transform);
            _drone.transform.localPosition = Vector3.zero;

            bodyGraphic.gameObject.SetActive(true);
            bodyGraphic.Active(3f);
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
                        var cube = GetRandomMedusaCube();

                        _timeCounter.AddSequence(name,loopTime,null,(x)=>{
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
            else if (item.type == SequenceItem.EventEnum.BirdyApear)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    ActiveDrone();
                });
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
