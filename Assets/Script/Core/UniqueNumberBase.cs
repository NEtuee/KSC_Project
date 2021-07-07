using UnityEngine;
using System.Collections.Generic;

public class UniqueNumberBase : MonoBehaviour
{
    public int uniqueNumber{get{return _uniqueNumber;}}
    private int _uniqueNumber;
    private static int _numberOrder = 1;
    private static Dictionary<string, int> _numberStorage;

    protected virtual void Awake()
    {
        SetUniqueNumber();
    }

    private void SetUniqueNumber()
    {
        _uniqueNumber = _numberOrder++;
    }

    protected int GetSavedNumber(string key)
    {
        if(!_numberStorage.ContainsKey(key))
        {
            Debug.Log("key dose not exists");
            return -1;
        }

        return _numberStorage[key];
    }

    protected void SaveMyNumber(string key)
    {
        if(_numberStorage == null)
            _numberStorage = new Dictionary<string, int>();

        if(_numberStorage.ContainsKey(key))
        {
            Debug.Log("key already exsist");
            return;
        }

        _numberStorage.Add(key,_uniqueNumber);
    }
}
