using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCore : MonoBehaviour
{
    public Transform absorbTransform;
    private SphereCollider trigger;
    void Start()
    {
        trigger = GetComponent<SphereCollider>();
    }

    public void Over()
    {
        if(trigger != null)
        {
            trigger.enabled = false;
            this.gameObject.tag = "Untagged";
        }
    }
}
