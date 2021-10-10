using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class A1_Sector2_floor : MonoBehaviour
{
    public class LineItem
    {
        public float floorMoveSpeed = 1f;
        public float floorDisapearTime = 1f;
        public HexCubeGrid grid;
        int startKey;
        int count = 0;
        int lineStart = 0;
        int key = 0;

        public LineItem(float floorMoveSpeed, float floorDisapearTime, int start,HexCubeGrid grid)
        {
            this.floorMoveSpeed = floorMoveSpeed;
            this.floorDisapearTime = floorDisapearTime;
            startKey = start;
            lineStart = start;
            this.grid = grid;
            

            Init();
        }

        public void Init()
        {
            count = 0;
            lineStart = startKey;
            key = startKey;
        }

        public void SetUpdown()
        {
            HexCube cube = grid.GetCube(key);

            if(cube == null)
            {
                Init();
                cube = grid.GetCube(key);
            }

            while(cube != null)
            {
                cube.SetMove(false,0f,floorMoveSpeed,floorDisapearTime);
                key -= (46 + count % 2);
                count += 1;

                cube = grid.GetCube(key);
                
            }

            --lineStart;
            count = 0;
            key = lineStart;
        }
    };

    public float lineDisapearTerm = 0.3f;
    public float floorMoveSpeed = 1f;
    public float floorDisapearTime = 1f;

    public int startKey;
    public int loopCount;
    public float loopTerm;
    public HexCubeGrid grid;

    private List<LineItem> _lines = new List<LineItem>();
    private List<TimeCounterEx.SequenceProcessor> _sequencers = new List<TimeCounterEx.SequenceProcessor>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private int _loopCount;

    public void Start()
    {
        for(int i = 0; i < loopCount; ++i)
        {
            var line = new LineItem(floorMoveSpeed,floorDisapearTime,startKey,grid);
            var sequencer = new TimeCounterEx.SequenceProcessor();
            sequencer.AddSequence(0f,null,(x)=>{
                line.SetUpdown();
            });
            sequencer.AddSequence(lineDisapearTerm,null,null);

            _lines.Add(line);
            _sequencers.Add(sequencer);
        }

        _loopCount = 1;
        _timeCounter.InitTimer("term",0f,loopTerm);
    }



    public void FixedUpdate()
    {
        if(_loopCount < loopCount)
        {
            _timeCounter.IncreaseTimerSelf("term",out var limit,Time.fixedDeltaTime);
            if(limit)
            {
                _timeCounter.InitTimer("term",0f,loopTerm);
                ++_loopCount;
            }
        }
        for(int i = 0; i < _loopCount; ++i)
        {
            if(_sequencers[i].Process(Time.fixedDeltaTime))
            {
                _sequencers[i].InitSequencer();
            }
        }

    }
}
