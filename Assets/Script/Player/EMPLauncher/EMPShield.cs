using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPShield : MonoBehaviour
{
    public GameObject destroyEffect;
    private float hp = 100f;
    private float shakeTime = 0.0f;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(shakeTime > 0.0f)
        {
            transform.position = (Vector3)Random.insideUnitCircle*0.2f + startPos;
            shakeTime -= Time.deltaTime;
            if(shakeTime <= 0.0f)
            {
                transform.position = startPos;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("EMP"))
        {
            Hit();
        }
    }

    public void Hit()
    {
        hp -= 100f;
        shakeTime = 0.1f;
        if (hp <= 0f)
        {
            Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
            Destroy(gameObject);
        }
    }
}
