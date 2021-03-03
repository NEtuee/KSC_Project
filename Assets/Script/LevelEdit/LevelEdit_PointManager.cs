using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LevelEdit_PointManager : MonoBehaviour
{
    [System.Serializable]
    public class PathClass
    {
        public string name;
        public List<LevelEdit_MovePoint> movePoints = new List<LevelEdit_MovePoint>();

        public LevelEdit_MovePoint GetNextPoint(ref int point, out bool isEnd)
        {
            isEnd = ++point >= movePoints.Count ? true : false;

            point = point >= movePoints.Count ? 0 : point;
            
            return movePoints[point];
        }

        public LevelEdit_MovePoint FindNearestPoint(Vector3 position, out int target)
        {
            float near = Vector3.Distance(position, movePoints[0].GetPoint());
            target = 0;
            for(int i = 1; i < movePoints.Count; ++i)
            {
                float dist = Vector3.Distance(position,movePoints[i].GetPoint());
                if(dist < near)
                {
                    near = dist;
                    target = i;
                }
            }

            return movePoints[target];
        }

        public LevelEdit_MovePoint GetPoint(int point)
        {
            return movePoints[point];
        }

        public PathClass(string n)
        {
            name = n;
        }
    }

    public List<PathClass> movePaths = new List<PathClass>();

#if UNITY_EDITOR
    public string currentPath;
#endif

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
        if(currentPath == "")
			return;

		var list = FindPath(currentPath);
		if(list == null)
			return;

        for(int i = 0; i < list.movePoints.Count; ++i)
        {
            if(i == 0)
                Handles.Label(list.movePoints[i].transform.position, list.name);
            else
                Handles.Label(list.movePoints[i].transform.position, "p" + i);
    
            Handles.color = Color.red;
            // Handles.Label(list.movePoints[i].GetBezierPoint1().position, "p" + i + " Bezier_1");
            // Handles.Label(list.movePoints[i].GetBezierPoint2().position, "p" + i + " Bezier_2");
            Handles.DrawLine(list.movePoints[i].GetPoint(),list.movePoints[i].GetPoint() + Vector3.up * 10f);
            // Handles.DrawLine(list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint(),list.movePoints[i].GetBezierPoint2().position);
        }

        Handles.color = Color.white;

        for(int i = 0; i < list.movePoints.Count; ++i)
        {
            Vector3 startPoint = list.movePoints[i].GetPoint();
            Vector3 endPoint = list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint();

            Handles.DrawLine(startPoint,endPoint);

        }
    //     foreach(var list in movePaths)
    //     {
    //         for(int i = 0; i < list.movePoints.Count; ++i)
    //         {
    //             if(i == 0)
    //                 Handles.Label(list.movePoints[i].transform.position, list.name);
    //             else
    //                 Handles.Label(list.movePoints[i].transform.position, "p" + i);
    
    //             Handles.color = Color.red;
    //             // Handles.Label(list.movePoints[i].GetBezierPoint1().position, "p" + i + " Bezier_1");
    //             // Handles.Label(list.movePoints[i].GetBezierPoint2().position, "p" + i + " Bezier_2");
    //             Handles.DrawLine(list.movePoints[i].GetPoint(),list.movePoints[i].GetPoint() + Vector3.up * 10f);
    //             // Handles.DrawLine(list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint(),list.movePoints[i].GetBezierPoint2().position);
    //         }
    //     }
        

    //     Handles.color = Color.white;

    //     foreach(var list in movePaths)
    //     {
    //         for(int i = 0; i < list.movePoints.Count; ++i)
    //         {
    //             Vector3 startPoint = list.movePoints[i].GetPoint();
    //             Vector3 endPoint = list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint();

    //             Handles.DrawLine(startPoint,endPoint);

    //         }
    }
#endif
}
