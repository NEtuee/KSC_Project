using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelEdit_PointManager : MonoBehaviour
{
    public class PathClass
    {
        public string name;
        public List<LevelEdit_MovePoint> movePoints = new List<LevelEdit_MovePoint>();

        public PathClass(string n)
        {
            name = n;
        }
    }

    public List<PathClass> movePaths = new List<PathClass>();

    public void CreatePath(string path)
    {
        movePaths.Add(new PathClass(path));
    }

    public void DisposePath(string path)
    {
        var target = FindPath(path);
        if(target == null)
            return;
        
        foreach(var point in target.movePoints)
        {
            DestroyImmediate(point.gameObject);
        }

        target.movePoints.Clear();
        movePaths.Remove(target);

        target = null;
    }

    public PathClass FindPath(string path)
    {
        return movePaths.Find((x)=>{ return x.name == path;});
    }

    public LevelEdit_MovePoint GetNextPoint(string path, ref int currentPoint)
    {
        currentPoint = currentPoint >= FindPath(path).movePoints.Count - 1 ? 0 : currentPoint + 1;
        return GetPoint(path, currentPoint);
    }

    public LevelEdit_MovePoint GetPoint(string path, int point)
    {
        return FindPath(path).movePoints[point];
    }


    public List<LevelEdit_MovePoint> GetList(string path) 
    {
        var target = FindPath(path);
        if(target == null)
            return null;
        return target.movePoints;
    }
    public void AddPoint(string path, LevelEdit_MovePoint point)
    {
        FindPath(path).movePoints.Add(point);
    }

    public void DeletePoint(string path, int point)
    {
        DeletePoint(path, GetPoint(path, point));
    }
    public void DeletePoint(string path, LevelEdit_MovePoint point)
    {
        FindPath(path).movePoints.Remove(point);

        DestroyImmediate(point.gameObject);
    }

#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        foreach(var list in movePaths)
        {
            for(int i = 0; i < list.movePoints.Count; ++i)
            {
                if(i == 0)
                    Handles.Label(list.movePoints[i].transform.position, list.name);
                else
                    Handles.Label(list.movePoints[i].transform.position, "p" + i);
    
                Handles.color = Color.red;
                Handles.Label(list.movePoints[i].GetBezierPoint1().position, "p" + i + " Bezier_1");
                Handles.Label(list.movePoints[i].GetBezierPoint2().position, "p" + i + " Bezier_2");
                Handles.DrawLine(list.movePoints[i].GetPoint(),list.movePoints[i].GetBezierPoint1().position);
                Handles.DrawLine(list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint(),list.movePoints[i].GetBezierPoint2().position);
            }
        }
        

        int accur = 10;

        Handles.color = Color.white;

        foreach(var list in movePaths)
        {
            for(int i = 0; i < list.movePoints.Count; ++i)
            {
                Vector2 startPoint = MathEx.Vector3ToVector2(list.movePoints[i].GetPoint());
                Vector2 endPoint = MathEx.Vector3ToVector2(list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint());

                Vector2 p0 = MathEx.Vector3ToVector2(list.movePoints[i].GetBezierPoint1().position);
                Vector2 p1 = MathEx.Vector3ToVector2(list.movePoints[i].GetBezierPoint2().position);

                Vector3 currentPoint = MathEx.Vector2ToVector3(startPoint);

                for(int j = 1; j <= accur; ++j)
                {
                    var target = MathEx.Vector2ToVector3(MathEx.GetPointOnBezierCurve(startPoint,p0,p1,endPoint,j / (float)accur));

                    Handles.DrawLine(currentPoint,target);
                    currentPoint = target;
                }

            }
        }


    }
#endif
}
