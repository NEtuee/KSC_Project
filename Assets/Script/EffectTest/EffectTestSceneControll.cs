using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTestSceneControll : MonoBehaviour
{
    public PlayerCtrl_Ver2 player;
    public EMPGun gun;
    public Transform explosionPoint;

    void Start()
    {
        StartCoroutine(LateStart());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            gun.LaunchLaser(40.0f);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            gun.LaunchLaser(80.0f);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            gun.LaunchLaser(120.0f);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameManager.Instance.effectManager.Active("CannonShotExplosion", explosionPoint.position, Quaternion.identity);
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Aiming);
        player.ActiveAim(true);
    }
}
