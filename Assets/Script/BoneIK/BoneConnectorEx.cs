using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoneConnectorEx : MonoBehaviour
{
    public List<HingeEx> Hinges = new List<HingeEx>();
    public Transform LastPoint;

    public Transform Ik;

    public void Awake()
    {
        Initialize();
    }

    public void Update()
    {
        Solve();
    }

    public void Initialize()
    {

    }

    public void Solve()
    {
        foreach(var hinge in Hinges)
        {
            Solve(hinge,Ik);
        }
    }

    public void Solve(HingeEx hinge, Transform point)
    {
        var direction = (LastPoint.position - hinge.transform.position).normalized;
        var IkDirection = (point.position - hinge.transform.position).normalized;

        var look = Quaternion.FromToRotation(direction,IkDirection);
        var angle = hinge.transform.rotation;

        if(MathEx.abs(Mathf.DeltaAngle(0f,angle.eulerAngles.y)) >= 90f)
        {
            look = Quaternion.Inverse(look);
        }

        hinge.transform.rotation = angle * look;
    }

#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        foreach(var hinge in Hinges)
        {
            var direction = (LastPoint.position - hinge.transform.position).normalized;
            var IkDirection = (Ik.position - hinge.transform.position).normalized;

            Handles.color = Color.blue;
            Handles.DrawLine(hinge.transform.position, LastPoint.position);
            Handles.color = Color.red;
            Handles.DrawLine(hinge.transform.position, hinge.transform.position + Vector3.Distance(Ik.position, hinge.transform.position) * IkDirection);
        }

    }
#endif
}