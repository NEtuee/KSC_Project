using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLineManager : ManagerBase
{
    public List<ClimbingLine> climbingLines = new List<ClimbingLine>();
    public List<ClimbingLine> dynamicClimbingLines = new List<ClimbingLine>();
    public CL_Node _rootNode = null;

    private bool _drawNode = false;
    public bool DrawNode { get => _drawNode; set => _drawNode = value; }
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
    public void ClearNode()
    {
        DestoyNode(_rootNode);
        _rootNode.climbingLines.Clear();
    }

    private void DestoyNode(CL_Node node)
    {
        if (node.child[0] != null)
        {
            DestoyNode(node.child[0]);
            DestoyNode(node.child[1]);
            DestoyNode(node.child[2]);
            DestoyNode(node.child[3]);

            node.child[0] = node.child[1] = node.child[2] = node.child[3] = null;
        }

        if(_rootNode != node)
          DestroyImmediate(node.gameObject);
    }

    public void BulidNode(int maxClimbingLineNum)
    {
        if (_rootNode.child[0] != null)
            ClearNode();

        DivideNode(_rootNode, maxClimbingLineNum);
    }

    private void DivideNode(CL_Node node, int maxClimbingLineNum)
    {
        int count = 0;
        foreach(var line in climbingLines)
        {
            if(CheckClimbingLineInNode(node,line))
            {
                count++;
                node.climbingLines.Add(line);
            }

            if(count > maxClimbingLineNum)
            {
                GameObject newNode = new GameObject("Node");
                newNode.transform.SetParent(_rootNode.transform);
                CL_Node first = newNode.AddComponent<CL_Node>();
                first.min = node.min;
                first.max = new Vector3((node.min.x + node.max.x) * 0.5f, node.max.y, (node.min.z + node.max.z) * 0.5f);

                newNode = new GameObject("Node");
                newNode.transform.SetParent(_rootNode.transform);
                CL_Node second = newNode.AddComponent<CL_Node>();
                second.min = new Vector3((node.min.x + node.max.x) * 0.5f, node.min.y, node.min.z);
                second.max = new Vector3(node.max.x, node.max.y, (node.min.z + node.max.z) * 0.5f);

                newNode = new GameObject("Node");
                newNode.transform.SetParent(_rootNode.transform);
                CL_Node third = newNode.AddComponent<CL_Node>();
                third.min = new Vector3(node.min.x, node.min.y, (node.min.z + node.max.z) * 0.5f);
                third.max = new Vector3((node.min.x + node.max.x) * 0.5f, node.max.y, node.max.z);

                newNode = new GameObject("Node");
                newNode.transform.SetParent(_rootNode.transform);
                CL_Node fourth = newNode.AddComponent<CL_Node>();
                fourth.min = new Vector3((node.min.x + node.max.x) * 0.5f, node.min.y, (node.min.z + node.max.z) * 0.5f);
                fourth.max = node.max;

                //CL_Node first = new CL_Node(node.min, new Vector3((node.min.x+ node.max.x)*0.5f, node.max.y, (node.min.z + node.max.z) * 0.5f));
                //CL_Node second = new CL_Node(new Vector3((node.min.x + node.max.x) * 0.5f, node.min.y, node.min.z), new Vector3(node.max.x, node.max.y, (node.min.z + node.max.z) * 0.5f));
                //CL_Node third = new CL_Node(new Vector3(node.min.x, node.min.y, (node.min.z + node.max.z)*0.5f), new Vector3((node.min.x + node.max.x) * 0.5f, node.max.y, node.max.z));
                //CL_Node fourth = new CL_Node(new Vector3((node.min.x + node.max.x) * 0.5f, node.min.y, (node.min.z + node.max.z) * 0.5f), node.max);

                node.child[0] = first;
                node.child[1] = second;
                node.child[2] = third;
                node.child[3] = fourth;

                for(int i = 0; i<node.child.Length; i++)
                {
                    DivideNode(node.child[i], maxClimbingLineNum);
                }

                return;
            }
        }
    }

   

    private bool CheckClimbingLineInNode(CL_Node node, ClimbingLine line)
    {
        //Vector3 linePosition = line.transform.position;
        //Vector3 startPoint = line.points[0].position;
        //Vector3 endPoint = line.points[line.points.Count - 1].position;

        //if ((node.min.x > startPoint.x || node.max.x < startPoint.x) &&
        //    (node.min.x > endPoint.x || node.max.x < endPoint.x))
        //    return false;

        //if ((node.min.y > startPoint.y || node.max.y < startPoint.y) &&
        //    (node.min.y > endPoint.y || node.max.y < endPoint.y))
        //    return false;

        //if ((node.min.z > startPoint.z || node.max.z < startPoint.z)&&
        //    (node.min.z > endPoint.z || node.max.z < endPoint.z))
        //    return false;

        foreach(var point in line.points)
        {
            if (CheckPositionInNode(node, point.position) == true)
            {
                return true;
            }
        }

        for(int i = 1; i<line.points.Count; i++)
        {
            Vector3 dir = line.points[i - 1].position - line.points[i].position;
            dir.Normalize();
            if(RayIntersectWithCLNode(line.points[i].position,dir,node) == true)
            {
                return true;
            }
        }

        return false;
    }

    private static bool RayIntersectWithCLNode(Vector3 start, Vector3 dir, CL_Node node)
    {
        Vector3 dirfrac = new Vector3(1.0f / dir.x, 1.0f / dir.y, 1.0f / dir.z);

        float t1 = (node.min.x - start.x) * dirfrac.x;
        float t2 = (node.max.x - start.x) * dirfrac.x;
        float t3 = (node.min.y - start.y) * dirfrac.y;
        float t4 = (node.max.y - start.y) * dirfrac.y;
        float t5 = (node.min.z - start.z) * dirfrac.z;
        float t6 = (node.max.z - start.z) * dirfrac.z;

        float tmin = Mathf.Max(Mathf.Max(Mathf.Min(t1, t2), Mathf.Min(t3, t4)), Mathf.Min(t5, t6));
        float tmax = Mathf.Min(Mathf.Min(Mathf.Max(t1, t2), Mathf.Max(t3, t4)), Mathf.Max(t5, t6));

        float t;
        if (tmax < 0)
        {
            t = tmax;
            return false;
        }

        if (tmin > tmax)
        {
            t = tmax;
            return false;
        }

        t = tmin;
        return true;
    }

    public bool CheckPositionInNode(CL_Node node, Vector3 position)
    {
        if (node.min.x > position.x || node.max.x < position.x)
            return false;

        if (node.min.y > position.y || node.max.y < position.y)
            return false;

        if (node.min.z > position.z || node.max.z < position.z)
            return false;

        return true;
    }

    public List<ClimbingLine> GetCurrentCheckClimbingLines(Vector3 position)
    {
        if (CheckPositionInNode(_rootNode, position) == false)
            return null;

        CL_Node currNode = _rootNode;
        while(true)
        {
            if (currNode.child[0] == null)
                break;

            for(int i = 0; i<4; i++)
            {
                if(CheckPositionInNode(currNode.child[i],position) == true)
                {
                    currNode = currNode.child[i];
                    break;
                }
            }
        }

        return currNode.climbingLines;
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_drawNode == false)
            return;

        if(_rootNode != null)
        {
            Gizmos.color = Color.yellow;

            Vector3 d1 = _rootNode.min;
            Vector3 d2 = new Vector3(_rootNode.min.x, _rootNode.min.y, _rootNode.max.z);
            Vector3 d3 = new Vector3(_rootNode.max.x, _rootNode.min.y, _rootNode.min.z);
            Vector3 d4 = new Vector3(_rootNode.max.x, _rootNode.min.y, _rootNode.max.z);
            Vector3 u1 = new Vector3(_rootNode.min.x, _rootNode.max.y, _rootNode.min.z);
            Vector3 u2 = new Vector3(_rootNode.min.x, _rootNode.max.y, _rootNode.max.z);
            Vector3 u3 = new Vector3(_rootNode.max.x, _rootNode.max.y, _rootNode.min.z);
            Vector3 u4 = _rootNode.max;

            Gizmos.DrawLine(d1, d3);
            Gizmos.DrawLine(d1, d2);
            Gizmos.DrawLine(d2, d4);
            Gizmos.DrawLine(d3, d4);

            Gizmos.DrawLine(u1, u3);
            Gizmos.DrawLine(u1, u2);
            Gizmos.DrawLine(u2, u4);
            Gizmos.DrawLine(u3, u4);

            Gizmos.DrawLine(d1, u1);
            Gizmos.DrawLine(d2, u2);
            Gizmos.DrawLine(d3, u3);
            Gizmos.DrawLine(d4, u4);

            DrawGizmoNode(_rootNode);
        }
    }

    private void DrawGizmoNode(CL_Node node)
    {
        node.DrawGizmos();

        if (node.child[0] != null)
        {
            DrawGizmoNode(node.child[0]);
            DrawGizmoNode(node.child[1]);
            DrawGizmoNode(node.child[2]);
            DrawGizmoNode(node.child[3]);
        }
    }

#endif
}
