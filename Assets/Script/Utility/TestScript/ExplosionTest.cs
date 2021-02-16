using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTest : MonoBehaviour
{
    [SerializeField]private GameObject explosionPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Exlposion(transform.position);
        }

    }

    public void Exlposion(Vector3 position)
    {
        Instantiate(explosionPrefab, position, Quaternion.identity);
        Collider[] coll = Physics.OverlapSphere(position, 30.0f);
        
        if(coll.Length != 0)
        {
            for(int i = 0; i < coll.Length; i++)
            {
                PlayerRagdoll ragdoll = coll[i].GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(750.0f, position, 10000.0f);
                }
            }
        }
    }
}
