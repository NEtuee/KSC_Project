using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_PatternOne : ObjectBase
{
    public enum EventEnum
    {
        SpawnSpider = 0,
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
        Giro,
        FallPillar,
        HorizonPillar,
        SpiderPillar,
        GroundCutStart,
        GroundCutEnd,
        LoopPatternStart,
        LoopPatternEnd,
        LoopPatternEndFence,
        ActiveRandomTentacle,
        TentacleFence,
        GroundShot,
        ExplosionSpiderAll,
        ExplosionDroneAll,
        GroundCutV2Start,
        GroundCutV2End,
        InverseGroundPattern,
        StartHeadMove,
        HeadOut,


        PatternEND,
    };

    [System.Serializable]
    public class SequenceItem
    {
        public string identifier;
        public EventEnum type;
        public int code;
        public int point;
        public float value;
        public float value2;
        public float value3;

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
    public List<LoopSequence> sequences = new List<LoopSequence>();
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
    public Material floorFog;
    public float fogDensity = 0.09f;
    public float fogOutDensity = 0.01f;

    [Header("Genie")]
    public BridyBoss_GeniePlatform geniePlatform;

    [Header("HoriznoPillar Point")]
    public List<Transform> horizonPillarPoints = new List<Transform>();

    [Header("Platform")]
    public BirdyBoss_PlatformCut platformCut;
    public BirdyBoss_PlatformCutV2 platformCutV2;

    [Header("Tentacle")]
    public BirdyBoss_TentacleControl tentacleControl;

    public int recentlyLoop;

    public MessageEventSender truthSender;
    public SoundPlayer bgmPlayer;

    //[Header("GiroPattern")]
    //public GiroPattern giroPattern;

    List<HexCube> _groundPatternList = new List<HexCube>();

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

    private string[] _mainSequances;

    public override void Assign()
    {
        base.Assign();
        _mainSequances = new string[sequences.Count];
        for (int i = 0; i < sequences.Count; ++i)
        {
            var target = sequences[i].loopSequences;
            _mainSequances[i] = "process" + i;
            CreateSequencer(_mainSequances[i], ref target);
        }
            

        for(int i = 0; i < loopSequences.Count; ++i)
        {
            CreateSequencer(loopSequences[i].title, ref loopSequences[i].loopSequences);
        }
        

        _timeCounter.CreateSequencer("Respawn");
        _timeCounter.AddSequence("Respawn",10f,null,(x)=>{
            if (_respawnCube != null)
            {
                _respawnCube.special = false;
                _respawnCube.MoveLock(false);
            }
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
            floorFog.SetFloat("FogDensity", Mathf.Lerp(5, 0, factor));
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

        for(int i = 0; i < _mainSequances.Length; ++i)
        {
            _timeCounter.ProcessSequencer(_mainSequances[i], deltaTime);
        }

        //_timeCounter.ProcessSequencer("process",deltaTime);

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
        else if(active)
        {
            _drone.transform.localPosition = Vector3.zero;//headPattern.transform.position;
        }
    }

    public void GetMainProcessingSequences(ref List<string> targets)
    {
        targets.Clear();

        for(int i = 0; i < sequences.Count; ++i)
        {
            var seq = _timeCounter.GetSequencer(_mainSequances[i]);
            if(!seq.isEnd)
            {
                string currEvent = sequences[i].loopSequences[seq.current].identifier;
                targets.Add(sequences[i].title + ", " + currEvent);
            }
        }
    }

    public void GetLoopProcessingSequences(ref List<string> targets)
    {
        targets.Clear();

        for (int i = 0; i < loopSequences.Count; ++i)
        {
            if (!loopSequences[i].active)
                continue;

            var seq = _timeCounter.GetSequencer(loopSequences[i].title);
            if(!seq.isEnd)
            {
                string currEvent = loopSequences[i].loopSequences[seq.current].identifier;
                targets.Add(loopSequences[i].title + ", " + currEvent);
            }
        }
    }

    public void FogOut()
    {
        _fogOutProcess = true;
        //fogDrone.gameObject.SetActive(false);
        headPattern.DisableShield();
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
            _drone.transform.position = headPattern.transform.position;

            headPattern.dissolveControl.Active(0.1f);
            headPattern.Teleport();
            headPattern.DroneSound();
            headPattern.disapearTarget.SetActive(true);

            RenderSettings.fogDensity = fogOutDensity;
            floorFog.SetFloat("FogDensity", 5f);

            bgmPlayer.Play();
        }
    }

    public void SetDamage()
    {
        SendMessageEx(MessageTitles.uimanager_damageFianlBossUi, GetSavedNumber("UIManager"), null);
    }

    public void LoopStart(int target)
    {
        if (loopSequences[target].active)
            Debug.LogError("target is already activated");

        _timeCounter.InitSequencer(loopSequences[target].title);
        loopSequences[target].active = true;
    }

    public void LoopStop(int target)
    {
        if (!loopSequences[target].active)
            Debug.LogError("target is already deactivated");
        loopSequences[target].active = false;
    }

    public void LoopStopRecently()
    {
        loopSequences[recentlyLoop].active = false;
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
            if (item.type == EventEnum.SpawnDrone)
            {
                if (item.value == 0)
                {
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, 0f, null, (x) =>
                        {
                            var target = database.SpawnCommonDrone();
                            target.transform.position = GetDroneSpawnPosition(item.point).position;
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, loopTime, null, (x) =>
                        {
                            var target = database.SpawnCommonDrone();
                            target.transform.position = GetDroneSpawnPosition(item.point).position;
                        });
                    }
                }


            }
            else if (item.type == EventEnum.SpawnFlySpider)
            {
                if (item.value == 0)
                {
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, 0f, null, (x) =>
                        {
                            var target = database.SpawnFlySpider();
                            target.transform.position = ceilingSpawnPosition.position;
                            target.transform.rotation = ceilingSpawnPosition.rotation;
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, loopTime, null, (x) =>
                        {
                            var target = database.SpawnFlySpider();
                            target.transform.position = ceilingSpawnPosition.position;
                            target.transform.rotation = ceilingSpawnPosition.rotation;
                        });
                    }
                }
            }
            else if (item.type == EventEnum.SpawnSpider)
            {
                if (item.value == 0)
                {
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, 0f, null, (x) =>
                        {
                            var target = database.SpawnSpider();
                            //target.target = centerTransform;
                            target.transform.position = GetSpawnPosition(item.point).position;
                        });
                    }
                }
                else
                {
                    float loopTime = item.value / (float)item.code;
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, loopTime, null, (x) =>
                        {
                            var target = database.SpawnSpider();
                            //target.target = centerTransform;
                            target.transform.position = GetSpawnPosition(item.point).position;
                        });
                    }
                }
            }
            else if (item.type == EventEnum.SpawnSpiderRandomGrid)
            {
                if (item.value == 0)
                {
                    for (int i = 0; i < item.code; ++i)
                    {

                        _timeCounter.AddSequence(name, 0f, null, (x) =>
                        {
                            var cube = GetRandomCube();
                            cube.SetMove(false, 0f, 1f, 0.1f, () =>
                            {
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
                    for (int i = 0; i < item.code; ++i)
                    {

                        _timeCounter.AddSequence(name, loopTime, null, (x) =>
                        {
                            var cube = GetRandomCube();
                            cube.SetMove(false, 0f, 1f, 0.1f, () =>
                            {
                                var target = database.SpawnSpider();
                                //target.target = centerTransform;
                                target.transform.position = cube.transform.position + Vector3.up * 1.5f;
                                target.transform.SetParent(cube.transform);
                            });
                        });
                    }
                }
            }
            else if (item.type == EventEnum.SpawnMedusa)
            {
                if (item.value == 0)
                {
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, 0f, null, (x) =>
                        {
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
                    for (int i = 0; i < item.code; ++i)
                    {
                        _timeCounter.AddSequence(name, loopTime, null, (x) =>
                        {
                            var cube = GetRandomMedusaCube();
                            var target = database.SpawnMedusa();
                            target.transform.position = cube.transform.position + Vector3.up * .9f;

                            target.GetComponent<BirdyBoss_Medusa>().Launch();

                        });
                    }
                }
            }
            else if (item.type == EventEnum.WaitSeconds)
            {
                _timeCounter.AddSequence(name, item.value, null, null);
            }
            else if (item.type == EventEnum.AnnihilationFence)
            {
                _timeCounter.AddFence(name, () =>
                {
                    return database.updateCount == 0;
                });
            }
            else if (item.type == EventEnum.InvokeEvent)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    item.eventSet?.Invoke();
                });
            }
            else if (item.type == EventEnum.CutsceneFence)
            {
                _timeCounter.AddFence(name, () =>
                {
                    return !LevelEdit_TimelinePlayer.CUTSCENEPLAY;
                });
            }
            else if (item.type == EventEnum.DroneAnnihilationFence)
            {
                _timeCounter.AddFence(name, () =>
                {
                    return database.droneCache.updateCount == 0;
                });
            }
            else if (item.type == EventEnum.ActiveHPUI)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    //var data = MessageDataPooling.GetMessageData<MD.BoolData>();
                    //data.value = true;
                    //SendMessageEx(MessageTitles.uimanager_enableDroneStatusUi, GetSavedNumber("UIManager"), data);
                    //SendMessageEx(MessageTitles.uimanager_ActiveFianlHp, GetSavedNumber("UIManager"), null);

                    //SendMessageEx(MessageTitles.uimanager_ActiveLeveLineUIAndSetBossName, GetSavedNumber("UIManager"), "지식인 버디");

                    SendMessageEx(MessageTitles.uimanager_activeBirdyBossNameAndHp, GetSavedNumber("UIManager"), "지식인 버디");

                    //var dialogData = MessageDataPooling.GetMessageData<MD.DroneTextKeyAndDurationData>();
                    //dialogData.key = "Birdy_BossBirdy_Truth02";
                    //dialogData.duration = 5f;
                    //SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, GetSavedNumber("PlayerManager"), data);

                    truthSender.Send();
                });
            }
            else if (item.type == EventEnum.DeactiveHPUI)
            {
                //_timeCounter.AddSequence(name, 0f, null, (x) =>
                //{
                //    var data = MessageDataPooling.GetMessageData<MD.BoolData>();
                //    data.value = false;
                //    SendMessageEx(MessageTitles.uimanager_enableDroneStatusUi, GetSavedNumber("UIManager"), data);
                //});
            }
            else if (item.type == EventEnum.HeadStemp)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    if (item.code == 0)
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
            else if (item.type == EventEnum.BirdyApear)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    ActiveDrone();
                });
            }
            else if (item.type == EventEnum.SpawnFlySpiderBall)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    var cube = GetRandomMedusaCube();

                    if (cube != null)
                    {
                        var fly = database.SpawnFlySpiderBall();
                        fly.StempTarget(cube);
                    }
                });
            }
            else if (item.type == EventEnum.StartFog)
            {
                _timeCounter.AddSequence(name, item.value, (x) =>
                {
                    if (_fogIn)
                        return;

                    var factor = x / item.value;
                    RenderSettings.fogDensity = Mathf.Lerp(fogOutDensity, fogDensity, factor);
                    floorFog.SetFloat("FogDensity", Mathf.Lerp(5, 0, factor));
                }, (x) =>
                {
                    if (_fogIn)
                        return;

                    //fogDrone.Respawn(headPattern.transform.position);
                    headPattern.FogPathFollow();
                    _fogIn = true;
                });
            }
            else if (item.type == EventEnum.EndFog)
            {
                _timeCounter.AddSequence(name, item.value, (x) =>
                {
                    if (!_fogIn)
                        return;

                    //if(fogDrone.gameObject.activeInHierarchy)
                    //{
                    //    fogDrone.gameObject.SetActive(false);
                    //}
                    headPattern.DisableShield();

                    var factor = x / item.value;
                    RenderSettings.fogDensity = Mathf.Lerp(fogDensity, fogOutDensity, factor);
                    floorFog.SetFloat("FogDensity", Mathf.Lerp(0, 5, factor));
                }, (x) =>
                {
                    _fogIn = false;
                });
            }
            else if (item.type == EventEnum.GenieHitGround)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    geniePlatform.Active();
                });
            }
            else if (item.type == EventEnum.Giro)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    var target = database.SpawnGiroPattern();
                    target.transform.position = headPattern.transform.position;
                    target.transform.SetParent(headPattern.transform);
                    target.Launch(item.value, item.value2);
                });
            }
            else if (item.type == EventEnum.FallPillar)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    var target = database.SpawnFallPillarPattern();
                    for (int i = 0; i < item.code; i++)
                    {
                        var cube = cubeGrid.GetRandomActiveCube(false);
                        target.AddFallPosition(cubeGrid.CubePointToWorld(cube.cubePoint) + Vector3.up * 20.0f);
                    }
                });
            }
            else if (item.type == EventEnum.HorizonPillar)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    var target = database.SpawnHorizonPillarPattern();
                    target.Launch(ref horizonPillarPoints, _player.transform, item.value, item.value2, item.value3);
                    //target.SetPoint(ref horizonPillarPoints);
                });
            }
            else if (item.type == EventEnum.SpiderPillar)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    var target = database.SpawnSpiderPillarPattern();
                    target.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    target.Launch(item.code, item.value, item.value2);
                });
            }
            else if (item.type == EventEnum.GroundCutStart)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    platformCut.PatternStart(_player.transform);
                });
            }
            else if (item.type == EventEnum.GroundCutEnd)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    platformCut.PatternEnd();
                });
            }
            else if (item.type == EventEnum.LoopPatternStart)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    LoopStart(item.code);
                    recentlyLoop = item.code;
                });
            }
            else if (item.type == EventEnum.LoopPatternEnd)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    LoopStop(item.code);
                });
            }
            else if (item.type == EventEnum.LoopPatternEndFence)
            {
                _timeCounter.AddFence(name, () =>
                 {
                     return !loopSequences[recentlyLoop].active;
                 });
            }
            else if (item.type == EventEnum.ActiveRandomTentacle)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                 {
                     tentacleControl.StartRandomTentacle();
                 });
            }
            else if (item.type == EventEnum.TentacleFence)
            {
                _timeCounter.AddFence(name, () =>
                {
                    return !tentacleControl.IsTentacleActivate();
                });
            }
            else if (item.type == EventEnum.GroundShot)
            {
                _timeCounter.AddSequence(name, item.value, null, (x) =>
                {
                    headPattern.Shot();
                });
            }
            else if (item.type == EventEnum.ExplosionSpiderAll)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    database.ExplosionGroundSpiders();
                });
            }
            else if (item.type == EventEnum.ExplosionDroneAll)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    database.ExplosionDrones();
                });
            }
            else if (item.type == EventEnum.GroundCutV2Start)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    platformCutV2.PatternStart(_player.transform);
                });
            }
            else if (item.type == EventEnum.GroundCutV2End)
            {
                _timeCounter.AddSequence(name, 0f, null, (x) =>
                {
                    platformCutV2.PatternEnd();
                });
            }
            else if (item.type == EventEnum.InverseGroundPattern)
            {
                _timeCounter.AddSequence(name, (0f), null, (x) =>
                {
                    for (int i = cubeGrid.mapSize / 2 + 1; i > 0; --i)
                    {
                        int mapsize = cubeGrid.mapSize / 2 + 1;
                        float count = (float)(mapsize - i);
                        int point = i;

                        _groundPatternList.Clear();
                        cubeGrid.GetCubeRing(ref _groundPatternList, Vector3Int.zero, point - 1);

                        foreach (var hexCube in _groundPatternList)
                        {
                            hexCube.SetMove(false, count * item.value + item.value2, 1f, item.value3);
                            //hexCube.SetAlertTime(1f);
                        }
                    }

                    var centerCube = cubeGrid.GetCube(Vector3Int.zero);
                    centerCube.SetMove(false, (float)(cubeGrid.mapSize / 2 + 1) * item.value, 1f, item.value3);
                   // centerCube.SetAlertTime(1f);
                });
            }
            else if (item.type == EventEnum.StartHeadMove)
            {
                _timeCounter.AddSequence(name, (0f), null, (x) =>
                {
                    headPattern.PathFollow("FogBirdyPath");
                });
            }
            else if (item.type == EventEnum.HeadOut)
            {
                _timeCounter.AddSequence(name, (0f), null, (x) =>
                {
                    headPattern.QuickOut();
                });
            }

        }
    }

    public void DroneDeactive()
    {
        _drone.enabled = false;
        _drone.gameObject.SetActive(false);
    }

    public void MoveUpAll()
    {
        cubeGrid.MoveUpALL();
    }

    public void SetRandomRespawn()
    {
        if(!active)
            return;

        if(_respawnCube != null)
        {
            _respawnCube.MoveLock(false);
            _respawnCube.special = false;
        }

        _respawnCube = cubeGrid.GetRandomActiveCube(true);
        _respawnCube.MoveToUp();
        _respawnCube.special = true;
        _respawnCube.MoveLock(true);
        respawn.SetRespawnPoint(_respawnCube.originWorldPosition);

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
        int count = 10;
        while(!cube.IsActive())
        {
            cube = _spawnCubeList[Random.Range(0,_spawnCubeList.Count)];

            if(--count <= 0)
            {
                break;
            }
        }

        return cube;
    }

    public HexCube GetRandomMedusaCube()
    {
        if(_medusaSpawnList.Count == 0)
            return null;

        var cube = _medusaSpawnList[Random.Range(0,_medusaSpawnList.Count)];
        int count = 10;

        while (!cube.IsActive())
        {
            cube = _medusaSpawnList[Random.Range(0,_medusaSpawnList.Count)];

            if (--count <= 0)
            {
                break;
            }
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
