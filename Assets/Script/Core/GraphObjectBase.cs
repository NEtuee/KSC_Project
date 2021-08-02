using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;

public class GraphObjectBase : UnTransfromObjectBase
{
    public StateMachineGraph graphOrigin;
    private Dictionary<string,EntryNode> _entryNodes = new Dictionary<string, EntryNode>();

    private StateMachineGraph _graph;

    HashSet<BaseNode> _nodeDependenciesGathered = new HashSet<BaseNode>();
	HashSet<BaseNode> _skipConditionalHandling  = new HashSet<BaseNode>();

    Stack<BaseNode> _nodeToExecute = new Stack<BaseNode>();
    Stack<BaseNode> _dependencies = new Stack<BaseNode>();

    protected override void Awake()
    {
        InitGraph();
        base.Awake();
    }

    public override void Assign()
    {
        base.Assign();

        RunGraph("Assign");
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));

        RunGraph("Initialize");
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        var node = FindNode("Progress");
        if(node != null)
        {
            ((ObjectProgressEntryNode)node).deltaTime = deltaTime;
            RunGraph(node);
        }
    }

    public override void AfterProgress(float deltaTime)
    {
        base.AfterProgress(deltaTime);

        RunGraph("AfterProgress");
    }

    public override void Release()
    {
        base.Release();

        RunGraph("Release");

        Destroy(_graph);
    }

    EntryNode FindNode(string key)
    {
        return _entryNodes.ContainsKey(key) ? _entryNodes[key] : null;
    }

    void RunGraph(EntryNode node)
    {
        _nodeToExecute.Clear();
        _nodeToExecute.Push(node);
        RunTheGraph(_nodeToExecute);
    }

    void RunGraph(string key)
    {
        if(!_entryNodes.ContainsKey(key))
            return;
    
        RunGraph(_entryNodes[key]);
    }

    void InitGraph()
    {
        _graph = ScriptableObject.Instantiate(graphOrigin);
        var entryNodeList = _graph.nodes.Where(n => n is EntryNode).Select(n => n as EntryNode).ToList();

        foreach(var node in entryNodeList)
        {
            _entryNodes.Add(node.entryCode,node);
        }
    }

    private void RunTheGraph(Stack<BaseNode> nodeToExecute)
	{
        _nodeDependenciesGathered.Clear();
        _skipConditionalHandling.Clear();
        
		while(nodeToExecute.Count > 0)
		{
			var node = nodeToExecute.Pop();
			// TODO: maxExecutionTimeMS

			// In case the node is conditional, then we need to execute it's non-conditional dependencies first
			if(node is IConditionalNode && !_skipConditionalHandling.Contains(node))
			{
				// Gather non-conditional deps: TODO, move to the cache:
				 if(_nodeDependenciesGathered.Contains(node))
				{
					// Execute the conditional node:
					node.OnProcess();

					// And select the next nodes to execute:
					switch(node)
					{
						// special code path for the loop node as it will execute multiple times the same nodes
						case ForLoopNode forLoopNode:
							forLoopNode.index = forLoopNode.start - 1; // Initialize the start index
							foreach(var n in forLoopNode.GetExecutedNodesLoopCompleted())
								nodeToExecute.Push(n);
							for(int i = forLoopNode.start; i < forLoopNode.end; i++)
							{
								foreach(var n in forLoopNode.GetExecutedNodesLoopBody())
									nodeToExecute.Push(n);

								nodeToExecute.Push(node); // Increment the counter
							}

							_skipConditionalHandling.Add(node);
							break;
						case IConditionalNode cNode:
							foreach(var n in cNode.GetExecutedNodes())
								nodeToExecute.Push(n);
						 	break;
						default:
							Debug.LogError($"Conditional node {node} not handled");
							break;
					}

				 	_nodeDependenciesGathered.Remove(node);
				}
				else
				{
					nodeToExecute.Push(node);
					_nodeDependenciesGathered.Add(node);
                    _dependencies.Clear();

                    _dependencies.Push(node);

                    while (_dependencies.Count > 0)
                    {
                        var dependency = _dependencies.Pop();

                        foreach (var port in dependency.inputPorts)
				            foreach (var edge in port.GetEdges())
				            	if(!(edge.outputNode is IConditionalNode))
                                    _dependencies.Push(edge.outputNode);

                        if (dependency != node)
                            nodeToExecute.Push(dependency);
                    }
				}
			}
			else
			{
				node.OnProcess();
			}
		}
	}
}
