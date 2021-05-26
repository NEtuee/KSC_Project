using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEdit_PlayerShockEvent : MonoBehaviour
{
    public GameObject[] electric;
    public float shockTime = 1f;
    public float damage = 5f;
    public LayerMask playerLayer;
    public bool progress = true;

    private float _timer = 0f;

    public UnityEvent whenPlayerShock;

    public void Update()
    {
        foreach (var elec in electric)
        {
            elec.SetActive(progress);
        }
        
        if (!progress)
            return;

        _timer += Time.deltaTime;
        if(_timer >= .5f)
        {
            //GameManager.Instance.effectManager.Active("ElectricSpark",transform.position,Quaternion.identity);
            _timer = 0f;
        }
        
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (((1 << transform.GetChild(i).gameObject.layer) & playerLayer.value) != 0)
            {
                transform.GetChild(i).SetParent(null);
                var ragdoll = GameManager.Instance.player.GetComponent<PlayerRagdoll>();
                GameManager.Instance.effectManager.Active("ElectricSpark",ragdoll.transform.position,Quaternion.identity);
                GameManager.Instance.player.TakeDamage(damage);
                ragdoll.SetPlayerShock(shockTime);
                ragdoll.ExplosionRagdoll(100f,(ragdoll.transform.position - transform.position).normalized);
                whenPlayerShock?.Invoke();
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
