using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelEdit_PointManager : MonoBehaviour
{
    public List<LevelEdit_MovePoint> movePoints = new List<LevelEdit_MovePoint>();

    public LevelEdit_MovePoint GetNextPoint(ref int currentPoint)
    {
        currentPoint = currentPoint >= movePoints.Count - 1 ? 0 : currentPoint + 1;
        return GetPoint(currentPoint);
    }

    public LevelEdit_MovePoint GetPoint(int point)
    {
        return movePoints[point];
    }


    public List<LevelEdit_MovePoint> GetList() {return movePoints;}
    public void AddPoint(LevelEdit_MovePoint point)
    {
        movePoints.Add(point);
    }

    public void DeletePoint(int point)
    {
        DeletePoint(GetPoint(point));
    }
    public void DeletePoint(LevelEdit_MovePoint point)
    {
        movePoints.Remove(point);

        DestroyImmediate(point.gameObject);
    }

#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        for(int i = 0; i < movePoints.Count; ++i)
        {
            Handles.Label(movePoints[i].transform.position, "p" + i);

            Handles.color = Color.red;
            Handles.Label(movePoints[i].GetBezierPoint1().position, "p" + i + " Bezier_1");
            Handles.Label(movePoints[i].GetBezierPoint2().position, "p" + i + " Bezier_2");
            Handles.DrawLine(movePoints[i].GetPoint(),movePoints[i].GetBezierPoint1().position);
            Handles.DrawLine(movePoints[(i == movePoints.Count - 1 ? 0 : i + 1)].GetPoint(),movePoints[i].GetBezierPoint2().position);
        }

        int accur = 10;

        Handles.color = Color.white;

        for(int i = 0; i < movePoints.Count; ++i)
        {
            Vector2 startPoint = MathEx.Vector3ToVector2(movePoints[i].GetPoint());
            Vector2 endPoint = MathEx.Vector3ToVector2(movePoints[(i == movePoints.Count - 1 ? 0 : i + 1)].GetPoint());

            Vector2 p0 = MathEx.Vector3ToVector2(movePoints[i].GetBezierPoint1().position);
            Vector2 p1 = MathEx.Vector3ToVector2(movePoints[i].GetBezierPoint2().position);

            Vector3 currentPoint = MathEx.Vector2ToVector3(startPoint);

            for(int j = 1; j <= accur; ++j)
            {
                var target = MathEx.Vector2ToVector3(MathEx.GetPointOnBezierCurve(startPoint,p0,p1,endPoint,j / (float)accur));

                Handles.DrawLine(currentPoint,target);
                currentPoint = target;
            }

        }



    }
#endif
}
