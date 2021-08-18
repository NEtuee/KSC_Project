using UnityEngine;
using System.Collections.Generic;

public class UniqueNumberBase : MonoBehaviour
{
    public int uniqueNumber{get{return _uniqueNumber;}}
    private int _uniqueNumber = 0;
    private static int _numberOrder = 1;
    private static Dictionary<string, int> _numberStorage;

    protected virtual void Awake()
    {
        SetUniqueNumber();
    }

    private void SetUniqueNumber()
    {
        if(_uniqueNumber == 0)
            _uniqueNumber = _numberOrder++;
    }

    public static int GetSavedNumberStatic(string key)
    {
        if(!_numberStorage.ContainsKey(key))
        {
            Debug.Log("key dose not exists : " + key);
            return -1;
        }

        return _numberStorage[key];
    }

    protected int GetSavedNumber(string key)
    {
        if(!_numberStorage.ContainsKey(key))
        {
            Debug.Log("key dose not exists : " + key);
            return -1;
        }

        return _numberStorage[key];
    }

    protected void SaveMyNumber(string key, bool overWrite = false)
    {
        if(_numberStorage == null)
            _numberStorage = new Dictionary<string, int>();

        SetUniqueNumber();

        if(_numberStorage.ContainsKey(key))
        {
            if(overWrite)
            {
                _numberStorage[key] = uniqueNumber;
            }
            else
            {
                Debug.Log("key already exsist");
            }

        }
        else
        {
            _numberStorage.Add(key,_uniqueNumber);
        }
    }

    public static void InitUniqueNumber()
    {
        _numberOrder = 1;
        _numberStorage?.Clear();
    }
}
