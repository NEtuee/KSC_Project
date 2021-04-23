using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombRepositioner : MonoBehaviour
{
    public Transform position;

    public void OnTriggerEnter(Collider coll)
    {
        Debug.Log("Tkqlfk");
        coll.transform.position = position.position;
    }
}
