using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageData
{
    public bool isUsing = false;
}

public class MessageDataPool 
{
    public List<MessageData> _poolList = new List<MessageData>();
    public int _count = 0;

    public MessageData Get()
    {
        if(_count == _poolList.Count)
            _count = 0;

        if(_poolList[_count].isUsing == true)
        {
            _poolList.Add(new MessageData());
            _count = 0;
            return _poolList[_poolList.Count - 1];
        }
        else
        {
            _poolList[_count].isUsing = true;
            return _poolList[_count++];
        }
    }
}

public static class MessageDataPooling
{
    private static Dictionary<System.Type, MessageDataPool> _messageDataDic = new Dictionary<System.Type, MessageDataPool>();
    private const int initalizeDataNum = 3;

    public static void RegisterMessageData<T>() where T : MessageData,new()
    {
        System.Type type = typeof(T);

        if(_messageDataDic.ContainsKey(type) == false)
        {
            _messageDataDic.Add(type, new MessageDataPool());

            for(int i =0; i<initalizeDataNum;i++)
            {
                _messageDataDic[type]._poolList.Add(new T());
            }
        }
    }

    public static T GetMessageData<T>() where T : MessageData, new()
    {
        System.Type type = typeof(T);

        if(_messageDataDic.ContainsKey(type) == false)
        {
            RegisterMessageData<T>();
        }

        return _messageDataDic[type].Get() as T;
    }

    public static T CastData<T>(object data) where T : MessageData
    {
        T realData = (T)data;
        realData.isUsing = false;
        return realData;
    }
}
