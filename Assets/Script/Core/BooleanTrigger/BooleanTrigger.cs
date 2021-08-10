using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MD
{
    public class TriggerData : MessageData
    {
        public string name;
        public bool trigger;
    }
}


[System.Serializable]
public class BooleanTuple
{
    [SerializeField,SerializeReference]
    public string name;
    [SerializeField,SerializeReference]
    public string description;
    [SerializeField,SerializeReference]
    public bool trigger;
}

[CreateAssetMenu(fileName = "BooleanTrigger", menuName = "Options/BooleanTrigger")]
public class BooleanTrigger : ScriptableObject
{
    [SerializeField,SerializeReference]
    public List<BooleanTuple> booleans = new List<BooleanTuple>();

    public void AddTrigger(string name, bool trigger = false)
    {
        if(FindTrigger(name) != null)
        {
            Debug.LogError("Trigger \"" + name + "\" already exists");
            return;
        }

        booleans.Add(new BooleanTuple{name = name, trigger = trigger, description = "trigger : " + name});

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void RemoveTrigger(string name)
    {
        var trigger = FindTrigger(name);
        if(trigger != null)
        {
            RemoveTrigger(trigger);
        }
    }

    public void RemoveTrigger(BooleanTuple trigger)
    {
        booleans.Remove(trigger);
    }

    public BooleanTuple FindTrigger(string name)
    {
        return booleans.Find((x)=>{
            return x.name == name;
        });
    }

    public void SaveDataToFile(string name)
    {
        List<string> dataList = new List<string>();

        for(int i = 0; i < booleans.Count; ++i)
        {
            dataList.Add(booleans[i].name + ":" + (booleans[i].trigger ? "1" : "0"));
        }

        IOControl.WriteStringToFile_NoMark(dataList.ToArray(),name);
    }

    public void LoadDataFromFile(string name, bool addMissing)
    {
        var dataArray = IOControl.ReadStringFromFile(name);

        foreach(var data in dataArray)
        {
            var tuple = DataToTuple(data);
            var trigger = FindTrigger(tuple.name);

            if(trigger == null && addMissing)
            {
                booleans.Add(tuple);
            }
            else if(trigger != null)
            {
                trigger.trigger = tuple.trigger;
            }
        }
    }

    public BooleanTuple DataToTuple(string data)
    {
        var dataArray = data.Split(':');

        return new BooleanTuple{name = dataArray[0], trigger = dataArray[1] == "1"};
    }
}
