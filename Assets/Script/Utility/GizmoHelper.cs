using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 디버그용 기즈모 그리는 클래스, 다른 프로젝트에서 가져온거임
public class GizmoHelper : MonoBehaviour
{
    public enum DrawType
    {
        Line,
        Point
    };

    public class GizmoItem
    {
        public List<Vector3> Points = new List<Vector3>();
        public DrawType Type;
        public Color Color;
    }

    public static GizmoHelper Instance;
	public void SetSingleton(GizmoHelper t){Instance = t;}

    List<GizmoItem> DrawList = new List<GizmoItem>();

    Queue<GizmoItem> Queue = new Queue<GizmoItem>();

    private bool gizmoUpdate = false;

    public void Awake()
    {
        Instance = this;
    }

    GizmoItem GetItem() 
    {
        if(Queue.Count == 0)
            return new GizmoItem();
        else
            return Queue.Dequeue();
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        if (!gizmoUpdate)
            return;
        
        var item = GetItem();
        item.Points.Add(start);
        item.Points.Add(end);
        item.Color = color;
        item.Type = DrawType.Line;

        DrawList.Add(item);
    }

    public void DrawRectangle(Vector3 position, Vector2 half, float radian, Color color)
    {
        if (!gizmoUpdate)
            return;

        var item = GetItem();
        var cosTheta = Mathf.Cos(radian);
        var sinTheta = Mathf.Sin(radian);
        var lefttop = new Vector3(-half.x, half.y);
        var leftbottom = new Vector3(-half.x, -half.y);
        var righttop = new Vector3(half.x, half.y);
        var rightbottom = new Vector3(half.x, -half.y);

        lefttop = new Vector3(lefttop.x * cosTheta - lefttop.y * sinTheta, lefttop.x * sinTheta + lefttop.y * cosTheta);
        leftbottom = new Vector3(leftbottom.x * cosTheta - leftbottom.y * sinTheta, leftbottom.x * sinTheta + leftbottom.y * cosTheta);
        rightbottom = new Vector3(rightbottom.x * cosTheta - rightbottom.y * sinTheta, rightbottom.x * sinTheta + rightbottom.y * cosTheta);
        righttop = new Vector3(righttop.x * cosTheta - righttop.y * sinTheta, righttop.x * sinTheta + righttop.y * cosTheta);



        item.Points.Add(position + lefttop);
        item.Points.Add(position + leftbottom);
        item.Points.Add(position + rightbottom);
        item.Points.Add(position + righttop);
        item.Points.Add(position + lefttop);


        DrawList.Add(item);
    }

    public void DrawCircle(Vector3 position, float radius, Color color)
    {
        if (!gizmoUpdate)
            return;

        var item = GetItem();

        for(int i = 0; i <= 36; ++i)
        {
            var angle = 10f * (float)i;
            var radian = angle * Mathf.Deg2Rad;
            var point = position + new Vector3(Mathf.Cos(radian),Mathf.Sin(radian)) * radius;

            angle = 10f * (float)(i == 36 ? 0 : i + 1);
            radian = angle * Mathf.Deg2Rad;
            var point2 = position + new Vector3(Mathf.Cos(radian),Mathf.Sin(radian)) * radius;

            item.Points.Add(point);
            item.Points.Add(point2);
        }

        item.Type = DrawType.Line;
        item.Color = color;

        DrawList.Add(item);
    }

    public void OnDrawGizmos()
    {
        gizmoUpdate = true;
        foreach(var item in DrawList)
        {
            Gizmos.color = item.Color;

            if(item.Type == DrawType.Line)
            {
                for(int i = 0; i < item.Points.Count - 1; ++i)
                {
                    Gizmos.DrawLine(item.Points[i],item.Points[i + 1]);
                }
            }
            
            item.Points.Clear();
            Queue.Enqueue(item);
        }

        DrawList.Clear();
    }
}