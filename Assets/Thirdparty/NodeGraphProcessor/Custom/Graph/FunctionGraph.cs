using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable]
public class FunctionGraph : BaseGraph
{
    [System.Serializable]
    public class FunctionInfo
    {
        public string name;
        public int uniqueID;

        [SerializeField,SerializeReference]
        public List<object> inputParams = new List<object>();

        [SerializeField,SerializeReference]
        public List<object> outputParams = new List<object>();

        [SerializeField,SerializeReference]
        public FunctionStartNode entryNode;

        [SerializeField,SerializeReference]
        public FunctionEndNode endNode;

        public Action<string> onNameChanged;

        public void UpdateNodeTitle()
        {
            entryNode.ChangeTitle(name);
            endNode.ChangeTitle(name);
            onNameChanged?.Invoke(name);
        }

#if UNITY_EDITOR
        public bool hideInList = false;
#endif

    }

    [SerializeField,SerializeReference]
    public List<FunctionInfo> functions = new List<FunctionInfo>();

    [SerializeField,SerializeReference]
    public int functionID = 0;

    public event Action onFunctionListChanged;

    public FunctionInfo FindFunction(int id) {return functions.Find((x)=>{return x.uniqueID == id;});}

    public void AddFunction(FunctionInfo info)
    {
        functions.Add(info);
        onFunctionListChanged?.Invoke();
    }

    public void RemoveFunction(FunctionInfo info)
    {
        functions.Remove(info);
        onFunctionListChanged?.Invoke();
    }

    public void UpdateFunctionName(FunctionGraph.FunctionInfo function, string name)
    {
        function.name = name;
        function.UpdateNodeTitle();
    }

    // public void AddFunction(Vector2 position)
    // {
    //     var function = new FunctionInfo{
    //         name = "New Function " + functionID,
    //         uniqueID = ++functionID,
    //     };

    //     var startNode = BaseNode.CreateFromType<FunctionStartNode>(position);
    //     AddNode(startNode);

    //     var endNode = BaseNode.CreateFromType<FunctionEndNode>(position + new Vector2(10f,10f));
    //     AddNode(endNode);

    //     function.entryNode = startNode;
    //     function.endNode = endNode;

    //     functions.Add(function);
    //     onFunctionListChanged?.Invoke();
    // }

    // public void UpdateFunctionName(FunctionInfo function, string name)
    // {
    //     function.name = name;
    // }

    // public void RemoveFunction(FunctionInfo function)
    // {
    //     if(function.entryNode != null)
    //     {
    //         RemoveNode(function.entryNode);
    //     }

    //     if(function.entryNode != null)
    //     {
    //         RemoveNode(function?.endNode);
    //     }
        

    //     functions.Remove(function);

    //     onFunctionListChanged?.Invoke();
    // }
}
