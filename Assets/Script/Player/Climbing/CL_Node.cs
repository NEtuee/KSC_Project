using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CL_Node : MonoBehaviour
{
    public List<ClimbingLine> climbingLines = new List<ClimbingLine>();

    public Vector3 min = Vector3.zero;
    public Vector3 max = Vector3.zero;

    public CL_Node[] child = { null, null, null, null };
    public CL_Node()
    {
    }
    public CL_Node(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }

#if UNITY_EDITOR

    public void DrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 d1 = min;
        Vector3 d2 = new Vector3(min.x, min.y, max.z);
        Vector3 d3 = new Vector3(max.x, min.y, min.z);
        Vector3 d4 = new Vector3(max.x, min.y,max.z);
        Vector3 u1 = new Vector3(min.x, max.y, min.z);
        Vector3 u2 = new Vector3(min.x, max.y, max.z);
        Vector3 u3 = new Vector3(max.x, max.y, min.z);
        Vector3 u4 = max;

        Gizmos.DrawLine(d1, d3);
        Gizmos.DrawLine(d1, d2);
        Gizmos.DrawLine(d2, d4);
        Gizmos.DrawLine(d3, d4);

        Gizmos.DrawLine(u1, u3);
        Gizmos.DrawLine(u1, u2);
        Gizmos.DrawLine(u2, u4);
        Gizmos.DrawLine(u3, u4);

        Gizmos.DrawLine(d1, u1);
        Gizmos.DrawLine(d2, u2);
        Gizmos.DrawLine(d3, u3);
        Gizmos.DrawLine(d4, u4);
    }

#endif
}
