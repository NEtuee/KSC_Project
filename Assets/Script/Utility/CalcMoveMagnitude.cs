using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcMoveMagnitude : MonoBehaviour
{
    private float moveDist = 0f;
    private Vector3 prevPos;
    void Start()
    {
        prevPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveDist = Vector3.Distance(transform.position, prevPos) * 15f;
        prevPos = transform.position;
    }

    public float GetMoveGap()
    {
        return moveDist;
    }
}
