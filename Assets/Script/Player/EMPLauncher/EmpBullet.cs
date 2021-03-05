using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpBullet : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private GameObject hitEffect;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity),2.0f);
        Destroy(this.gameObject);
    }
}
