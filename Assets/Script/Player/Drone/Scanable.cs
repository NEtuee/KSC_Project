using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scanable : MonoBehaviour
{
    [SerializeField] protected LayerMask visibleCastLayer;
    [SerializeField]protected Renderer renderer;
    protected Collider collider;

    public bool visible;
    public bool removeCheck = true;
    public abstract void Scanned();

    protected bool _isTriggered = false;
    
    protected void Start()
    {
        renderer = GetComponent<Renderer>();
        collider = GetComponent<Collider>();
    }

    public bool IsTriggered()
    {
        return _isTriggered;
    }

    public bool CheckInAngle(Vector3 scanPos)
    {
        if(renderer == null)
            return true;
        Vector3 cameraPosition = scanPos;
        Bounds bound = collider.bounds;
        Vector3 extents = bound.extents;

        Vector3 point;

        //1
        point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //2
        point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //3
        point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //4
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //5
        point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //6
        point = bound.center + new Vector3(extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //7
        point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //8
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }

        return false;
    }
}
