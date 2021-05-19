using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_PlayerShockEvent : MonoBehaviour
{
    public float shockTime = 1f;
    public LayerMask playerLayer;
    public bool progress = true;

    private float _timer = 0f;

    public void Update()
    {
        if (!progress)
            return;

        _timer += Time.deltaTime;
        if(_timer >= .5f)
        {
            GameManager.Instance.effectManager.Active("ElectricSpark",transform.position,Quaternion.identity);
            _timer = 0f;
        }
        
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (((1 << transform.GetChild(i).gameObject.layer) & playerLayer.value) != 0)
            {
                var ragdoll = GameManager.Instance.player.GetComponent<PlayerRagdoll>();
                GameManager.Instance.effectManager.Active("ElectricSpark",ragdoll.transform.position,Quaternion.identity);
                ragdoll.SetPlayerShock(shockTime);
                ragdoll.ExplosionRagdoll(100f,(ragdoll.transform.position - transform.position).normalized);
            }
        }
    }

    // public void OnCollisionEnter(Collision other)
    // {
    //     Debug.Log(other.gameObject.name);
    //     if ((other.gameObject.layer & playerLayer.value) != 0)
    //     {
    //         Debug.Log(other.gameObject.name);
    //         var ragdoll = GameManager.Instance.player.GetComponent<PlayerRagdoll>();
    //         ragdoll.SetPlayerShock(shockTime);
    //         ragdoll.FlyRagdoll();
    //     }
    // }
}
