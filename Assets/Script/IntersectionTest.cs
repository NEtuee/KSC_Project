using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTest : MonoBehaviour
{
    public Transform p1, p2;
    public Transform p3, p4;

    public Transform capsuleP1, capsuleP2;
    public float radius = 1f;
    public bool overlap = false;

    public float s;
    public float t;

    public float minDist;
    public float roundDist;

    private void Update()
    {
        minDist = Intersection.DistanceLineAndLine(p1.position, p2.position, p3.position, p4.position,out s,out t);
        roundDist = Mathf.Round(minDist);

        overlap = Intersection.IntersectionCapsuleAndLine(capsuleP1.position, capsuleP2.position, radius, p1.position, p2.position);
    }

    private void OnDrawGizmos()
    {
        if(p1 != null && p2 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(p1.position, p2.position);
        }

        if (p3 != null && p4 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p3.position, p4.position);
        }

        if(capsuleP1!= null && capsuleP2 != null)
        {
            Gizmos.color = Color.green;
            if (overlap == true)
                Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(capsuleP1.position, radius);
            Gizmos.DrawWireSphere(capsuleP2.position, radius);
            Gizmos.DrawLine(capsuleP1.position, capsuleP2.position);
        }
    }
}
