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
            transform.rotation = targetBone.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Inverse(targetBone.rotation);
        }
    }
}
