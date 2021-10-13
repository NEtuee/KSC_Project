using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1_Sector3_floor : MonoBehaviour
{
    public Material alertMat;
    public Material originMat;

    public float switchTerm = 5f;
    public float alertTime = 4f;
    public float floorMoveSpeed = 1f;
    public List<HexCube> grid;
    private bool _term = false;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        Switch();
        _timeCounter.CreateSequencer("loop");
        _timeCounter.AddSequence("loop",alertTime,null,Alert);
        _timeCounter.AddSequence("loop",switchTerm - alertTime,null,Switch);

        _timeCounter.InitSequencer("loop");
    }

    public void FixedUpdate()
    {
        if(_timeCounter.ProcessSequencer("loop",Time.fixedDeltaTime))
        {
            _timeCounter.InitSequencer("loop");
        }
    }

    public void Alert(float t = 0f)
    {
        foreach(var item in grid)
        {
            bool alert = _term == item.special;

            item.GetRenderer().material = alert ? originMat : alertMat;
        }

    }

    public void Switch(float t = 0f)
    {
        foreach(var item in grid)
        {
            item.SetMove(_term == item.special,0f,1f);
            item.GetRenderer().material = originMat;
        }

        _term = !_term;
    }

}
