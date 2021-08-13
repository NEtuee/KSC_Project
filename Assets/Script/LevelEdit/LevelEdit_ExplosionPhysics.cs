using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_ExplosionPhysics : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public LayerMask changeLayer;

    public float force = 100f;
    public float torque = 100f;

    public void Launch()
    {
        foreach(var item in targets)
        {
            var collider = item.GetComponent<Collider>();
            if(collider != null)
                collider.enabled = false;

            var rig = item.AddComponent<Rigidbody>();
            var dir = (item.transform.position - transform.position).normalized;
            rig.AddForce(dir * force);
            rig.AddTorque(MathEx.RandomCircle(1f).normalized * torque);
            //item.layer = 1 << changeLayer.value;
        }
    }
}
