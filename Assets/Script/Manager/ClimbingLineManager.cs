using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CL_Node
{
    public List<ClimbingLine> climbingLines = new List<ClimbingLine>();

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
    public CL_Node _rootNode = new CL_Node();


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
        DivideNode(_rootNode, minClimbingLineNum);
    }

    private void DivideNode(CL_Node node, int minClimbingLineNum)
    {
        int count = 0;
        foreach(var line in climbingLines)
        {
            if(CheckClimbingLineInNode(node,line))
            {
                count++;
            }

            if(count > minClimbingLineNum)
            {
                CL_Node first = new CL_Node(node.min, new Vector3((node.min.x+ node.max.x)*0.5f, node.max.y, (node.min.z + node.max.z) * 0.5f));
                CL_Node second = new CL_Node(new Vector3((node.min.x + node.max.x) * 0.5f, node.min.y, node.min.z), new Vector3(node.max.x, node.max.y, (node.min.z + node.max.z) * 0.5f));
                CL_Node third = new CL_Node(new Vector3(node.min.x, node.min.y, (node.min.z + node.max.z)*0.5f), new Vector3((node.min.x + node.max.x) * 0.5f, node.max.y, node.max.z));
                CL_Node fourth = new CL_Node(new Vector3((node.min.x + node.max.x) * 0.5f, node.min.y, (node.min.z + node.max.z) * 0.5f), node.max);

                node.child[0] = first;
                node.child[1] = second;
                node.child[2] = third;
                node.child[3] = fourth;

                for(int i = 0; i<node.child.Length; i++)
                {
                    DivideNode(node.child[i], minClimbingLineNum);
                }

                return;
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

    public bool CheckPositionInNode(CL_Node node, Vector3 position)
    {
        if (node.min.x >= position.x || node.max.x <= position.x)
            return false;

        if (node.min.y >= position.y || node.max.y <= position.y)
            return false;

        if (node.min.z >= position.z || node.max.z <= position.z)
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
