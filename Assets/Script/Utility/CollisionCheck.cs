using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Stay : "+collision.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter : " +collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit : " + collision.gameObject);
    }
}
