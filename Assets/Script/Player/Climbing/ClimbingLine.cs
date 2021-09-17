using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    public int p1;
    public int p2;
}

public class ClimbingLine : MonoBehaviour
{
    [SerializeField] public List<Transform> points = new List<Transform>();
    [SerializeField] private GameObject maker;

    public bool DetectLine(Vector3 start, Vector3 end, float radius, Transform playerPos,out Vector3 nearPoint, ref Line line)
    {
        nearPoint = Vector3.zero;
        if (points.Count <= 1)
        {
            return false;
        }

        bool detect = false;
        for(int i = 1; i < points.Count; i++)
        {
            if(Intersection.IntersectionCapsuleAndLine(start,end,radius,points[i-1].position, points[i].position))
            {
                detect = true;
                Vector3 u = points[i].position - points[i - 1].position;
                Vector3 v = playerPos.position - points[i - 1].position;
                Vector3 near = points[i - 1].position + Vector3.Project(v, u);

                if (nearPoint == Vector3.zero)
                {
                    nearPoint = near;
                    line.p1 = i - 1;
                    line.p2 = i;
                    continue;
                }

                //Debug.Log(Vector3.SqrMagnitude(near - playerPos.position));
                //Debug.Log(Vector3.SqrMagnitude(nearPoint - playerPos.position));
                if (Vector3.SqrMagnitude(near - playerPos.position) < Vector3.SqrMagnitude(nearPoint - playerPos.position))
                {
                    nearPoint = near;
                    line.p1 = i - 1;
                    line.p2 = i;
                }
            }
        }

        if(detect == true)
        {
            Instantiate(maker, nearPoint, Quaternion.identity).transform.SetParent(this.transform);
        }

        return detect;
    }

    public bool PassRight(ref int leftNum,ref int rightNum)
    {
        if(rightNum > leftNum)
        {
            if (rightNum >= points.Count - 1)
                return false;

            rightNum++;
            leftNum++;
        }
        else
        {
            if (rightNum <= 0)
                return false;

            rightNum--;
            leftNum--;
        }

        return true;
    }
    public bool PassLeft(ref int leftNum, ref int rightNum)
    {
        if (leftNum > rightNum)
        {
            if (leftNum >= points.Count - 1)
                return false;

            rightNum++;
            leftNum++;
        }
        else
        {
            if (leftNum <= 0)
                return false;

            rightNum--;
            leftNum--;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (points.Count == 0)
            return;

        Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);

        for(int i = 0; i<points.Count; i++)
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
