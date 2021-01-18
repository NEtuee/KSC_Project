using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSolver
{

    private RayEx ray = new RayEx(new Ray(Vector3.zero,Vector3.zero),0f,0);
    public bool FindEdgePoint(Vector3 anchor, Vector3 pos, ref List<Vector3> point, int accuracy, LayerMask layer)
    {
        var origin = pos;
        int limit = 1;
        while(true)
        {
            float angle = 90f;
            bool find = false;
            Vector3 hitPoint = Vector3.zero;
            var dir = (anchor - pos).normalized;
            var dist = Vector3.Distance(anchor,pos);

            for(int i = 0; i < accuracy; ++i)
            {
                ray.SetDirection(dir);
                ray.CheckLayer = layer;
                ray.Distance = dist;

                var isHit = ray.Cast(pos,out RaycastHit hit);

                if(!isHit && !find)
                {
                    break;
                }
                else if(isHit)
                {
                    hitPoint = hit.point;
                }

                find = true;
                angle *= 0.5f;

                var sliding =  MathEx.GetSlidingVector(dir,hit.normal);

                Quaternion dirQuat = Quaternion.LookRotation(sliding,Vector3.up);
               // dirQuat = dirQuat * Quaternion.Euler(Vector3.up * angle);

                Debug.Log(dir.x + "," + dir.y + "," + dir.z);
                dir = dirQuat * dir;
                Debug.Log(dir.x + "," + dir.y + "," + dir.z);
                pos = dir * dist;
            }
            
            if(!find)
                break;

            point.Add(hitPoint);
            anchor = hitPoint;
            pos = origin;

            --limit;
            if(limit == 0)
            {
                Debug.Log("limit");
                Debug.Log(anchor);
                break;
            }
        }


        return point.Count > 0;
    }
}
