using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundRotator : MonoBehaviour
{
    public Vector3 axis;
    public float speed;
    public bool play = true;


    public void Update()
    {
        if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Fixed)
            return;

        if (GameManager.Instance.PAUSE == true)
            return;

        if (play)
        {
            transform.RotateAround(transform.position,axis,speed * Time.deltaTime);

        }

    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Update)
            return;

        if (GameManager.Instance.PAUSE == true)
            return;

        if (play)
        {
            transform.RotateAround(transform.position,axis,speed * Time.fixedDeltaTime);

        }

    }
}
