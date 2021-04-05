using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImmortalJirungE_master : MonoBehaviour
{
    public List<ImmortalJirungE_AI> aIs;
    public BezierLightning lightning;

    public UnityEvent whenAllShieldDestroy;

    private int shieldCount = 0;

    private TimeCounterEx _timeCounterEx = new TimeCounterEx();

    public void Start()
    {
        Recovery();

        _timeCounterEx.InitTimer("time",0f,Random.Range(1f,2f));
    }

    public void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        _timeCounterEx.IncreaseTimer("time",out var limit);
        if(limit)
        {
            for(int i = 1; i < aIs.Count + 1; ++i)
            {
                lightning.Active(aIs[i - 1].transform.position,aIs[i >= aIs.Count ? 0 : i].transform.position,3,0.1f,4f);
            }

            _timeCounterEx.InitTimer("time",0f,Random.Range(1f,2f));
            
        }

        bool whip = false;
        foreach(var jirung in aIs)
        {
            if(jirung.currentState == ImmortalJirungE_AI.State.FloorWhip)
            {
                whip = true;
                break;
            }
        }

        if(!whip)
        {
            foreach(var jirung in aIs)
            {
                if(jirung.canFloorWhip && jirung.currentState == ImmortalJirungE_AI.State.WallMove)
                {
                    jirung.ChangeState(ImmortalJirungE_AI.State.FloorWhip);
                    break;
                }
            }
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

            whenAllShieldDestroy?.Invoke();
        }
    }
}
