using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable]
public class StateMachineGraph : FunctionGraph
{
    [System.Serializable]
    public class StateInfo
    {
        public string name;
        public int uniqueID;

        [SerializeField,SerializeReference]
        public FunctionGraph.FunctionInfo stateInitialize;

        [SerializeField,SerializeReference]
        public FunctionGraph.FunctionInfo stateProgress;

        public Action<string> onNameChanged;

        public void UpdateNodeTitle()
        {
            stateInitialize.name = name + " Initialize";
            stateProgress.name = name + " Progress";
            stateInitialize.UpdateNodeTitle();
            stateProgress.UpdateNodeTitle();

            onNameChanged?.Invoke(name);
        }

    }

    [SerializeField,SerializeReference]
    public List<StateInfo> states;

    [SerializeField,SerializeReference]
    public int stateID = 0;

    [SerializeField,SerializeReference]
    public int initializeState = 0;

    [SerializeField,SerializeReference]
    public int currentState = 0;

    public event Action onStateListChanged;
    public event Action onStateNameChanged;

    public StateInfo FindState(int id) {return states.Find((x)=>{return x.uniqueID == id;});}
    public StateInfo FindState(string name) {return states.Find((x)=>{return x.name == name;});}
    public int FindStateIndex(int id) 
    {
        for(int i = 0; i < states.Count; ++i)
        {
            if(states[i].uniqueID == id)
                return i;
        }

        return -1;
    }

    public List<string> GetStateNames()
    {
        List<string> names = new List<string>();
        foreach(var name in states)
        {
            names.Add(name.name);
        }

        return names;
    }

    public void ChangeState(int code)
    {
        currentState = code;
    }

    public void AddState(StateInfo info)
    {
        states.Add(info);
        onStateListChanged?.Invoke();
    }

    public void RemoveState(StateInfo info)
    {
        states.Remove(info);
        onStateListChanged?.Invoke();
    }

    public void UpdateStateName(StateInfo state, string name)
    {
        state.name = name;
        state.UpdateNodeTitle();

        onStateNameChanged?.Invoke();
    }
}
