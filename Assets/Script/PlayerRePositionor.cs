using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRePositionor : MonoBehaviour
{
    public Transform respawn;
    public void OnTriggerEnter(Collider coll)
    {
        if(coll.TryGetComponent<PlayerCtrl_Ver2>(out var ctrl))
        {
            ctrl.transform.position = respawn.position;
        }
    }
}
