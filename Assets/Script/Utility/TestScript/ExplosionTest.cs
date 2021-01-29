using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTest : MonoBehaviour
{
    [SerializeField]private GameObject explosionPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            Exlposion();
        }

    }

    private void Exlposion()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Collider[] coll = Physics.OverlapSphere(transform.position, 10.0f);
        
        if(coll.Length != 0)
        {
            for(int i = 0; i < coll.Length; i++)
            {
                PlayerRagdoll ragdoll = coll[i].GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(750.0f, transform.position, 10000.0f);
                }
            }
        }
    }
}
