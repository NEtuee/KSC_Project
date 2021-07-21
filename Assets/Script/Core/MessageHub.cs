using System.Collections.Generic;
using System;

public abstract class MessageHub<T> : MessageReceiver where T : MessageReceiver
{
    protected Dictionary<int, T> _receivers = new Dictionary<int, T>();
    protected Action<Message> _unknownMessageProcess;

    protected override void Awake()
    {
        base.Awake();
    }

    public virtual void RegisterReceiver(T receiver)
    {
        _receivers.Add(receiver.uniqueNumber, receiver);
    }

    public virtual void DeleteReceiver(int target)
    {
        if(_receivers.ContainsKey(target))
            _receivers.Remove(target);
    }

    public void DeleteReceiver(MessageReceiver target)
    {
        if(_receivers.ContainsKey(target.uniqueNumber))
            _receivers.Remove(target.uniqueNumber);
    }

    public void CallReceiveMessageProcessing()
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.ReceiveMessageProcessing();
        }
    }

    public void SendMessageProcessing()
    {
        foreach(var receiver in _receivers.Values)
        {
            SendMessageProcessing(receiver);
        }
    }

    public virtual void SendMessageProcessing(T receiver)
    {
        Message msg = receiver.DequeueSendMessage();
        while(msg != null)
        {
            HandleMessage(msg);

            msg = receiver.DequeueSendMessage();
        }
    }

    public virtual void HandleMessage(Message msg)
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
            _unknownMessageProcess(msg);
        }
    }

    public T GetReciever(int number)
    {
        return _receivers[number];
    }

    public bool IsInReceivers(int number)
    {
        return _receivers.ContainsKey(number);
    }

    public override void Dispose()
    {
        foreach(var receiver in _receivers.Values)
        {
            receiver.Dispose();
        }

        _receivers.Clear();

        base.Dispose();
    }

#if UNITY_EDITOR

    public Dictionary<int, T> Debug_GetReceivers()
    {
        return _receivers;
    }

#endif
}
