using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1_Sector4_floor : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent endEvent;
    public Material originMat;
    public Material targetMat;

    public HexCube respawnTile;

    public List<HexCube> groundGrid;
    public List<HexCube> entranceGrid;

    public GameObject respawnTrigger;
    public GameObject respawnPoint;


    public int groundDisapearCount = 7;
    public float groundAleartTime = 2f;
    public float groundDisapearTerm = 3f;
    public float groundDisapearTime = 1f;
    public float groundDisapearMoveSpeed = 1f;

    public float entranceMoveSpeed = 1f;

    public float groundMoveSpeed = 0.25f;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private List<HexCube> _cubes = new List<HexCube>();

    private bool _getDown = false;
    private bool _spawn = false;


    public void Start()
    {
        _timeCounter.CreateSequencer("loop");
        _timeCounter.AddSequence("loop",groundAleartTime,null,GroundAleart);
        _timeCounter.AddSequence("loop",groundDisapearTerm - groundAleartTime,null,GroundDisapear);

        _timeCounter.InitSequencer("loop");


        _timeCounter.CreateSequencer("Down");
        _timeCounter.AddSequence("Down",groundDisapearTime + 2,null,GroundDownMove);

        _timeCounter.InitSequencer("Down");

        _timeCounter.InitTimer("spawn",0f,2f);

        _getDown = false;
    }

    public void FixedUpdate()
    {
        if(!_getDown)
        {
            if(_timeCounter.ProcessSequencer("loop",Time.fixedDeltaTime))
            {
                _timeCounter.InitSequencer("loop");
            }
        }
        else
        {
            if(_timeCounter.ProcessSequencer("Down",Time.fixedDeltaTime))
            {
                enabled = false;
            }
        }

        if(_spawn)
        {
            _timeCounter.IncreaseTimerSelf("spawn",out var limit, Time.fixedDeltaTime);
            if(limit)
            {
                _spawn = false;
                respawnTile.special = false;
            }
        }
        
    }

    public void GroundDisapear(float t)
    {
        for(int i = 0; i < _cubes.Count; ++i)
        {
            _cubes[i].GetRenderer().material = originMat;

            _cubes[i].SetMove(false,0f,groundDisapearMoveSpeed,groundDisapearTime);
        }
    }

    public void GroundAleart(float t)
    {
        PickRandomGround();

        for(int i = 0; i < _cubes.Count; ++i)
        {
            _cubes[i].GetRenderer().material = targetMat;
        }
    }

    public void RespawnPlayer()
    {
        _spawn = true;
        _timeCounter.InitTimer("spawn",0f,2f);
        respawnTile.MoveToUp();
        respawnTile.special = true;
    }

    public void PickRandomGround()
    {
        int count = 0;
        int loopLimit = 100;
        _cubes.Clear();

        while(count < groundDisapearCount && loopLimit > 0)
        {
            int random = Random.Range(0,groundGrid.Count);
            if(!groundGrid[random].special && groundGrid[random].IsActive() && (_cubes.Find(x => x == groundGrid[random]) == null))
            {
                _cubes.Add(groundGrid[random]);
                ++count;

            }

            ++loopLimit;
        }
    }

    public void EntranceDownMove()
    {
        //endEvent?.Invoke();
        foreach(var item in entranceGrid)
        {
            item.SetMove(false,0f,entranceMoveSpeed);
        }
    }

    public void GroundDownMove(float t)
    {
        endEvent?.Invoke();
        // respawnTrigger.transform.SetParent(respawnTile.transform);
        // respawnPoint.transform.SetParent(respawnTile.transform);
        
        // foreach(var item in groundGrid)
        // {
        //     item.SetMove(false,0f,groundMoveSpeed);
        // }
    }

    public void GroundDownMove()
    {
        _getDown = true;
        for(int i = 0; i < _cubes.Count; ++i)
        {
            _cubes[i].GetRenderer().material = originMat;
        }
    }

}
