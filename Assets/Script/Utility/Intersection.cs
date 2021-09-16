using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Intersection
{
    public static float DistanceLineAndLine(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Vector3 u = (p2 - p1);
        Vector3 v = (p4 - p3);
        Vector3 w = p1 - p3;

        float a = Vector3.Dot(u, u);
        float b = Vector3.Dot(u, v);
        float c = Vector3.Dot(v, v);
        float d = Vector3.Dot(u, w);
        float e = Vector3.Dot(v, w);

        float den = a * c - (b * b);
        if (den == 0)
            den = Mathf.Epsilon;

        float s = ((b * e) - (c * d)) / den;
        float t = ((a * e) - (b * d)) / den;

        //if(s <= 0)
        //{
        //    s = Vector3.Dot(u, w) / Vector3.Dot(u,u);
        //}
        //else if(s>=1)
        //{
        //    s = (Vector3.Dot(u, w) + Vector3.Dot(v,u)) / Vector3.Dot(u, u);
        //}

        if(s <= 0)
        {
            s = 0;
            t = Vector3.Dot(v, w) / Vector3.Dot(v, v);
        }
        else if(s>=1)
        {
            s = 1;
            t = (Vector3.Dot(v, w) + Vector3.Dot(u, v)) / Vector3.Dot(v, v);
        }

        //outS = s;
        //outT = t;

        Vector3 point1 = p1 + s * u;
        Vector3 point2 = p3 + t * v;

        return Vector3.Distance(point1, point2);
    }

    public static bool IntersectionCapsuleAndLine(Vector3 start, Vector3 end, float radius, Vector3 lineStart, Vector3 lineEnd)
    {
        float minDist = DistanceLineAndLine(start, end, lineStart, lineEnd);
        if (minDist <= radius)
            return true;

        return false;
    }
}
