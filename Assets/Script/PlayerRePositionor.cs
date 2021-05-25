using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRePositionor : MonoBehaviour
{
    public UnityEvent whenFall;
    public Transform respawn;
    public Transform bip;

    public void Start()
    {
        bip = GameManager.Instance.player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
    }

    public void OnTriggerEnter(Collider coll)
    {        
        if(coll.TryGetComponent<PlayerCtrl_Ver2>(out var ctrl))
        {
            ctrl.transform.position = respawn.position;
            ctrl.TakeDamage(5.0f);
            whenFall.Invoke();
            return;
        }
        
        {
            GameManager.Instance.player.transform.position = respawn.position;
            bip.position = respawn.position;
        }

        whenFall?.Invoke();

        // else if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {

        // }
    }
}
