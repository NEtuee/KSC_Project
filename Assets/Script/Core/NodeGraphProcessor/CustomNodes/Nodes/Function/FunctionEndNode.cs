using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using NodeGraphProcessor.Examples;

[System.Serializable]
public class FunctionEndNode : LinearConditionalNode, IConditionalNode
{
    public Action onTitleChange;
    public string title = "";
    public List<ConditionalNode> nextNode;
    
    public override bool deletable => false;

    public void ChangeTitle(string title)
    {
        this.title = title;
        onTitleChange?.Invoke();
    }

    public override List<ConditionalNode>	GetExecutedNodes()
	{
        return nextNode;
    }
}
