using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_Controll : SingletonMono<LevelEdit_Controll>
{
    public LevelEdit_BehaviorControll behaviorControll;
    private LevelEdit_PointManager pointManager;
    private List<LevelEdit_Trigger> activeTriggers = new List<LevelEdit_Trigger>();
    public int currentPoint = -1;

    private void Start()
    {
        SetSingleton(this);

        pointManager = GetComponent<LevelEdit_PointManager>();
    }

    void Update()
    {
        behaviorControll.Progress();
        if(behaviorControll.GetState() == LevelEdit_BehaviorControll.State.MoveEnd)
        {
            Launch();
        }
        else if(behaviorControll.GetState() == LevelEdit_BehaviorControll.State.StepIdle)
        {
            if(activeTriggers.Count > 0)
            {
                foreach(var trigger in activeTriggers)
                {
                    trigger.TriggerEventInvoke();
                }

                activeTriggers.Clear();
            }
            
        }
    }

    public void SetRightFoot()
    {
        behaviorControll.SetStepFoot(true);
    }

    public void AddActiveTrigger(LevelEdit_Trigger t)
    {
        activeTriggers.Add(t);
    }

    public void Launch()
    {
        behaviorControll.SetMoveInfo(pointManager.GetList());
    }

    public void SetPrevState()
    {
        behaviorControll.SetPrevState();
    }
    
    public void EditorSetup()
    {
        pointManager = GetComponent<LevelEdit_PointManager>();
    }

    public void AddPoint(LevelEdit_MovePoint point) {pointManager.AddPoint(point);}
    public void DeletePoint(int pos) {pointManager.DeletePoint(pos);}

    public LevelEdit_MovePoint GetPoint(int pos) {return pointManager.GetPoint(pos);}
    public List<LevelEdit_MovePoint> GetPointList() {return pointManager.GetList();}
}
