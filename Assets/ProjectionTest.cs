using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionTest : MonoBehaviour
{
    RayEx ray;

    public void Start()
    {
        ray = new RayEx(new Ray(Vector3.zero,Vector3.down),10f,~0);
    }

    public void Update()
    {
        if(ray.Cast(transform.position,out RaycastHit hit))
        {
            GizmoHelper.Instance.DrawLine(transform.position, hit.point,Color.red);
            GizmoHelper.Instance.DrawLine(hit.point,hit.point + hit.normal * 3f,Color.blue);
            GizmoHelper.Instance.DrawLine(transform.position,transform.position + transform.forward * 3f,Color.blue);
            GizmoHelper.Instance.DrawLine(hit.point,hit.point + Vector3.ProjectOnPlane(transform.forward,hit.normal),Color.green);
        }
    }
}
