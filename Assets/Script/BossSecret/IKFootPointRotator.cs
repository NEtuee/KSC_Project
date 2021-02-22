using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IKFootPointRotator : MonoBehaviour
{
    public Transform rayPoint;

    public Transform leftFootPoint;
    public Transform rightFootPoint;

    public LayerMask layerMask;

    public float rayDistance = 10f;
    public float baseHeight = 1f;
    public float weight = 1f;

    public bool rotation = true;

    private RayEx ray;

    private void Start()
    {
        ray = new RayEx(new Ray(Vector3.zero,Vector3.zero),rayDistance,layerMask);
    }

    void LateUpdate()
    {
        var down = -transform.up;

        ray.SetDirection(down);

        Vector3 leftNormal = Vector3.zero;
        Vector3 rightNormal = Vector3.zero;
        if(ray.Cast(leftFootPoint.position,out RaycastHit hit))
        {
            leftNormal = hit.normal;

            GizmoHelper.Instance.DrawLine(hit.point, hit.point + hit.normal * 3f,Color.red);
        }
        if(ray.Cast(rightFootPoint.position,out hit))
        {
            rightNormal = hit.normal;

            GizmoHelper.Instance.DrawLine(hit.point, hit.point + hit.normal * 3f,Color.red);
        }

        if(ray.Cast(transform.position,out hit))
        {
            var point = hit.point + (-down * baseHeight);
            transform.position = point;
        }

        var avg = (leftNormal + rightNormal).normalized;

        if(rotation)
            transform.rotation = Quaternion.Lerp(transform.rotation,(Quaternion.FromToRotation(transform.up,avg) * transform.rotation),0.2f);

        
    }
}
