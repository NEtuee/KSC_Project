using System.Collections.Generic;
using System;

public abstract class MessageReceiver : UniqueNumberBase
{
    private Dictionary<ushort, Action<Message>> _msgProcActions = new Dictionary<ushort, Action<Message>>();
    private Queue<Message> _sendQueue = new Queue<Message>();
    private Queue<Message> _receiveQueue = new Queue<Message>();

    protected Object _recentlySender;

    public void ReceiveMessage(Message msg)
    {
#if UNITY_EDITOR
        Debug_AddReceivedQueue(msg);
#endif
        if(!_msgProcActions.ContainsKey(msg.title))
        {
            MessagePool.ReturnMessage(msg);
            return;
        }
        _recentlySender = msg.sender;
        _receiveQueue.Enqueue(msg);
    }

    public void ReceiveMessageProcessing()
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

    public Message DequeueReceiveMessage()
    {
        return _receiveQueue.Count == 0 ? null : _receiveQueue.Dequeue();
    }

    protected void AddAction(ushort title, Action<Message> action)
    {
        if(_msgProcActions.ContainsKey(title))
            return;

        _msgProcActions.Add(title,action);
    }

    protected void SendMessageEx(Message msg)
    {
        _sendQueue.Enqueue(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    protected void SendMessageEx(ushort title, int target, Object data)
    {
        var msg = MessagePack(title,target,data);
        _sendQueue.Enqueue(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    protected void SendMessageEx(MessageReceiver receiver, Message msg)
    {
        receiver.ReceiveMessage(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    protected void SendMessageEx(MessageReceiver receiver, ushort title, Object data)
    {
        var msg = MessagePack(title,receiver.uniqueNumber,data);
        receiver.ReceiveMessage(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    protected void SendBroadcastMessage()
    {
        //..
    }

    protected Message MessagePack(ushort title, int target, Object data)
    {
        var msg = MessagePool.GetMessage();
        msg.Set(title,target,data,(Object)this);

        return msg;
    }

#if UNITY_EDITOR

    public Queue<DebugMessage> sendedQueue = new Queue<DebugMessage>();
    public Queue<DebugMessage> receivedQueue = new Queue<DebugMessage>();

    public bool allowDebugMode = false;

    public void Debug_AddSendedQueue(Message msg)
    {
        if(!allowDebugMode)
            return;

        sendedQueue.Enqueue(Debug_CopyMessage(msg));
    }

    public void Debug_AddReceivedQueue(Message msg)
    {
        if(!allowDebugMode)
            return;
            
        receivedQueue.Enqueue(Debug_CopyMessage(msg));
    }

    public void Debug_ClearQueue()
    {
        foreach(var msg in sendedQueue)
        {
            DebugMessagePool.ReturnMessage(msg);

        }

        foreach(var msg in receivedQueue)
        {
            DebugMessagePool.ReturnMessage(msg);
            
        }

        sendedQueue.Clear();
        receivedQueue.Clear();
    }

    public DebugMessage Debug_CopyMessage(Message msg)
    {
        var target = DebugMessagePool.GetMessage();

        bool data = msg.data != null;
        int senderCode = -1;
        string senderName = "null";

        if(msg.sender != null)
        {
            var recv = (MessageReceiver)msg.sender;
            if(recv != null)
            {
                senderCode = recv.uniqueNumber;
                senderName = recv.name;
            }
        }
        


        target.Set(msg.title,msg.target,data,senderCode,senderName);

        return target;
    }

#endif
}
