using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_GroundFix : MonoBehaviour
{
    [SerializeField]private Transform rayStartTransform;
    [SerializeField]private Transform footTransform;
    [SerializeField]private LayerMask groundLayer;

    private SphereRayEx ray;
    private Vector3 groundDiff;

    void Start()
    {
        ray = new SphereRayEx(new Ray(Vector3.zero,Vector3.down),100f,.3f,groundLayer);
        groundDiff = transform.localPosition - footTransform.localPosition;

    }

    void LateUpdate()
    {
        FixGroundPos();
    }

    private void FixGroundPos()
    {
        RaycastHit hit;
        if(ray.Cast(rayStartTransform.position,out hit))
        {
            var pos = transform.localPosition;
            pos.y = hit.point.y + groundDiff.y;
            transform.localPosition = pos;
        }
    }
}
