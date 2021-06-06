using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRePositionor : MonoBehaviour
{
    public LayerMask playerLayer;
    public UnityEvent whenFall;
    public UnityEvent beforeFall;
    public Transform respawn;
    public Transform bip;

    public void Start()
    {
        bip = GameManager.Instance.player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(playerLayer == (playerLayer | (1<<coll.gameObject.layer)))
        {
            beforeFall?.Invoke();
        }
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

        // else if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {

        // }
    }

    IEnumerator Defferd(Collider coll)
    {
        if (coll.TryGetComponent<PlayerCtrl_Ver2>(out var ctrl))
        {
            if (ctrl.Dead == true)
                yield break;

            ctrl.ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
            yield return new WaitForSeconds(1.0f);

            var rot = Quaternion.LookRotation(respawn.forward);

            ctrl.transform.position = respawn.position;
            ctrl.transform.SetPositionAndRotation(respawn.position,rot);
            GameManager.Instance.cameraManager.SetBrainCameraPosition(respawn.position);
            
            GameManager.Instance.followTarget.SetPitchYaw(rot.eulerAngles.x,rot.eulerAngles.y);
            ctrl.TakeDamage(5.0f,false);
            whenFall?.Invoke();
            yield break;
        }

        if(playerLayer == (playerLayer | (1<<coll.gameObject.layer)))
        {
            PlayerCtrl_Ver2 player = ((PlayerCtrl_Ver2)(GameManager.Instance.player));

            if (player.Dead == true)
                yield break;

            player.ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
            yield return new WaitForSeconds(1.0f);
            //GameManager.Instance.player.transform.position = respawn.position;
            //bip.position = respawn.position;
            var rot = Quaternion.LookRotation(respawn.forward);

            ((PlayerCtrl_Ver2)(GameManager.Instance.player)).transform.SetPositionAndRotation(respawn.position,rot);
            ((PlayerCtrl_Ver2)(GameManager.Instance.player)).TakeDamage(5.0f);
            GameManager.Instance.cameraManager.SetBrainCameraPosition(respawn.position);
            
            GameManager.Instance.followTarget.SetPitchYaw(rot.eulerAngles.x,rot.eulerAngles.y);
            whenFall?.Invoke();
        }
    }
}
