using System.Collections.Generic;
using System;

public abstract class MessageReceiver : UniqueNumberBase
{
    private Dictionary<ushort, Action<Message>> _msgProcActions = new Dictionary<ushort, Action<Message>>();
    private Queue<Message> _sendQueue = new Queue<Message>();
    private Queue<Message> _receiveQueue = new Queue<Message>();

    protected Object _recentlySender;

    protected const int boradcastNumber = -10;
    protected const int boradcastWithoutSenderNumber = -11;


    public void ReceiveMessage(Message msg)
    {
#if UNITY_EDITOR
        Debug_AddReceivedQueue(msg);
#endif
        if(!CanHandleMessage(msg))
        {
            MessagePool.ReturnMessage(msg);
            return;
        }
        _recentlySender = msg.sender;
        _receiveQueue.Enqueue(msg);
    }

    public virtual void ReceiveAndProcessMessage(Message msg)
    {
#if UNITY_EDITOR
        Debug_AddReceivedQueue(msg);
#endif
        if(!CanHandleMessage(msg))
        {
            MessagePool.ReturnMessage(msg);
            return;
        }
        _recentlySender = msg.sender;
        MessageProcessing(msg);
        MessagePool.ReturnMessage(msg);
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

    public virtual bool CanHandleMessage(Message msg)
    {
        return _msgProcActions.ContainsKey(msg.title);
    }

    public virtual void MessageProcessing(Message msg)
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

    public void SendMessageQuick(MessageReceiver receiver,Message msg)
    {
        receiver.ReceiveAndProcessMessage(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageQuick(MessageReceiver receiver,ushort title, Object data)
    {
        var msg = MessagePack(title,receiver.uniqueNumber,data);
        receiver.ReceiveAndProcessMessage(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageEx(Message msg)
    {
        _sendQueue.Enqueue(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageEx(ushort title, int target, Object data)
    {
        var msg = MessagePack(title,target,data);
        _sendQueue.Enqueue(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageEx(MessageReceiver receiver, Message msg)
    {
        receiver.ReceiveMessage(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageEx(MessageReceiver receiver, ushort title, Object data)
    {
        var msg = MessagePack(title,receiver.uniqueNumber,data);
        receiver.ReceiveMessage(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendBroadcastMessage(ushort title, Object data, bool withoutSender)
    {
        var msg = MessagePack(title, withoutSender ? boradcastWithoutSenderNumber : boradcastNumber, data);

        _sendQueue.Enqueue(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    protected Message MessagePack(Message msg)
    {
        var target = MessagePool.GetMessage();
        target.Set(msg.title,msg.target,msg.data,msg.sender);
        return target;
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

    public List<string> debugExceptionTitles = new List<string>();

    [UnityEngine.HideInInspector] public int sendedCount = 0;
    [UnityEngine.HideInInspector] public int receivedCount = 0;


    private int _debugCount = 6;

    public void Debug_AddSendedQueue(Message msg)
    {
        if(!allowDebugMode)
            return;

        if(debugExceptionTitles.Count != 0)
        {
            string title = msg.title.ToString("X4");
            if(debugExceptionTitles.Exists((x)=>{return x == title;}))
            {
                return;
            }
        }

        sendedQueue.Enqueue(Debug_CopyMessage(msg,++sendedCount));
        if(sendedQueue.Count > _debugCount)
        {
            DebugMessagePool.ReturnMessage(sendedQueue.Dequeue());
        }
    }

    public void Debug_AddReceivedQueue(Message msg)
    {
        if(!allowDebugMode)
            return;
        
        if(debugExceptionTitles.Count != 0)
        {
            string title = msg.title.ToString("X4");
            if(debugExceptionTitles.Exists((x)=>{return x == title;}))
            {
                return;
            }
        }

        receivedQueue.Enqueue(Debug_CopyMessage(msg,++receivedCount));
        if(receivedQueue.Count > _debugCount)
        {
            DebugMessagePool.ReturnMessage(receivedQueue.Dequeue());
        }
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

    public DebugMessage Debug_CopyMessage(Message msg, int count)
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
        
        target.gameObject = null;

        if(msg.sender != null)
        {
            if(((MessageReceiver)msg.sender) != null)
                target.gameObject = ((MessageReceiver)msg.sender).gameObject;
        }

        target.Set(msg.title,msg.target,data,senderCode,senderName,count);

        return target;
    }

#endif
}
