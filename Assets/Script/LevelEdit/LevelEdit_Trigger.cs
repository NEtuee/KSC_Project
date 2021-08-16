using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEdit_Trigger : UnTransfromObjectBase
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
    [SerializeField]private UnityEvent triggerExitEvent;
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

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        _timeCounter.InitTimer("time");
 
    }

    public override void Progress(float deltaTime)
    {
        _timeCounter.IncreaseTimer("time",1f,out _timeOut);

        if(isTriggered && triggerTimer != 0f)
        {
            triggerTimer -= deltaTime;

            if(triggerTimer < 0f)
            {
                isTriggered = false;
            }
        }
        
        if(isTriggered && !afterEvent && afterEventProgress)
        {
            afterTriggerTimer -= deltaTime;
            if(afterTriggerTimer < 0f)
            {
                afterEvent = true;
                afterTriggerEvent.Invoke();
            }
        }   
    }

    public bool IsTriggered() {return isTriggered;}
    public void SetCollisionTrigger(bool active) { collisionTrigger = active; }

    public void OnTriggerEnter(Collider coll)
    {

        if (isTriggered || !collisionTrigger || !_timeOut)
            return;

        // Debug.Log(LayerMask.LayerToName(coll.gameObject.layer));
        // Debug.Log(LayerMask.LayerToName(targetLayer));
       
        if (targetLayer == (targetLayer | (1<<coll.gameObject.layer)))
        {
            Debug.Log(gameObject.name);
            gameObject.name = "triggered";
            TriggerEnable();
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if ((coll.gameObject.layer & targetLayer.value) != 0)
        {
            triggerExitEvent.Invoke();
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
