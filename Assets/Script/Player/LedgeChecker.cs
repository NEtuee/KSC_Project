using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    [SerializeField] private LedgeCollider collider1;
    [SerializeField] private LedgeCollider collider2;
    [SerializeField] private bool isDetectLedge;

    private void FixedUpdate()
    {
        foreach(GameObject obj in collider1.collidedObjects)
        {
            if(collider2.collidedObjects.Contains(obj) == false)
            {
                isDetectLedge = true;
                break;
            }
            else
            {
                isDetectLedge = false;
            }
        }

        if(collider1.collidedObjects.Count == 0)
        {
            isDetectLedge = true;
        }
    }

    public bool IsDetectedLedge()
    {
        return isDetectLedge;
    }
}

