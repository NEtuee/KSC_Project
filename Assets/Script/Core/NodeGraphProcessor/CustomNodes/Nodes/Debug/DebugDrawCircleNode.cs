using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Debug/Draw Circle")]
public class DebugDrawCircleNode : LinearConditionalNode
{
    [Input(name = "Position"),SerializeField]
	public Vector3C pos;

    [Input(name = "Radius"),SerializeField]
    public float radius;

    [Input(name = "Color"),SerializeField]
    public Color lineColor;


	public override string		name => "Draw Circle";

	protected override void Process()
	{
        float angle = 0f;
	    for(float i = 0; i <= 36; ++i)
        {
            if(i == 36)
            {
                Vector3 start = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),0f,Mathf.Sin(angle * Mathf.Deg2Rad)) + (Vector3)pos;
                angle = 0f;
                Vector3 end = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),0f,Mathf.Sin(angle * Mathf.Deg2Rad)) + (Vector3)pos;
                
                Debug.DrawLine(start,end,lineColor);
            }
            else
            {
                Vector3 start = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),0f,Mathf.Sin(angle * Mathf.Deg2Rad)) + (Vector3)pos;
                angle += 10f;
                Vector3 end = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),0f,Mathf.Sin(angle * Mathf.Deg2Rad)) + (Vector3)pos;
                
                Debug.DrawLine(start,end,lineColor);
            }
        }
	}
}