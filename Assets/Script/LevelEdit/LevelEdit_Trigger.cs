using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEdit_Trigger : MonoBehaviour
{
    public enum TriggerType
    {
        Immediate,
        WhenBossStepEnd,
    }

    public delegate void TriggerEventDelegate();
    public TriggerEventDelegate triggerEventDelegate = ()=>{};

    [SerializeField]private TriggerType triggerType;
    [SerializeField]private UnityEvent triggerEvent;
    [SerializeField]private UnityEvent afterTriggerEvent = new UnityEvent();
    [SerializeField]private LayerMask targetLayer;
    [SerializeField]private bool isTriggered = false;
    [SerializeField]private bool collisionTrigger = true;
    [SerializeField]private float reloadTime = 0f;
    [SerializeField]private float afterTriggerTime = 0f;

    private float triggerTimer = 0f;
    private float afterTriggerTimer = 0f;

    private bool afterEvent = false;
    private bool afterEventProgress = false;
    private bool _timeOut = false;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public void Start()
    {
        _timeCounter.InitTimer("time");

        Debug.Log("??");
    }

    public void Update()
    {
        _timeCounter.IncreaseTimer("time",1f,out _timeOut);

        if(isTriggered && triggerTimer != 0f)
        {
            triggerTimer -= Time.deltaTime;

            if(triggerTimer < 0f)
            {
                isTriggered = false;
            }
        }
        
        if(isTriggered && !afterEvent && afterEventProgress)
        {
            afterTriggerTimer -= Time.deltaTime;
            if(afterTriggerTimer < 0f)
            {
                afterEvent = true;
                afterTriggerEvent.Invoke();
            }
        }   
    }

    public bool IsTriggered() {return isTriggered;}

    public void OnTriggerEnter(Collider coll)
    {
        if(isTriggered || !collisionTrigger || !_timeOut)
            return;

        if((coll.gameObject.layer & targetLayer.value) != 0)
        {
            Debug.Log(gameObject.name);
            gameObject.name = "triggered";
            TriggerEnable();
        }
    }

    public void TriggerEnable()
    {
        isTriggered = true;

        triggerEventDelegate();
        triggerTimer = reloadTime;

        if(triggerType == TriggerType.WhenBossStepEnd)
        {
            LevelEdit_Controll.instance.AddActiveTrigger(this);
            return;
        }
        else
        {
            TriggerEventInvoke();
        }
    }

    public void TriggerEventInvoke()
    {
        afterTriggerTimer = afterTriggerTime;
        afterEventProgress = true;
        triggerEvent.Invoke();
    }
}
