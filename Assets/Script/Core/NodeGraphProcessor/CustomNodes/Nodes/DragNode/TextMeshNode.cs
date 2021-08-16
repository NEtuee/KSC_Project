using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("Reference/Text Mesh")]
public class TextMeshNode : BaseNode, ICreateNodeFrom<TextMesh>
{
	[Output(name = "Out"), SerializeField]
	public TextMesh			output;

	public override string		name => "Text Mesh";

	public bool InitializeNodeFromObject(TextMesh value)
	{
		output = value;
		return true;
	}
}