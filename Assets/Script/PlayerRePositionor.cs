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

    private PlayerUnit _player;
    private Collider collider;

    private bool _active;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    protected override void Awake()
    {
        base.Awake();
        RegisterRequest(GetSavedNumber("ObjectManager"));

        collider = GetComponent<Collider>();
        //collider.enabled = false;

        AddAction(MessageTitles.set_setplayer, (msg) =>
        {
            _player = (PlayerUnit)msg.data;
        });

        AddAction(MessageTitles.scene_beforeSceneChange, (msg) =>
        {
            if (collider != null)
                collider.enabled = false;
        });

        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
        {
            if (collider != null)
                collider.enabled = true;
        
        });
    }

    //protected override void Start()
    //{
    //    base.Start();
    //    //bip = GameManager.Instance.player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
    //    SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    //    bip = _player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
    //}

    public override void Initialize()
    {
        base.Initialize();

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
        bip = _player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips);

        _timeCounter.InitTimer("Check",0,3f);
        
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Check", out var limit, deltaTime);
        _active = limit;
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(!_active)
            return;

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
        Debug.Log("RespawnColl");
        StartCoroutine(Defferd(coll));

        // else if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {

        // }
    }

    IEnumerator Defferd(Collider coll)
    {
        if (coll.TryGetComponent<PlayerUnit>(out var ctrl))
        {
            ctrl.TakeDamage(5.0f, false);

            if (ctrl.GetState == PlayerUnit.deadState || ctrl.GetState == PlayerUnit.respawnState)
                yield break;

            ctrl.ChangeState(PlayerUnit.respawnState);
            yield return new WaitForSeconds(1.0f);

            var rot = Quaternion.LookRotation(respawn.forward);

            var respawnData = MessageDataPooling.GetMessageData<MD.PositionRotation>();
            respawnData.position = respawn.position;
            respawnData.rotation = rot;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), respawnData);
            Vector3Data position = MessageDataPooling.GetMessageData<Vector3Data>();
            position.value = respawn.position;
            SendMessageEx(MessageTitles.cameramanager_setBrainCameraPosition, GetSavedNumber("CameraManager"), position);

            PitchYawData data = MessageDataPooling.GetMessageData<PitchYawData>();
            data.pitch = rot.eulerAngles.x; data.yaw = rot.eulerAngles.y;
            SendMessageEx(MessageTitles.cameramanager_setYawPitch, GetSavedNumber("CameraManager"), data);
            whenFall?.Invoke();
            yield break;
        }

        if(playerLayer == (playerLayer | (1<<coll.gameObject.layer)))
        {
            if (_player.GetState == PlayerUnit.deadState || _player.GetState == PlayerUnit.respawnState)
                yield break;

            _player.ChangeState(PlayerUnit.respawnState);
            yield return new WaitForSeconds(1.0f);
            var rot = Quaternion.LookRotation(respawn.forward);

            var respawnData = MessageDataPooling.GetMessageData<MD.PositionRotation>();
            respawnData.position = respawn.position;
            respawnData.rotation = rot;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), respawnData);
            Vector3Data position = MessageDataPooling.GetMessageData<Vector3Data>();
            position.value = respawn.position;
            SendMessageEx(MessageTitles.cameramanager_setBrainCameraPosition, GetSavedNumber("CameraManager"), position);
            PitchYawData data = MessageDataPooling.GetMessageData<PitchYawData>();
            data.pitch = rot.eulerAngles.x; data.yaw = rot.eulerAngles.y;
            SendMessageEx(MessageTitles.cameramanager_setYawPitch, GetSavedNumber("CameraManager"), data);
            whenFall?.Invoke();
        }
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
    }
}
