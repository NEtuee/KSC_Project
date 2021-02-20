 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float speed = 5f;
    public float turn = 10f;
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            var euler = transform.eulerAngles;
            euler.y -= turn * Time.deltaTime;
            transform.eulerAngles = euler;
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            var euler = transform.eulerAngles;
            euler.y += turn * Time.deltaTime;
            transform.eulerAngles = euler;
        }
    }
}
