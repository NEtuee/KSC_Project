using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CL_Node
{
    public Vector3 min = Vector3.zero;
    public Vector3 max = Vector3.zero;

    public CL_Node[] child = { null, null, null, null };
    public CL_Node()
    {
    }
    public CL_Node(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }
}
public class ClimbingLineManager : ManagerBase
{
    public List<ClimbingLine> climbingLines = new List<ClimbingLine>();
    private CL_Node _rootNode = new CL_Node();


    //public override void Assign()
    //{
    //    base.Assign();
    //    SaveMyNumber("ClimbingLineManager");

    //    AddAction(MessageTitles.climbingLineManager_getClimbingLineManager, (msg) =>
    //     {
    //         var target = (MessageReceiver)msg.sender;
    //         SendMessageQuick(target, MessageTitles.set_climbingLineManager,this);
    //     });
    //}

    public void BulidNode(int minClimbingLineNum)
    {

    }

    private void DivideNode(CL_Node node, int minClimbingLineNum)
    {
        int count = 0;
        foreach(var line in climbingLines)
        {
            if(CheckClimbingLineInNode(node,line))
            {

            }
        }
    }

    private bool CheckClimbingLineInNode(CL_Node node, ClimbingLine line)
    {
        Vector3 linePosition = line.transform.position;

        if (node.min.x >= linePosition.x || node.max.x <= linePosition.x)
            return false;

        if (node.min.y >= linePosition.y || node.max.y <= linePosition.y)
            return false;

        if (node.min.z >= linePosition.z || node.max.z <= linePosition.z)
            return false;

        return true;
    }
    protected override void Awake()
    {
        base.Awake();
        SaveMyNumber("ClimbingLineManager");
        RegisterRequest();

        AddAction(MessageTitles.climbingLineManager_getClimbingLineManager, (msg) =>
        {
            var target = (MessageReceiver)msg.sender;
            SendMessageQuick(target, MessageTitles.set_climbingLineManager, this);
        });

        SendMessageQuick(MessageTitles.set_climbingLineManager, GetSavedNumber("Player"), this);
    }

    public void AddClimbingLines(ClimbingLine climbingLine)
    {
        climbingLines.Add(climbingLine);
    }
}
