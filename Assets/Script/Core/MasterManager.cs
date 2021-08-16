using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MessageHub<ManagerBase>
{
    public static MasterManager instance;

    public List<ManagerBase> managers;

    protected override void Awake()
    {
        instance = this;

        base.Awake();

        _unknownMessageProcess = (msg)=>{
            MessagePool.ReturnMessage(msg);
        };

        AddAction(MessageTitles.system_registerRequest,(msg)=>{
            RegisterReceiver(msg);
        });

        AddAction(MessageTitles.system_withdrawRequest,(msg)=>{
            DeleteReceiver(msg);
        });

        foreach(var m in managers)
        {
            m.Assign();
            RegisterReceiver(m);
        }

        foreach(var m in managers)
        {
            m.Initialize();
        }
    }

    public void Start()
    {
        ReceiveMessageProcessing();
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;

        ManagersUpdate(deltaTime);
        ManagersAfterUpdate(deltaTime);

        ManagersSendMessageProcessing();
        SendMessageProcessing();

        CallReceiveMessageProcessing();
        ManagersReceiveMessageProcessing();

        ReceiveMessageProcessing();
    }

    public void FixedUpdate()
    {
        ManagersFixedUpdate(Time.fixedDeltaTime);
    }

    public void ManagersUpdate(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.Progress(deltaTime);
        }
    }

    public void ManagersAfterUpdate(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.AfterProgress(deltaTime);
        }
    }

    public void ManagersFixedUpdate(float deltaTime)
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.FixedProgress(deltaTime);
        }
    }

    public void ManagersSendMessageProcessing()
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.SendMessageProcessing();
        }
    }

    public void ManagersReceiveMessageProcessing()
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.CallReceiveMessageProcessing();
        }
    }

    // public override void SendMessageProcessing(ManagerBase receiver)
    // {
    //     Message msg = receiver.DequeueSendMessage();
    //     while(msg != null)
    //     {
    //         HandleMessage(msg);

    //         msg = receiver.DequeueSendMessage();
    //     }
    // }

    public void HandleMessageQuick(Message msg)
    {
        if(IsInReceivers(msg.target) && _receivers[msg.target].CanHandleMessage(msg))
        {
#if UNITY_EDITOR
            _receivers[msg.target].Debug_AddReceivedQueue(msg);
#endif
            _receivers[msg.target].MessageProcessing(msg);
            MessagePool.ReturnMessage(msg);
        }
        else if((msg.target == 0 || uniqueNumber == msg.target) && CanHandleMessage(msg))
        {
#if UNITY_EDITOR
            Debug_AddReceivedQueue(msg);
#endif
            MessageProcessing(msg);
            MessagePool.ReturnMessage(msg);
        }
        else
        {
            bool find = false;
            foreach(var other in _receivers.Values)
            {            
                if(other.IsInReceivers(msg.target) && other.GetReciever(msg.target).CanHandleMessage(msg))
                {
#if UNITY_EDITOR
                    other.GetReciever(msg.target).Debug_AddReceivedQueue(msg);
#endif
                    other.GetReciever(msg.target).MessageProcessing(msg);
                    MessagePool.ReturnMessage(msg);
                    find = true;
                    break;
                }
            }

            if(!find)
                _unknownMessageProcess(msg);
        }
    }

    public override void HandleMessage(Message msg)
    {
        if(IsInReceivers(msg.target))
        {
            _receivers[msg.target].ReceiveMessage(msg);
        }
        else if(msg.target == 0 || uniqueNumber == msg.target)
        {
            ReceiveMessage(msg);
        }
        else
        {
            bool find = false;
            foreach(var other in _receivers.Values)
            {
                if(other.IsInReceivers(msg.target))
                {
                    other.GetReciever(msg.target).ReceiveMessage(msg);
                    find = true;
                    break;
                }
            }

            if(!find)
                _unknownMessageProcess(msg);
        }
    }

    private void RegisterReceiver(Message msg)
    {
        if(msg.target == uniqueNumber || msg.target == 0)
        {
            var manager = (ManagerBase)msg.sender;
            manager.Assign();
            RegisterReceiver(manager);
        }
        else if(IsInReceivers(msg.target))
        {
            _receivers[msg.target].RegisterReceiver((ObjectBase)msg.sender);
        }
    }

    private void DeleteReceiver(Message msg)
    {
        if(msg.target == uniqueNumber || msg.target == 0)
        {
            var manager = (int)msg.data;
            DeleteReceiver(manager);
        }
        else if(IsInReceivers(msg.target))
        {
            var target = (int)msg.data;
            _receivers[msg.target].DeleteReceiver(target);
        }
    }
}
