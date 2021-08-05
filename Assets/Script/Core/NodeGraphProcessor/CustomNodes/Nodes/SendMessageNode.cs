using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Object/SendMessage")]
public class SendMessageNode : LinearConditionalNode
{
	[Input(name = "Sender")]
    public MessageReceiver receiver;

	[Input(name = "Title"),SerializeField]
    public ushort title;

	[Input(name = "Target"),SerializeField]
    public int target;

	[Input(name = "Data")]
    public object data = null;

	public override string		name => "SendMessage";

	protected override void Process()
	{
	    receiver.SendMessageEx(title,target,data);
	}
}
