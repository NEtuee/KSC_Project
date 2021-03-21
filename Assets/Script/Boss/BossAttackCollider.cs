using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackCollider : MonoBehaviour
{
    public BossCtrl boss;

    private void OnCollisionEnter(Collision collision)
    {
        if (boss.state == BossCtrl.BossState.Rush || boss.state == BossCtrl.BossState.Turn)
        {
            if(collision.collider.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerRagdoll>().
                    ExplosionRagdoll(200.0f, (collision.transform.position- transform.position + Vector3.up).normalized);
            }
        }
    }
}
