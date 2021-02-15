using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IKWalkTest : MonoBehaviour
{
    public LayerMask layer;
    public List<IKTrack> iks;
    public List<Transform> footRayPoints;
    public float rayHeight = 10f;
    public float rayDist = 20f;
    public float legDistance = 1f;

    private RayEx _ray;

    public void Start()
    {
        _ray = new RayEx(new Ray(Vector3.zero,Vector3.down),rayDist,layer);
    }   

    public void Update()
    {
        for(int i = 0; i < iks.Count; ++i)
        {
            if(_ray.Cast(footRayPoints[i].position,out RaycastHit hit))
            {
                var dist = Vector3.Distance(hit.point,iks[i].transform.position);
                if(dist >= legDistance)
                {
                    iks[i].SetTarget(hit.point);
                }

                GizmoHelper.Instance.DrawLine(hit.point,footRayPoints[i].transform.position,Color.red);
            }

        }
    }
}
