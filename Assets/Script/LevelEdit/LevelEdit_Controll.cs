using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelEdit_PointManager))]
public class LevelEdit_Controll : SingletonMono<LevelEdit_Controll>
{
    private LevelEdit_PointManager pointManager;
    private List<LevelEdit_Trigger> activeTriggers = new List<LevelEdit_Trigger>();
    public int currentPoint = -1;

    private void Start()
    {
        SetSingleton(this);

        pointManager = GetComponent<LevelEdit_PointManager>();
    }


    public void AddActiveTrigger(LevelEdit_Trigger t)
    {
        activeTriggers.Add(t);
    }
    
    public void EditorSetup()
    {
        pointManager = GetComponent<LevelEdit_PointManager>();
    }

    public LevelEdit_PointManager GetPointManager() {return pointManager;}

    public void AddPoint(string path, LevelEdit_MovePoint point) {pointManager.AddPoint(path, point);}
    public void DeletePoint(string path, int pos) {pointManager.DeletePoint(path, pos);}

    public LevelEdit_MovePoint GetPoint(string path, int pos) {return pointManager.GetPoint(path, pos);}
    public List<LevelEdit_MovePoint> GetPointList(string path) {return pointManager.GetList(path);}
}
