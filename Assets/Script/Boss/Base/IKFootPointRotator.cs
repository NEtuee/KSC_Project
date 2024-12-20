using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IKFootPointRotator : MonoBehaviour
{
    public List<Transform> footPoints;
    public List<IKLegMovement> legs;

    public LayerMask layerMask;
    public Vector3 rayOffset = Vector3.zero;
    public bool rayPointCast = false;

    public float rayDistance = 10f;
    public float baseHeight = 1f;
    public float rotateFactor = 0.07f;

    public bool rotation = true;
    public bool setParentToGround = false;
    public bool sphereRay = false;
    public bool rotateToRay = false;

    private RayEx ray;

    private void Start()
    {
        if(sphereRay)
            ray = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),rayDistance,0.3f,layerMask);
        else
            ray = new RayEx(new Ray(Vector3.zero,Vector3.zero),rayDistance,layerMask);
    }

    void FixedUpdate()
    {
        var down = -transform.up;

        ray.SetDirection(down);

        Vector3 normals = Vector3.zero;
        var pos = Vector3.zero;
        int hitCount = 0;
        RaycastHit hit;

        foreach(var point in footPoints)
        {
            if(rayPointCast)
            {
                ray.SetDirection(-point.up);
            }
            if(ray.Cast(point.position,out hit))
            {
                normals += hit.normal;
                pos += hit.point;
                ++hitCount;
            }
        }

        if(setParentToGround)
        {
            if(ray.Cast(transform.position + rayOffset,out hit))
            {
                transform.SetParent(hit.transform);
            }
        }

        if(!rayPointCast)
        {
            if(ray.Cast(transform.position + rayOffset,out hit))
            {
                Debug.DrawLine(transform.position,hit.point,Color.red);
                var point = hit.point + (-down * baseHeight);
                transform.position = point;

                if(rotation && rotateToRay)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, (Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation),rotateFactor);
                }
            }
        }
        else
        {
            transform.position = pos / hitCount + (-down * baseHeight);
        }
        

        var avg = (normals).normalized;

        if(rotation && !rotateToRay)
            transform.rotation = Quaternion.Lerp(transform.rotation,(Quaternion.FromToRotation(transform.up,avg) * transform.rotation),rotateFactor);

        
    }

    public void DisableAllLegs()
    {
        foreach(var leg in legs)
        {
            leg.iKFabric.enabled = false;
        }
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
