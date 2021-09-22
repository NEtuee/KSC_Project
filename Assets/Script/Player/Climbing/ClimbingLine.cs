using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public struct Line
{
    public int p1;
    public int p2;
}

public enum DirectionType
{
    LeftMin, LeftMax
}

public class ClimbingLine : MonoBehaviour
{
    [SerializeField] public List<Transform> points = new List<Transform>();
    [SerializeField] public List<Transform> planeInfo = new List<Transform>();
    [SerializeField] private GameObject maker;
    public DirectionType directionType;

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
            //Instantiate(maker, nearPoint, Quaternion.identity).transform.SetParent(this.transform);
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

    public void AddPoint(Transform point)
    {
        points.Add(point);
        if (points.Count == 1)
            return;

        GameObject newPlaneInfo = new GameObject("PlaneInfo_" + (points.Count - 2)+"-"+(points.Count - 1));
        Vector3 p1top2 = points[points.Count - 1].position - points[points.Count - 2].position;
        newPlaneInfo.transform.position = points[points.Count - 2].position + p1top2 * 0.5f;

        newPlaneInfo.transform.SetParent(transform);
        planeInfo.Add(newPlaneInfo.transform);
    }

    public void RemovePoint(Transform point)
    {
        if (points.Contains(point) == false)
            return;

        int removeNum = points.FindIndex(p => p == point);
        Transform removePoint = points[removeNum];
        points.RemoveAt(removeNum);
        Transform removePlaneInfo = planeInfo[removeNum-1];
        planeInfo.RemoveAt(removeNum-1);

        DestroyImmediate(removePoint.gameObject);
        DestroyImmediate(removePlaneInfo.gameObject);
    }

    public Transform GetPlaneInfo(int leftNum, int rightNum)
    {
        int result = Mathf.Min(leftNum, rightNum);
        if (planeInfo.Count < result + 1)
            return null;

        return planeInfo[result];
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
            Handles.Label(points[i].position, i.ToString());

            if (i == 0)
            {
                Handles.Label(points[i].position + Vector3.up*0.5f, gameObject.name);
                continue;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(points[i].position, points[i - 1].position);
        }

        foreach(var planeInfo in planeInfo)
        {
            //Gizmos.color = Color.blue;
            //Gizmos.DrawRay(planeInfo.position, planeInfo.forward * 0.75f);
            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(0, planeInfo.position, Quaternion.LookRotation(planeInfo.forward),1,EventType.Repaint);

            //Gizmos.color = Color.red;
            //Gizmos.DrawRay(planeInfo.position, planeInfo.right * 0.75f);
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(0, planeInfo.position, Quaternion.LookRotation(planeInfo.right), 1, EventType.Repaint);

            //Gizmos.color = Color.green;
            //Gizmos.DrawRay(planeInfo.position, planeInfo.up * 0.75f);
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(0, planeInfo.position, Quaternion.LookRotation(planeInfo.up), 1, EventType.Repaint);
        }
    }
}
