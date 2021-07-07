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
        ManagersUpdateTransform();
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

    public void ManagersUpdateTransform()
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.UpdateTransform();
        }
    }

    public override void SendMessageProcessing(ManagerBase receiver)
    {
        Message msg = receiver.DequeueSendMessage();
        while(msg != null)
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
                        other.ReceiveMessage(msg);
                        find = true;
                        break;
                    }
                }

                if(!find)
                    _unknownMessageProcess(msg);
            }

            msg = receiver.DequeueSendMessage();
        }
    }

    private void RegisterReceiver(Message msg)
    {
        if(msg.target == uniqueNumber)
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
        if(msg.target == uniqueNumber)
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
