using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEx
{
    private Dictionary<string, bool> _triggerSet = new Dictionary<string, bool>();

    public bool SetTrigger(string target, bool value)
    {
        bool isChanged = false;
        if(_triggerSet.ContainsKey(target))
        {
            isChanged = _triggerSet[target] != value;
        }

        _triggerSet[target] = value;

        return isChanged;
    }

    public bool GetTrigger(string target, bool init = false)
    {
        if(_triggerSet.ContainsKey(target))
            return _triggerSet[target];
        else
        {
            SetTrigger(target,init);
            return init;
        }
    }
}
