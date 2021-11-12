    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundRotator : MonoBehaviour
{
    public Vector3 axis;
    public float speed;
    public bool play = true;



    public void FixedUpdate()
    {
        if (play)
        {
            transform.RotateAround(transform.position,axis,speed * Time.fixedDeltaTime);

        }

    }
}
