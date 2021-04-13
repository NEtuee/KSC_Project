using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieBomb : MonoBehaviour
{
    public void OnCollisionEnter(Collision coll)
    {
        GameManager.Instance.effectManager.Active("BigExplosion",transform.position,transform.rotation);

        if(coll.transform.TryGetComponent<BoogieHead>(out var head))
        {
            head.Hit();
        }

        Destroy(this.gameObject);
    }
}
