using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BirdyBoss_Platform : ObjectBase
{
    public bool active = true;
    public HexCubeGrid grid;

    public List<HexCube> cubePath = new List<HexCube>();
    public List<HexCube> ringCenter;
    public string birdyPathName = "";

    public float cubeUpTerm = 0.1f;
    public float cubeSpeed = 1f;

    public float platformUpTerm = 0.3f;

    public float cubeUpTime = 1f;

    public AnimationCurve fogDisapearCurve;
    public float fogTime = 1f;
    public float fogDensity = 0.09f;
    public float fogOutDensity = 0.01f;

    public GameObject platformTriggerPrefab;
    public bool showPath = false;

    public UnityEngine.Events.UnityEvent endEvent;

    private Drone _drone;
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private bool _end = false;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer,(x)=>{
            var player = (PlayerUnit)x.data;
            _drone = player.Drone;
        });

        
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
        GridSet();

        _drone.SetPath(birdyPathName,false,false,true);

        _timeCounter.CreateSequencer("EndProcess");
        _timeCounter.AddSequence("EndProcess",0f,null,(x)=>{
            foreach(var item in _drone.disapearTargets)
            {
                item.SetActive(false);
            }
        });
        _timeCounter.AddSequence("EndProcess",_drone.dissolveTime,(x)=>{
            _drone.UpdateDissolve(_drone.dissolveTime - x);
        },null);
        _timeCounter.AddSequence("EndProcess",cubeUpTime,null,(x)=>{
            CubeUpRing();
        });
        _timeCounter.AddSequence("EndProcess",fogTime,(x)=>{
            
            var time = x / fogTime;
            var factor = fogDisapearCurve.Evaluate(time);
            RenderSettings.fogDensity = Mathf.Lerp(fogDensity,fogOutDensity,factor);
        },null);


        RenderSettings.fogDensity = fogDensity;
        
        StartPattern();
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!active)
        {
            return;
        }

        if(_end)
        {
            if(_timeCounter.ProcessSequencer("EndProcess",deltaTime))
            {
                endEvent?.Invoke();
                enabled = false;
            }
        }
        else
        {
            _drone.FollowPathPerPoint(deltaTime);
        }
        
    }

    public void StartPattern()
    {
        SetBirdyCanMove(false);
        MoveToUp(0);
        
    }

    public void EndPattern()
    {
        _end = true;
    }

    public void CubeUpRing()
    {
        var cubeRing = new List<HexCube>();
        for(int i = 1; i < grid.mapSize; ++i)
        {
            grid.GetCubeRing(ref cubeRing,ringCenter[0].cubePoint,i);
            foreach(var item in cubeRing)
            {
                if(!item.IsActive())
                    item.SetMove(true,(float)(i - 1) * cubeUpTerm,cubeSpeed);
            }

            cubeRing.Clear();
        }
    }

    public void MoveToUp(int target)
    {
        cubePath[target].SetMove(true,platformUpTerm,cubeSpeed);
    }

    public void CenterUp()
    {
        foreach(var item in ringCenter)
        {
            item.SetMove(true,platformUpTerm,cubeSpeed);
        }
    }

    public void DroneFollowNextPoint()
    {
        _drone.SetNextPoint();
    }

    public void GridSet()
    {
        grid.MoveToDownAll();
    }

    public void SetBirdyCanMove(bool value)
    {
        var data = MessageDataPooling.GetMessageData<MD.BoolData>();
        data.value = value;
        SendMessageEx(MessageTitles.playermanager_setDroneCanMove, GetSavedNumber("PlayerManager"), data);
    }

#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        if(!showPath)
            return;

        for(int i = 0; i < cubePath.Count; ++i)
        {
            Handles.Label(cubePath[i].transform.position,i.ToString());
            if(i > 0)
            {
                Handles.DrawLine(cubePath[i - 1].transform.position,cubePath[i].transform.position);
            }
        }
        
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(BirdyBoss_Platform)),CanEditMultipleObjects]
public class BirdyBoss_PlatformEdit : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BirdyBoss_Platform changer = (BirdyBoss_Platform)target;

        if(GUILayout.Button("CreateTriggers"))
        {
            for(int i = 0; i < changer.cubePath.Count; ++i)
            {
                var item = Instantiate(changer.platformTriggerPrefab, changer.cubePath[i].transform.position + Vector3.up,Quaternion.identity);
                item.name = "path : " + i;
                item.transform.SetParent(changer.transform);
            }
            
        }
        
    }
}

#endif