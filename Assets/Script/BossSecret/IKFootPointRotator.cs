using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IKFootPointRotator : MonoBehaviour
{
    public List<Transform> footPoints;
    public List<IKLegMovement> legs;

    public LayerMask layerMask;

    public float rayDistance = 10f;
    public float baseHeight = 1f;
    public float rotateFactor = 0.07f;

    public bool rotation = true;

    private RayEx ray;

    private void Start()
    {
        ray = new RayEx(new Ray(Vector3.zero,Vector3.zero),rayDistance,layerMask);
    }

    void FixedUpdate()
    {
        var down = -transform.up;

        ray.SetDirection(down);

        Vector3 normals = Vector3.zero;
        RaycastHit hit;

        foreach(var point in footPoints)
        {
            if(ray.Cast(point.position,out hit))
            {
                normals += hit.normal;
            }
        }

        if(ray.Cast(transform.position,out hit))
        {
            var point = hit.point + (-down * baseHeight);
            transform.position = point;
        }

        var avg = (normals).normalized;

        if(rotation)
            transform.rotation = Quaternion.Lerp(transform.rotation,(Quaternion.FromToRotation(transform.up,avg) * transform.rotation),rotateFactor);

        
    }

    public void IKUnHold()
    {
        foreach(var leg in legs)
        {
            leg.Hold(false);
        }
    }

    public void IKHold()
    {
        foreach(var leg in legs)
        {
            leg.Hold(true);
        }
    }
}
