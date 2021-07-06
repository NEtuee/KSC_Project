using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessagePool
{
    private static Queue<Message> _freeQueue = new Queue<Message>();
    
    public static Message CreateNewItem()
    {
        return new Message();
    }

    public static Message GetMessage()
    {
        if(_freeQueue.Count == 0)
            return CreateNewItem();

        return _freeQueue.Dequeue();
    }

    public static void ReturnMessage(Message msg)
    {
        _freeQueue.Enqueue(msg);
    }
}
