using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieBomb : MonoBehaviour
{
    public Collider coll;
    public Rigidbody rig;
    
    public void OnCollisionEnter(Collision coll)
    {
        GameManager.Instance.effectManager.Active("CannonExplosion",transform.position,transform.rotation);

        if(coll.transform.TryGetComponent<BoogieHead>(out var head))
        {
            head.Hit();
        }

        gameObject.SetActive(false);
        //Destroy(this.gameObject);
    }
}
