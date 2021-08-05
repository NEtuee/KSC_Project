using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MD;

public class PlayerRePositionor : UnTransfromObjectBase
{
    public LayerMask playerLayer;
    public UnityEvent whenFall;
    public UnityEvent beforeFall;
    public Transform respawn;
    public Transform bip;

    private PlayerCtrl_Ver2 _player;

    protected override void Awake()
    {
        base.Awake();
        RegisterRequest(GetSavedNumber("ObjectManager"));

        AddAction(MessageTitles.set_setplayer, (msg) =>
        {
            _player = (PlayerCtrl_Ver2)msg.data;
        });
    }

    protected override void Start()
    {
        base.Start();
        //bip = GameManager.Instance.player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
        bip = _player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
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
            if (ctrl.Dead == true || ctrl.GetState() == PlayerCtrl_Ver2.PlayerState.Respawn)
                yield break;

            ctrl.ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
            yield return new WaitForSeconds(1.0f);

            var rot = Quaternion.LookRotation(respawn.forward);

            ctrl.transform.position = respawn.position;
            ctrl.transform.SetPositionAndRotation(respawn.position,rot);
            //GameManager.Instance.cameraManager.SetBrainCameraPosition(respawn.position);
            Vector3Data position = MessageDataPooling.GetMessageData<Vector3Data>();
            position.value = respawn.position;
            SendMessageEx(MessageTitles.cameramanager_setBrainCameraPosition, GetSavedNumber("CameraManager"), position);

            //GameManager.Instance.followTarget.SetPitchYaw(rot.eulerAngles.x,rot.eulerAngles.y);
            PitchYawData data = MessageDataPooling.GetMessageData<PitchYawData>();
            data.pitch = rot.eulerAngles.x; data.yaw = rot.eulerAngles.y;
            SendMessageEx(MessageTitles.cameramanager_setYawPitch, GetSavedNumber("CameraManager"), data);
            ctrl.TakeDamage(5.0f,false);
            whenFall?.Invoke();
            yield break;
        }

        if(playerLayer == (playerLayer | (1<<coll.gameObject.layer)))
        {
            //PlayerCtrl_Ver2 player = ((PlayerCtrl_Ver2)(GameManager.Instance.player));

            if (_player.Dead == true || _player.GetState() == PlayerCtrl_Ver2.PlayerState.Respawn)
                yield break;

            _player.ChangeState(PlayerCtrl_Ver2.PlayerState.Respawn);
            yield return new WaitForSeconds(1.0f);
            //GameManager.Instance.player.transform.position = respawn.position;
            //bip.position = respawn.position;
            var rot = Quaternion.LookRotation(respawn.forward);

            _player.transform.SetPositionAndRotation(respawn.position,rot);
            _player.TakeDamage(5.0f);
            Vector3Data position = MessageDataPooling.GetMessageData<Vector3Data>();
            position.value = respawn.position;
            SendMessageEx(MessageTitles.cameramanager_setBrainCameraPosition, GetSavedNumber("CameraManager"), position);
            PitchYawData data = MessageDataPooling.GetMessageData<PitchYawData>();
            data.pitch = rot.eulerAngles.x; data.yaw = rot.eulerAngles.y;
            SendMessageEx(MessageTitles.cameramanager_setYawPitch, GetSavedNumber("CameraManager"), data);
            whenFall?.Invoke();
        }
    }
}
