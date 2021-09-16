using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLine : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public bool DetectLine(Vector3 start, Vector3 end, float radius, out Vector3 nearPoint)
    {
        nearPoint = Vector3.zero;
        if (points.Length <= 1)
        {
            return false;
        }

        bool detect = false;
        for(int i = 1; i < points.Length; i++)
        {
            if(Intersection.IntersectionCapsuleAndLine(start,end,radius,points[i-1].position, points[i].position))
            {
                detect = true;
                Vector3 u = points[i].position - points[i - 1].position;
                Vector3 v = start - points[i - 1].position;
                Vector3 near = points[i - 1].position + Vector3.Project(v, u);
                if(nearPoint == Vector3.zero)
                {
                    nearPoint = near;
                    continue;
                }

                if(Vector3.SqrMagnitude(near - start)< Vector3.SqrMagnitude(nearPoint - start))
                {
                    nearPoint = near;
                }
            }
        }

        return detect;
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);

        for(int i = 0; i<points.Length;i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(points[i].position, size);

            if (i == 0)
                continue;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(points[i].position, points[i - 1].position);
        }
    }
}
