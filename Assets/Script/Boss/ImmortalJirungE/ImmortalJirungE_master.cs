using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalJirungE_master : MonoBehaviour
{
    public List<ImmortalJirungE_AI> aIs;
    public BezierLightning lightning;

    private int shieldCount = 0;

    private TimeCounterEx _timeCounterEx = new TimeCounterEx();

    public void Start()
    {
        Recovery();

        _timeCounterEx.InitTimer("time",0f,Random.Range(1f,2f));
    }

    public void Update()
    {
        _timeCounterEx.IncreaseTimer("time",out var limit);
        if(limit)
        {
            for(int i = 1; i < aIs.Count + 1; ++i)
            {
                lightning.Active(aIs[i - 1].transform.position,aIs[i >= aIs.Count ? 0 : i].transform.position,2,0.1f,0.4f);
            }

            _timeCounterEx.InitTimer("time",0f,Random.Range(1f,2f));
            
        }
    }

    public void Recovery()
    {
        shieldCount = aIs.Count;
    }

    public void DecreaseShieldCount()
    {
        --shieldCount;
        if(shieldCount == 0)
        {
            foreach(var ai in aIs)
            {
                ai.ChangeState(ImmortalJirungE_AI.State.Stun);

                Recovery();
            }
        }
    }
}
