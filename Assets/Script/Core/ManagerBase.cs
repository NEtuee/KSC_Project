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

    public void SendMessageQuick(Message msg)
    {
        MasterManager.instance.HandleMessageQuick(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageQuick(ushort title, int target, Object data)
    {
        var msg = MessagePack(title,target,data);
        MasterManager.instance.HandleMessageQuick(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

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
            if(receiver == null || !receiver.gameObject.activeInHierarchy || !receiver.enabled)
            {
                continue;
            }
            receiver.Progress(deltaTime);
        }
    }

    public virtual void AfterProgress(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            if(receiver == null || !receiver.gameObject.activeInHierarchy || !receiver.enabled)
                continue;
            receiver.AfterProgress(deltaTime);
        }
    }

    public virtual void UpdateTransform()
    {
        foreach(var receiver in _receivers.Values)
        {
            if(receiver == null || !receiver.gameObject.activeInHierarchy || !receiver.enabled)
                continue;
            receiver.UpdateTransform();
        }
    }

    public virtual void Release()
    {
        WithdrawRequest();
        Debug.Log(name);

#if UNITY_EDITOR
        Debug_ClearQueue();
#endif
    }

    public void RegisterRequest()
    {
        var msg = MessagePack(MessageTitles.system_registerRequest,0,null);
        MasterManager.instance.HandleMessageQuick(msg);

#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void WithdrawRequest()
    {
        var msg = MessagePack(MessageTitles.system_withdrawRequest,0,uniqueNumber);
        MasterManager.instance.ReceiveMessage(msg);

#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }


    public override void Dispose()
    {
        base.Dispose();
        Release();
    }

    protected virtual void OnDestroy()
    {
        Dispose();
    }
}
