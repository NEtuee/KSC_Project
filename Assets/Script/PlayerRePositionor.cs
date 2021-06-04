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
        //if(coll.TryGetComponent<PlayerCtrl_Ver2>(out var ctrl))
        //{
        //    ctrl.transform.position = respawn.position;
        //    ctrl.TakeDamage(5.0f);
        //    ctrl.ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
        //    whenFall.Invoke();
        //    return;
        //}

        //{
        //    GameManager.Instance.player.transform.position = respawn.position;
        //    bip.position = respawn.position;
        //}
        StartCoroutine(Defferd(coll));

        whenFall?.Invoke();

        // else if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {

        // }
    }

    IEnumerator Defferd(Collider coll)
    {
        if (coll.TryGetComponent<PlayerCtrl_Ver2>(out var ctrl))
        {
            ctrl.ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
            yield return new WaitForSeconds(1.0f);
            ctrl.transform.position = respawn.position;
            ctrl.TakeDamage(5.0f,false);
            whenFall.Invoke();
            yield break;
        }

        if(ctrl == null)
        {
            ((PlayerCtrl_Ver2)(GameManager.Instance.player)).ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
            yield return new WaitForSeconds(1.0f);
            //GameManager.Instance.player.transform.position = respawn.position;
            //bip.position = respawn.position;
            ((PlayerCtrl_Ver2)(GameManager.Instance.player)).transform.position = respawn.position;
            ((PlayerCtrl_Ver2)(GameManager.Instance.player)).TakeDamage(5.0f);
            whenFall.Invoke();
        }
    }
}
