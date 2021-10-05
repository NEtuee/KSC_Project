using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringRulerPointList", menuName = "Options/StringRulerPointList")]
public class StringRulerPointList : ScriptableObject
{
    [System.Serializable]
    public class PointInfo
    {
        public Vector3 position;
        public float distance;
        public float total;
        public float angle;
    }

    public List<PointInfo> points = new List<PointInfo>();

    public void Add(Vector3 position)
    {
        var item = new PointInfo();
        item.position = position;
        item.angle = 0f;

        points.Add(item);
        CalcDistanceAndAngle(points.Count - 1);
        CalcTotal(points.Count - 1);
    }

    public void CalcTotal(int target)
    {
        for(int i = target; i < points.Count; ++i)
        {
            if(i == 0)
            {
                points[i].total = 0f;
            }
            else
            {
                points[i].total = points[i - 1].total + points[i].distance;
            }
        }
    }

    public void CalcDistanceAndAngle(int target)
    {
        if(target == 0)
            return;
        
        var dist = Vector3.Distance(points[target - 1].position,points[target].position);
        var angle = Vector3.Dot(points[target - 1].position.normalized,-points[target].position.normalized) * Mathf.Rad2Deg;
        
        points[target].distance = dist;
        points[target].angle = angle;
    }

    public void Clear()
    {
        points.Clear();
    }
}
