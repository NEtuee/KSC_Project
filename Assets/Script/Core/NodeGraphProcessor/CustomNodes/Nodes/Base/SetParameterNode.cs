using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using System;
using NodeGraphProcessor.Examples;


[System.Serializable]
public class SetParameterNode : LinearConditionalNode
{
	[Input]
	public object input;

	public override string name => "Parameter";

	// We serialize the GUID of the exposed parameter in the graph so we can retrieve the true ExposedParameter from the graph
	[SerializeField, HideInInspector]
	public string parameterGUID;

	public ExposedParameter parameter { get; private set; }

	public event Action onParameterChanged;

	protected override void Enable()
	{
		// load the parameter
		LoadExposedParameter();

		graph.onExposedParameterModified += OnParamChanged;
		if (onParameterChanged != null)
			onParameterChanged?.Invoke();
	}

	void LoadExposedParameter()
	{
		parameter = graph.GetExposedParameterFromGUID(parameterGUID);

		if (parameter == null)
		{
			Debug.Log("Property \"" + parameterGUID + "\" Can't be found !");

			// Delete this node as the property can't be found
			graph.RemoveNode(this);
			return;
		}
	}

	void OnParamChanged(ExposedParameter modifiedParam)
	{
		if (parameter == modifiedParam)
		{
			onParameterChanged?.Invoke();
		}
	}

	[CustomPortBehavior(nameof(input))]
	IEnumerable<PortData> GetInputPort(List<SerializableEdge> edges)
	{
		yield return new PortData
		{
			identifier = "input",
			displayName = "Value",
			displayType = (parameter == null) ? typeof(object) : parameter.GetValueType(),
		};

	}

	protected override void Process()
	{
#if UNITY_EDITOR // In the editor, an undo/redo can change the parameter instance in the graph, in this case the field in this class will point to the wrong parameter
			if(!UnityEditor.EditorApplication.isPlaying)
				parameter = graph.GetExposedParameterFromGUID(parameterGUID);
#endif

			graph.UpdateExposedParameter(parameter.guid, input);
	}
}

