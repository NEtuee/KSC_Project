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

        float den = (a * c) - (b * b);

        float sc, sn;
        float sd = den;
        float tc, tn;
        float td = den;

        if (den < Mathf.Epsilon)
        {
            sn = 0.0f;
            sd = 1.0f;
            tn = e;
            td = c;
        }
        else
        {
            sn = (b * e - c * d);
            tn = (a * e - b * d);
            if (sn < 0.0f)
            {
                sn = 0.0f;
                tn = e;
                td = c;
            }
            else if (sn > sd)
            {
                sn = sd;
                tn = e + b;
                td = c;
            }
        }

        if (tn < 0.0)
        {
            tn = 0.0f;
            if (-d < 0.0f)
            {
                sn = 0.0f;
            }
            else if (-d > a)
            {
                sn = sd;
            }
            else
            {
                sn = -d;
                sd = a;
            }
        }
        else if (tn > td)
        {
            tn = td;
            if ((-d + b) < 0.0f)
            {
                sn = 0;
            }
            else if ((-d + b) > a)
            {
                sn = sd;
            }
            else
            {
                sn = (-d + b);
                sd = a;
            }
        }

        sc = (Mathf.Abs(sn) < Mathf.Epsilon ? 0.0f : sn / sd);
        tc = (Mathf.Abs(tn) < Mathf.Epsilon ? 0.0f : tn / td);

        return Vector3.Magnitude(w + (sc * u) - (tc * v));
    }

    public static float DistanceLineAndLine(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4,out float outS, out float outT)
    {
        Vector3 u = (p2 - p1);
        Vector3 v = (p4 - p3);
        Vector3 w = p1 - p3;

        float a = Vector3.Dot(u, u);
        float b = Vector3.Dot(u, v);
        float c = Vector3.Dot(v, v);
        float d = Vector3.Dot(u, w);
        float e = Vector3.Dot(v, w);

        float den = (a * c) - (b * b);

        float sc, sn;
        float sd = den;
        float tc, tn;
        float td = den;

        if(den < Mathf.Epsilon)
        {
            sn = 0.0f;
            sd = 1.0f;
            tn = e;
            td = c;
        }
        else
        {
            sn = (b * e - c * d);
            tn = (a * e - b * d);
            if(sn < 0.0f)
            {
                sn = 0.0f;
                tn = e;
                td = c;
            }
            else if(sn > sd)
            {
                sn = sd;
                tn = e + b;
                td = c;
            }
        }

        if(tn < 0.0)
        {
            tn = 0.0f;
            if(-d < 0.0f)
            {
                sn = 0.0f;
            }
            else if(-d > a)
            {
                sn = sd;
            }
            else
            {
                sn = -d;
                sd = a;
            }
        }
        else if(tn > td)
        {
            tn = td;
            if((-d+b)<0.0f)
            {
                sn = 0;
            }
            else if((-d + b)>a)
            {
                sn = sd;
            }
            else
            {
                sn = (-d + b);
                sd = a;
            }
        }

        sc = (Mathf.Abs(sn) < Mathf.Epsilon ? 0.0f : sn / sd);
        tc = (Mathf.Abs(tn) < Mathf.Epsilon ? 0.0f : tn / td);

        outS = sc;
        outT = tc;

        return Vector3.Magnitude(w + (sc * u) - (tc * v));


        //if(s <= 0)
        //{
        //    s = Vector3.Dot(u, w) / Vector3.Dot(u,u);
        //}
        //else if(s>=1)
        //{
        //    s = (Vector3.Dot(u, w) + Vector3.Dot(v,u)) / Vector3.Dot(u, u);
        //}

    }

    public static bool IntersectionCapsuleAndLine(Vector3 start, Vector3 end, float radius, Vector3 lineStart, Vector3 lineEnd)
    {
        float minDist = DistanceLineAndLine(start, end, lineStart, lineEnd);
        if (minDist <= radius)
            return true;

        return false;
    }

    public static Vector3 ShortestPointLineSegmentAndPoint(Vector3 p1, Vector3 p2, Vector3 point)
    {
        Vector3 u = p2 - p1;
        Vector3 v = point - p1;

        return Vector3.Project(v, u);
    }
}
