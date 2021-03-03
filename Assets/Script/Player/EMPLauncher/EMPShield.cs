using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPShield : MonoBehaviour
{
    public GameObject destroyEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("EMP"))
        {
            Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
            Destroy(gameObject);
        }
    }
}
