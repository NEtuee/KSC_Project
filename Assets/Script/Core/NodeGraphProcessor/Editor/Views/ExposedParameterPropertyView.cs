using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
	public class ExposedParameterPropertyView : VisualElement
	{
		protected BaseGraphView baseGraphView;

		public ExposedParameter parameter { get; private set; }

		public Toggle     hideInInspector { get; private set; }

		public ExposedParameterPropertyView(BaseGraphView graphView, ExposedParameter param)
		{
			baseGraphView = graphView;
			parameter      = param;

			if(graphView.exposedParameterFactory == null)
				graphView.exposedParameterFactory = new ExposedParameterFieldFactory(graphView.graph);


			var field = graphView.exposedParameterFactory.GetParameterSettingsField(param, (newValue) => {
				param.settings = newValue as ExposedParameter.Settings;
			});

			Add(field);
		}
	}
} 