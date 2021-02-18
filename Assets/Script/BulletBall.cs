using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBall : MonoBehaviour
{
    public GameObject explosionEffect;
    public Vector3 direction;
    public float speed = 1f;

    private Vector3 moveFactor;

    void Update()
    {
        moveFactor = direction * speed * Time.deltaTime;

    }

    void FixedUpdate()
    {
        transform.position += moveFactor;
        moveFactor = Vector3.zero;
    }

    public void OnTriggerEnter(Collider coll)
    {
        PortalProgress portal = null;
        if(coll.gameObject.TryGetComponent<PortalProgress>(out portal))
        {
            Debug.Log("deleted");

            portal.WhenHit();

            Destroy(Instantiate(explosionEffect,transform.position,Quaternion.identity),3.5f);
            Destroy(this.gameObject);
        }

        if(coll.tag == "Enviroment" || coll.tag == "Player")
        {
            Destroy(Instantiate(explosionEffect,transform.position,Quaternion.identity),5f);
            Destroy(this.gameObject);
        }
    }
}
