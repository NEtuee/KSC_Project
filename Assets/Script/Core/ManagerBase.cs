using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase<T> : MessageHub<T>, IProgress where T : ObjectBase
{
    public override void RegisterReceiver(T receiver)
    {
        receiver.Initialize();
        base.RegisterReceiver(receiver);
    }

    protected override void Awake()
    {
        base.Awake();
        Assign();
    }

    public virtual void Assign()
    {
        _unknownMessageProcess = (msg)=>{
            SendMessageEx(msg);
        };

        AddAction(MessageTitles.system_registerRequest,(msg)=>{
            RegisterReceiver((T)msg.sender);
        });

    }

    public virtual void Initialize(){}

    public virtual void Progress(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.Progress(deltaTime);
        }
    }

    public virtual void AfterProgress(float deltaTime)
    {
        MessageSendProcessing();
    }

    public virtual void Release()
    {
        Dispose();
    }
}
