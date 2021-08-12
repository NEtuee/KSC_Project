using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Get/Get Text : TextMesh")]
public class GetTextFromTextMesh : BaseNode
{
	[Input(name = "TextMesh")]
	public TextMesh textMesh;


	[Output(name = "Text")]
	public string text;

	public override string		name => "Get Text";

	protected override void Process()
	{
	    text = textMesh.text;
	}
}