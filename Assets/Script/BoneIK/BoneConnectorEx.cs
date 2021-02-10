using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneConnectorEx : MonoBehaviour
{
    public List<HingeEx> Hinges = new List<HingeEx>();
    public Transform LastPoint;

    public Transform Ik;

    // public void Awake()
    // {
    //     Initialize();
    // }

    // public void Update()
    // {
    //     Solve();
    // }

    // public void Initialize()
    // {

    // }

    // public void Solve()
    // {
    //     foreach(var hinge in Hinges)
    //     {
    //         Solve(hinge,Ik);
    //     }
    // }

    // public void Solve(HingeEx hinge, Transform point)
    // {
    //     var direction = (LastPoint.position - hinge.transform.position).normalized;
    //     var IkDirection = (point.position - hinge.transform.position).normalized;
    //     var up = hinge.GetAxisUpVector3();
        
    //     if(hinge.AxisMode != HingeEx.RotateAxisMode.Y_Only)
    //     {
    //         up *= hinge.Clamp360Degree(transform.localEulerAngles.y) >= 180f ? -1f : 1f;
    //     }

    //     var angle = Vector3.SignedAngle(direction,IkDirection,up);

    //     GizmoHelper.Instance.DrawLine(hinge.transform.position, hinge.transform.position + direction,Color.blue);
    //     GizmoHelper.Instance.DrawLine(hinge.transform.position, hinge.transform.position + IkDirection,Color.red);
    //     hinge.AddAngle(angle);
    // }
}