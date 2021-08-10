using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovePointEx : MonoBehaviour
{
    public Transform bezierPoint1;
    public Transform bezierPoint2;

    public void Initialize()
    {
        bezierPoint1 = CreateBezierPoint(new Vector3(1f,0f,1f));
        bezierPoint2 = CreateBezierPoint(new Vector3(-1f,0f,-1f));
    }

    private Transform CreateBezierPoint(Vector3 point)
    {
        GameObject obj = new GameObject("BezierPoint");
        obj.transform.SetParent(transform);
        obj.transform.position = point;

        return obj.transform;
    }

    public Transform GetBezierPoint1() {return bezierPoint1;}
    public Transform GetBezierPoint2() {return bezierPoint2;}
    

    public Vector3 GetPoint() {return transform.position;}
}