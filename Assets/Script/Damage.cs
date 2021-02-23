using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public ExplosionTest explosion;
    public SphereCollider col;
    public float factor = 10f;
    public float power = 750f;

    public void Start()
    {
        Destroy(this.gameObject,.1f);
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player")
        {
            if(TryGetComponent<TestHPScript>(out var con))
            {
                con.displayHp -= factor;
            }

            explosion.Exlposion(transform.position,col.radius,power);

            Destroy(this.gameObject);
        }
    }
}
