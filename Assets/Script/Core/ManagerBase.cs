using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase : MessageHub<ObjectBase>, IProgress
{
    public override void RegisterReceiver(ObjectBase receiver)
    {
        Debug.Log(receiver.name);
        base.RegisterReceiver(receiver);
    }

    // protected override void Awake()
    // {
    //     base.Awake();
    // }

    public virtual void Assign()
    {
        _unknownMessageProcess = (msg)=>{
            SendMessageEx(msg);
        };

        AddAction(MessageTitles.system_registerRequest,(msg)=>{
            RegisterReceiver((ObjectBase)msg.sender);
        });

        AddAction(MessageTitles.system_withdrawRequest,(msg)=>{
            DeleteReceiver(((ObjectBase)msg.sender).uniqueNumber);
        });
    }

    public virtual void Initialize(){}

    public virtual void Progress(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            if(receiver == null || !receiver.gameObject.activeInHierarchy)
                continue;
            receiver.Progress(deltaTime);
        }
    }

    public virtual void AfterProgress(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            if(receiver == null || !receiver.gameObject.activeInHierarchy)
                continue;
            receiver.AfterProgress(deltaTime);
        }
    }

    public virtual void UpdateTransform()
    {
        foreach(var receiver in _receivers.Values)
        {
            if(receiver == null || !receiver.gameObject.activeInHierarchy)
                continue;
            receiver.UpdateTransform();
        }
    }

    public virtual void Release()
    {
        Dispose();
    }
}
