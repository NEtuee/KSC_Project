using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeCollider : MonoBehaviour
{
    public List<GameObject> collidedObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(collidedObjects.Contains(other.gameObject) == false)
        {
            collidedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidedObjects.Contains(other.gameObject) == true)
        {
            collidedObjects.Remove(other.gameObject);
        }
    }
}
