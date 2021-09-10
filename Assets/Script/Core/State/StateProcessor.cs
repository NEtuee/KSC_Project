using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateProcessor : MonoBehaviour
{
    public List<StateBase> states = new List<StateBase>();

    public string currentState;

    private Dictionary<string, StateBase> _stateMap = new Dictionary<string, StateBase>();
    
    private StateBase _currentState = null;
    private StateBase _prevState = null;

    public void InitializeProcessor(UnityEngine.Object targetObject)
    {
        foreach(var state in states)
        {
            state.targetObject = targetObject;
            state.processor = this;
            state.Assign();

            _stateMap.Add(state.stateIdentifier,state);
        }
    }

    public void StateProcess(float deltaTime)
    {
        _currentState?.StateProgress(deltaTime);
    }

    public void StateChange(string key)
    {
        var state = GetState(key);

        if(state == null)
        {
            Debug.Log("State does not exists");
            return;
        }

        _currentState?.StateChanged(state);
        _prevState = _currentState;
        _currentState = state;

        _currentState.StateInitialize(_prevState);

        currentState = _currentState.stateIdentifier;
    }

    public StateBase GetState(string key)
    {
        return _stateMap.ContainsKey(key) ? _stateMap[key] : null;
    }
}
