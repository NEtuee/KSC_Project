using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float speed = 5f;
    public float turn = 10f;
    void Update()
    {
        if(Input.GetKey(KeyCode.I))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.J))
        {
            // var euler = transform.eulerAngles;
            // euler.y -= turn * Time.deltaTime;
            // transform.eulerAngles = euler;

            transform.RotateAround(transform.position,transform.up,-turn * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.K))
        {
            // var euler = transform.eulerAngles;
            // euler.y += turn * Time.deltaTime;
            // transform.eulerAngles = euler;

            transform.RotateAround(transform.position,transform.up,turn * Time.deltaTime);
        }
    }
}
