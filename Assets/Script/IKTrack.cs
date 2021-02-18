using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTrack : MonoBehaviour
{
    public Vector3 targetPoint;
    public float lerpFactor = .2f;
    public float height = 1f;

    private Vector3 prevFootPoint;
    private float distance = 0;

    public void SetTarget(Vector3 pos)
    {
        prevFootPoint = transform.position;

        distance = Vector3.Distance(prevFootPoint,pos);
        targetPoint = pos;
    }

    public void Start()
    {
        targetPoint = transform.position;
    }

    void Update()
    {
        var position = Vector3.Lerp(transform.position,targetPoint,lerpFactor);

        var currDist = Vector3.Distance(transform.position,targetPoint);
        //position.y = height * Mathf.Sin((distance - (currDist / distance)) * Mathf.PI);

        transform.position = position;
    }
}
