using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    public virtual string stateIdentifier => "";

    [HideInInspector]public UnityEngine.Object targetObject;
    [HideInInspector]public StateProcessor processor;

    protected TimeCounterEx _timeCounter = new TimeCounterEx();

    public virtual void Assign(){}
    public virtual void StateInitialize(StateBase prevState){}
    public virtual void StateProgress(float deltaTime){}
    public virtual void StateChanged(StateBase targetState){}

    public void StateChange(string key)
    {
        processor.StateChange(key);
    }
}
