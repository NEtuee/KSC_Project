using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;

[System.Serializable]
public class GraphObjectBase : UnTransfromObjectBase
{
    public LevelObjectGraph graphOrigin;
    private Dictionary<string,EntryNode> _entryNodes = new Dictionary<string, EntryNode>();

    protected LevelObjectGraph _graph;

    private Animator _animatorControll;

    HashSet<BaseNode> _nodeDependenciesGathered = new HashSet<BaseNode>();
	HashSet<BaseNode> _skipConditionalHandling  = new HashSet<BaseNode>();

    Stack<BaseNode> _nodeToExecute = new Stack<BaseNode>();
    Stack<BaseNode> _dependencies = new Stack<BaseNode>();

    Queue<Message> _receivedMessaged = new Queue<Message>();

    TimeCounterEx _timeCounterEx = new TimeCounterEx();

    protected override void Awake()
    {
        if(graphOrigin == null)
            Debug.LogError("Graph is null");
        InitGraph();
        base.Awake();

        if(!TryGetComponent<Animator>(out _animatorControll))
        {
            _animatorControll = GetComponentInChildren<Animator>();
        }
        
    }

    public override void Assign()
    {
        base.Assign();
        AddAction(MessageTitles.player_NormalHit,EMPHitMessage);
        AddAction(MessageTitles.player_EMPHit,EMPHitMessage);
        AddAction(MessageTitles.scan_scanned,ScannedMessage);
        AddAction(MessageTitles.scene_sceneChanged, SceneLoadedMessage);

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

            ClearMessageQueue();
        }
    }

    public override void AfterProgress(float deltaTime)
    {
        base.AfterProgress(deltaTime);

        var node = FindNode("AfterProgress");
        if(node != null)
        {
            ((ObjectAfterProgressEntryNode)node).deltaTime = deltaTime;
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        var node = FindNode("FixedProgress");
        if(node != null)
        {
            ((ObjectFixedProgressEntryNode)node).deltaTime = deltaTime;
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public override void Release()
    {
        base.Release();

        RunGraph("Release");

        Destroy(_graph);
    }

    public void OnTriggerEnter(Collider coll)
    {
        var node = FindNode("TriggerEnter");
        if(node != null)
        {
            ((ObjectTriggerEnterEntryNode)node).collider = coll;
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public void OnTriggerStay(Collider coll)
    {
        var node = FindNode("TriggerStay");
        if(node != null)
        {
            ((ObjectTriggerEnterEntryNode)node).collider = coll;
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        var node = FindNode("TriggerExit");
        if(node != null)
        {
            ((ObjectTriggerExitEntryNode)node).collider = coll;
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public void WhenEMPHit(float damage)
    {
        var node = FindNode("EMPHit");
        if(node != null)
        {
            ((ObjectEMPHitEntryNode)node).damage = damage;
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public void WhenScanned()
    {
        RunGraph("Scanned");

        ClearMessageQueue();
    }

#region Behaviour

    public void Turn(bool isLeft, Transform target, float rotationSpeed, float deltaTime)
    {
        Turn(target, rotationSpeed * deltaTime * (isLeft ? 1f : -1f));
    }

    public void Turn(bool isLeft, Transform target, float rotationSpeed, float deltaTime, Vector3 axis)
    {
        Turn(target, rotationSpeed * deltaTime * (isLeft ? 1f : -1f),axis);
    }

    public void Turn(Transform target, float factor, Vector3 axis)
    {
        target.RotateAround(target.position,axis,factor);
    }

    public void Turn(Transform target, float factor)
    {
        target.RotateAround(target.position,target.up,factor);
    }

#endregion


#region Message

    public void SceneLoadedMessage(Message msg)
    {
        Debug.Log("Check");
        var node = FindNode("SceneChanged");
        if (node != null)
        {
            RunGraph(node);

            ClearMessageQueue();
        }
    }

    public void ScannedMessage(Message msg)
    {
        WhenScanned();
    }

    public void EMPHitMessage(Message msg)
    {
        //var data = MessageDataPooling.CastData<MD.FloatData>(msg.data);

        WhenEMPHit(0f);
    }

    public override bool CanHandleMessage(Message msg)
    {
        return true;
    }

    // public override void ReceiveAndProcessMessage(Message msg)
    // {
    //     _receivedMessaged.Enqueue(msg);
    // }

    public override void MessageProcessing(Message msg)
    {
        if(msg.title == MessageTitles.player_EMPHit || 
            msg.title == MessageTitles.scan_scanned || 
            msg.title == MessageTitles.player_NormalHit ||
            msg.title == MessageTitles.scene_sceneChanged)
        {
            base.MessageProcessing(msg);
        }
        else
        {
            var data = MessagePool.GetMessage();
            data.title = msg.title;
            data.sender = msg.sender;
            data.target = msg.target;
            data.data = msg.data;

            _receivedMessaged.Enqueue(data);
        }
    }

    public void ClearMessageQueue()
    {
        if(_receivedMessaged.Count == 0)
            return;
            
        var msg = _receivedMessaged.Dequeue();
        while(msg != null)
        {
            MessagePool.ReturnMessage(msg);
            if(_receivedMessaged.Count == 0)
            {
                msg = null;
            }
            else
                msg = _receivedMessaged.Dequeue();
        }
    }

    public Message DequeueGraphMessage()
    {
        return _receivedMessaged.Count == 0 ? null : _receivedMessaged.Dequeue();
    }

#endregion

    public void InitTimer(string name,float timelimit)
    {
        _timeCounterEx.InitTimer(name,0f,timelimit);
    }

    public float IncreaseTimer(string name, float deltaTime, out bool limit)
    {
        return _timeCounterEx.IncreaseTimerSelf(name,out limit,deltaTime);
    }


    public Animator GetAnimator() {return _animatorControll;}

    public LevelObjectGraph GetCopyedGraph()
    {
        return _graph;
    }

    EntryNode FindNode(string key)
    {
        return _entryNodes.ContainsKey(key) ? _entryNodes[key] : null;
    }

    public void CallFunction(string function)
    {
        var func = _graph.FindFunction(function);
        if(func == null)
        {
            Debug.LogError("function not exists : " + function);
            return;
        }

        RunFunction(func.entryNode);
    }

    void RunFunction(FunctionStartNode node)
    {
        _nodeToExecute.Clear();
        _nodeToExecute.Push(node);
        RunTheGraph(_nodeToExecute);
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

        _graph.GetExposedParameterFromGUID(_graph.transformGUID).value = transform;
        _graph.GetExposedParameterFromGUID(_graph.gameObjectGUID).value = gameObject;
        _graph.GetExposedParameterFromGUID(_graph.levelObjectGUID).value = this;

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
                            {
								nodeToExecute.Push(n);
                            }
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
                        {
				            foreach (var edge in port.GetEdges())
				            	if(!(edge.outputNode is IConditionalNode))
                                {
                                    _dependencies.Push(edge.outputNode);
                                }
                        }

                        if (dependency != node)
                        {
                            nodeToExecute.Push(dependency);
                        }
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