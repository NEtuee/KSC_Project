using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyBoneRotation : MonoBehaviour
{
    public bool active;
    public bool mirror;
    public Transform targetBone;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (active == false)
            return;

        if (mirror == false)
        {
            //transform.localRotation = targetBone.localRotation;
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, targetBone.localRotation, Time.deltaTime * 10.0f);
            //transform.rotation = targetBone.rotation;
            //transform.localPosition = targetBone.localPosition;
            transform.SetPositionAndRotation(targetBone.position,targetBone.rotation);
        }
        else
        {
            //transform.localRotation = Quaternion.Inverse(targetBone.localRotation);
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Inverse(targetBone.localRotation), Time.deltaTime * 10.0f);
            //transform.localPosition = targetBone.localPosition;
            transform.SetPositionAndRotation(targetBone.position,targetBone.rotation);

        }
    }
}
