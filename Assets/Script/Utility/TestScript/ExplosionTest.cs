using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTest : MonoBehaviour
{
    [SerializeField]private GameObject explosionPrefab;

    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.B))
        // {
        //     Exlposion();
        // }

    }

    public void Exlposion(Vector3 position, float radius = 30f, float power = 750f)
    {
        if(explosionPrefab != null)
            Instantiate(explosionPrefab, position, Quaternion.identity);
        Collider[] coll = Physics.OverlapSphere(position, radius);
        
        if(coll.Length != 0)
        {
            for(int i = 0; i < coll.Length; i++)
            {
                PlayerRagdoll ragdoll = coll[i].GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    Debug.Log("?");
                    ragdoll.ExplosionRagdoll(750.0f, position, 10000.0f);
                }
            }
        }
    }
}
