using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_Phase_AI : PathfollowObjectBase
{
    public StateProcessor stateProcessor;
    public Transform respawnPoint;
    public Transform respawnTarget;
    public Transform explosionTarget;
    public Transform body;
    public Transform eyelightPosition;
    public Boogie_GridControll gridControll;
    public Animator animatorController;
    public ParticleSystem leftHandEffect;
    public float bodyRotateSpeed = 5f;

    public Transform leftHandHitEffect;
    public Transform rightHandHitEffect;

    private PlayerUnit _player;
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private bool _respawn = false;
    private HexCube _respawnCube = null;
    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            targetTransform = _player.transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        stateProcessor.StateChange("Idle");
        _timeCounter.InitTimer("spawn",0f,5f);
        _respawn = false;
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!_respawn)
            stateProcessor.StateProcess(deltaTime);
        else
        {
            var spawn = false;
            _timeCounter.IncreaseTimerSelf("spawn",out spawn, deltaTime);
            if(spawn)
            {
                _respawnCube.special = false;
                _respawn = false;
            }
        }
    }

    public void ChangeAnimation(int code)
    {
        if(code == 1)
        {
            MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

            data.key = "CannonExplosion";
            data.position = explosionTarget.position;
            data.rotation = explosionTarget.rotation;

            SendMessageEx(MessageTitles.effectmanager_activeeffect,
                        UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data);
        }
        animatorController.SetTrigger("Change");
        animatorController.SetInteger("Code",code);
    }

    public void Explosion(Vector3 start, float force)
    {
        var playerDist = Vector3.Distance(_player.transform.position, start);
        if (playerDist <= 6f)
        {
            var dir = transform.forward;//(_player.transform.position - start).normalized;
            _player.Ragdoll.ExplosionRagdoll(force, dir);
            _player.TakeDamage(10f);
        }

        SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse,
                                        UniqueNumberBase.GetSavedNumberStatic("CameraManager"), null);
    }

    public void Respawn()
    {
        _timeCounter.InitTimer("spawn",0f,5f);
        _respawn = true;

        if(_respawnCube != null)
            _respawnCube.special = false;

        _respawnCube = gridControll.cubeGrid.GetCubeFromWorld(respawnTarget.position,false);
        _respawnCube.MoveToUp();
        _respawnCube.special = true;

        respawnPoint.position = _respawnCube.transform.position + Vector3.up;
    }

    public void CreateEyeLight()
    {
        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

        data.key = "EyelightBig";
        data.position = eyelightPosition.position;
        data.rotation = eyelightPosition.rotation;
        data.parent = eyelightPosition;

        SendMessageEx(MessageTitles.effectmanager_activeeffectsetparent,
                    UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data);
    }

    public void LeftHit()
    {
        Explosion(leftHandHitEffect.position, 150f);
    }

    public void RightHit()
    {
        Explosion(rightHandHitEffect.position, 150f);
    }

    public void ChestOpenSound()
    {
        MD.SoundPlayData soundData = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
        soundData.id = 1535;
        soundData.position = explosionTarget.position;
        soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
    }

    public void LeftHitSound()
    {
        MD.SoundPlayData soundData = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
        soundData.id = 1536;
        soundData.position = leftHandHitEffect.position;
        soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
    }

    public void RightHitSound()
    {
        MD.SoundPlayData soundData = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
        soundData.id = 1536;
        soundData.position = rightHandHitEffect.position;
        soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
    }

    public void CreateLeftHit()
    {
        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

        data.key = "HeavyHit";
        data.position = leftHandHitEffect.position;

        SendMessageEx(MessageTitles.effectmanager_activeeffect,
                    UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data);

        LeftHit();
    }

    public void CreateRightHit()
    {
        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

        data.key = "HeavyHit";
        data.position = rightHandHitEffect.position;

        SendMessageEx(MessageTitles.effectmanager_activeeffect,
                    UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data);

        RightHit();
    }

    public void PlayLeftHandEffect()
    {
        leftHandEffect.Play();
    }

    public void PauseLeftHandEffect()
    {
        leftHandEffect.Stop(true);
    }
}
