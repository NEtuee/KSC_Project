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

    public void DeleteReceiver(int target)
    {
        if(_receivers.ContainsKey(target))
            _receivers.Remove(target);
    }

    public void DeleteReceiver(MessageReceiver target)
    {
        if(_receivers.ContainsKey(target.uniqueNumber))
            _receivers.Remove(target.uniqueNumber);
    }

    public void MessageSendProcessing()
    {
        foreach(var receiver in _receivers.Values)
        {
            MessageSendProcessing(receiver);
        }
    }

    public void MessageSendProcessing(MessageReceiver receiver)
    {
        Message msg = receiver.DequeueSendMessage();
        while(msg != null)
        {
            if(IsInReceivers(msg.target))
            {
                _receivers[msg.target].ReceiveMessage(msg);
            }
            else if(msg.target == 0)
            {
                ReceiveMessage(msg);
            }
            else
            {
                _unknownMessageProcess(msg);
            }

            msg = receiver.DequeueSendMessage();
        }
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
}
