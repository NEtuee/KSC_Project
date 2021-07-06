using System.Collections.Generic;
using System;

public abstract class MessageReceiver : UniqueNumberBase
{
    private Dictionary<short, Action<Message>> _msgProcActions = new Dictionary<short, Action<Message>>();
    private Queue<Message> _sendQueue = new Queue<Message>();
    private Queue<Message> _receiveQueue = new Queue<Message>();

    protected Object _recentlySender;

    public void ReceiveMessage(Message msg)
    {
        if(!_msgProcActions.ContainsKey(msg.title))
            return;

        _recentlySender = msg.sender;
        _receiveQueue.Enqueue(msg);
    }

    public void MessageProcessing()
    {
        foreach(var msg in _receiveQueue)
        {
            MessageProcessing(msg);

            MessagePool.ReturnMessage(msg);
        }

        _receiveQueue.Clear();
    }

    public void MessageProcessing(Message msg)
    {
        _msgProcActions[msg.title](msg);
    }

    public virtual void Dispose()
    {
        foreach(var msg in _receiveQueue)
        {
            MessagePool.ReturnMessage(msg);
        }

        foreach(var msg in _sendQueue)
        {
            MessagePool.ReturnMessage(msg);
        }

        _recentlySender = null;
    }

    public Message DequeueSendMessage()
    {
        return _sendQueue.Count == 0 ? null : _sendQueue.Dequeue();
    }

    protected void AddAction(short title, Action<Message> action)
    {
        if(_msgProcActions.ContainsKey(title))
            return;

        _msgProcActions.Add(title,action);
    }

    protected void SendMessageEx(Message msg)
    {
        _sendQueue.Enqueue(msg);
    }

    protected void SendMessageEx(short title, int target, Object data)
    {
        var msg = MessagePool.GetMessage();
        msg.Set(title,target,data,(Object)this);

        _sendQueue.Enqueue(msg);
    }

    protected void SendMessageEx(MessageReceiver receiver, Message msg)
    {
        receiver.ReceiveMessage(msg);
    }

    protected void SendMessageEx(MessageReceiver receiver, short title, Object data)
    {
        var msg = MessagePool.GetMessage();
        msg.Set(title,receiver.uniqueNumber,data,(Object)this);

        receiver.ReceiveMessage(msg);
    }

    protected void SendBroadcastMessage()
    {
        //..
    }


}
