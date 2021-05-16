using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImmortalJirungE_V2_master : MonoBehaviour
{
    public List<ImmortalJirungE_V2_AI> aIs;
    public BezierLightning lightning;
    public Transform lightningPoint;

    public UnityEvent whenAllShieldDestroy;

    private int shieldCount = 0;

    private TimeCounterEx _timeCounterEx = new TimeCounterEx();

    public void Start()
    {
        Recovery();

        _timeCounterEx.InitTimer("time",0f,Random.Range(1f,2f));
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        _timeCounterEx.IncreaseTimer("time",out var limit);
        if(limit)
        {
            for(int i = 0; i < aIs.Count + 0; ++i)
            {
                if(!aIs[i].shield.isOver)
                    lightning.Active(aIs[i].transform,lightningPoint,3,0.1f,4f,0.03f);
            }

            _timeCounterEx.InitTimer("time",0f,Random.Range(0.1f,0.15f));
            
        }

        bool whip = false;
        foreach(var jirung in aIs)
        {
            if(jirung.currentState == ImmortalJirungE_V2_AI.State.FloorWhip)
            {
                whip = true;
                break;
            }
        }

        if(!whip)
        {
            foreach(var jirung in aIs)
            {
                if(jirung.canFloorWhip && jirung.currentState == ImmortalJirungE_V2_AI.State.WallMove)
                {
                    jirung.ChangeState(ImmortalJirungE_V2_AI.State.FloorWhip);
                    break;
                }
            }
        }

        
    }

    public void Launch()
    {
        foreach(var jirung in aIs)
        {
            jirung.ChangeState(ImmortalJirungE_V2_AI.State.Launch);
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
                ai.ChangeState(ImmortalJirungE_V2_AI.State.Stun);

                Recovery();
            }

            whenAllShieldDestroy?.Invoke();
        }
    }
}
