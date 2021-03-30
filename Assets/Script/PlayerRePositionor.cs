using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRePositionor : MonoBehaviour
{
    public Transform respawn;
    public Transform bip;
    public void OnTriggerEnter(Collider coll)
    {        
        if(coll.TryGetComponent<PlayerCtrl_Ver2>(out var ctrl))
        {
            ctrl.transform.position = respawn.position;
            return;
        }
        
        
        {
            GameManager.Instance.player.transform.position = respawn.position;
            bip.position = respawn.position;
        }
        // else if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {

        // }
    }
}
