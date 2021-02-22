using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    public List<Transform> allParts = new List<Transform>();

    public float farDistance = 5f;

    void Update()
    {
        for(int i = 1; i < allParts.Count; ++i)
        {
            var one = allParts[i - 1].transform.position;
            var two = allParts[i].transform.position;
            var direction = (one - two).normalized;
            var dist = Vector3.Distance(one,two);

            if(dist >= farDistance)
            {
                allParts[i].transform.position = one - direction * farDistance;
            }

            var angle = Vector3.SignedAngle(allParts[i].forward,Vector3.ProjectOnPlane(direction,allParts[i].up),allParts[i].up);
            allParts[i].RotateAround(allParts[i].position,allParts[i].up,angle);
        }
    }
}
