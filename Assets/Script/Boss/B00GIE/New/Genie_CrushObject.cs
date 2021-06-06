using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_CrushObject : MonoBehaviour
{
    public List<Rigidbody> rigidbodies;

    public List<DitzelGames.FastIK.FastIKFabric> ikObjects;

    public float power;

    public LayerMask changeLayer;

    public void Explosion()
    {
        foreach(var ik in ikObjects)
        {
            ik.enabled = false;
        }
        
        foreach(var rig in rigidbodies)
        {
            rig.transform.SetParent(null);
            rig.gameObject.layer = 1 << changeLayer.value;
            rig.isKinematic = false;
            rig.useGravity = true;
            if(rig.TryGetComponent<MeshCollider>(out var coll))
            {
                coll.convex = true;
            }

            var dir = (rig.transform.position - transform.position).normalized;
            rig.AddForce(dir * power,ForceMode.Force);
        }
    }

    public void Explosion(Vector3 center)
    {
        foreach(var ik in ikObjects)
        {
            ik.enabled = false;
        }

        foreach(var rig in rigidbodies)
        {
            rig.transform.SetParent(null);
            rig.gameObject.layer = 1 << changeLayer.value;
            rig.isKinematic = false;
            rig.useGravity = true;
            if(rig.TryGetComponent<MeshCollider>(out var coll))
            {
                coll.convex = true;
            }

            var dir = (rig.transform.position - center).normalized;
            rig.AddForce(dir * power,ForceMode.Impulse);
        }
    }
}
