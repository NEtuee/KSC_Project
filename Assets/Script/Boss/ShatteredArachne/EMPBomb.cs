using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPBomb : MonoBehaviour
{
    // public EMPShield shield;
    // public Rigidbody rig;
    // public LayerMask targetLayer;
    // public float explosionRadius = 2f;
    // public float explosionForce = 300f;

    // public bool destroy = false;

    // private float _speed = 0f;

    // public void Hit()
    // {
    //     gameObject.SetActive(false);
    //     Collider[] playerColl = Physics.OverlapSphere(transform.position, explosionRadius,targetLayer);

    //     if(destroy)
    //         Destroy(this.gameObject);

    //     if(playerColl.Length != 0)
    //     {
    //         foreach(Collider curr in playerColl)
    //         {
    //             PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
    //             if(ragdoll != null)
    //             {
    //                 ragdoll.ExplosionRagdoll(explosionForce, (ragdoll.transform.position - transform.position).normalized);
    //             }
    //             else
    //             {
    //                 if(curr.TryGetComponent<MiniSpider_AI>(out var ai))
    //                 {
    //                     ai.ChangeState(MiniSpider_AI.State.Dead);
    //                     ai.body.AddForce((ai.transform.position - transform.position).normalized * 50f,ForceMode.Impulse);
    //                 }
    //                 else if(curr.TryGetComponent<ShatteredArachne_AI>(out var arac))
    //                 {
    //                     arac.Hit();
    //                 }
    //             }
    //         }
    //     }

    // }

    // public void FixedUpdate()
    // {
    //     _speed = rig.velocity.magnitude;
    // }

    // public void OnCollisionEnter(Collision coll)
    // {
    //     if(_speed >= 20f)
    //     {
    //         shield.isActive = true;
    //         shield.Hit();
    //         //Hit();
    //     }
    // }
}
