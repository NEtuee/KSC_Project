using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLookAtCtrl : MonoBehaviour
{
    private Transform mainCam;
    private RaycastHit hit;
    private Vector3 targetPos;
    private void Awake()
    {
        mainCam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if(Physics.Raycast(mainCam.position, mainCam.forward, out hit, 100f))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = mainCam.position + mainCam.forward * 100.0f;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, 100.0f * Time.deltaTime);
    }
}
