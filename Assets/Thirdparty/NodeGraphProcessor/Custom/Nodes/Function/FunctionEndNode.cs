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
    public IEnumerable<ConditionalNode> nextNode;

    public void ChangeTitle(string title)
    {
        this.title = title;
        onTitleChange?.Invoke();
    }

    public override IEnumerable< ConditionalNode >	GetExecutedNodes()
	{
        return nextNode;
    }
}
