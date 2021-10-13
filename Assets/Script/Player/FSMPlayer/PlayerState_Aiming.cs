using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class PlayerState_Aiming : PlayerState
{
    private IEnumerator _dechargingCoroutine;
    private float _fallingControlSenstive = 1f;
    private TimeCounterEx _chargeDelayTimer = new TimeCounterEx();
    private float dechargingRatio = 0.3f;
    private float _gunCost = 25.0f;
    private float _normalCost = 25.0f;
    private float _chargeCost = 50.0f;
    private int _transformingCount = 0;
    private float _chargeDelayTime;
    private float verticalValue = 0.0f;
    private float horizonValue = 0.0f;
    private float _horizonAmount = 0.0f;

    private void Awake()
    {
        _chargeDelayTimer.InitTimer("ChargeDelay", _chargeDelayTime, _chargeDelayTime);
    }

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Aiming";

        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1008; soundData.localPosition = Vector3.up; soundData.parent = playerUnit.Transform; soundData.returnValue = false;
        playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);

        AttachSoundPlayData chargeSoundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        chargeSoundPlayData.id = 1013; chargeSoundPlayData.localPosition = Vector3.up; chargeSoundPlayData.parent = playerUnit.Transform; chargeSoundPlayData.returnValue = true;
        playerUnit.SendMessageQuick(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), chargeSoundPlayData);

        playerUnit.loadCount.Value = 1;
        playerUnit.EmpGun.Active(true);
        animator.SetLayerWeight(1, 1.0f);
        animator.SetLayerWeight(2, 1.0f);

        playerUnit.SendMessageEx(MessageTitles.cameramanager_activeaimcamera, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), null);
        BoolData visibleDisable = MessageDataPooling.GetMessageData<BoolData>();
        visibleDisable.value = false;
        playerUnit.SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, UniqueNumberBase.GetSavedNumberStatic("UIManager"), visibleDisable);
        playerUnit.FootIK.DisableFeetIk();
        playerUnit.Drone.OrderAimHelp(true);
        BoolData data = MessageDataPooling.GetMessageData<BoolData>();
        data.value = true;
        playerUnit.SendMessageEx(MessageTitles.uimanager_activegunui, UniqueNumberBase.GetSavedNumberStatic("UIManager"), data);
        _transformingCount = 0;
        playerUnit.CanCharge = true;

        
        BoolData aimData = MessageDataPooling.GetMessageData<BoolData>();
        aimData.value = true;
        playerUnit.SendMessageEx(MessageTitles.cameramanager_setAim,UniqueNumberBase.GetSavedNumberStatic("CameraManager"),aimData);

        playerUnit.addibleSpineVector = Vector3.zero;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1009; soundData.localPosition = Vector3.up; soundData.parent = playerUnit.Transform; soundData.returnValue = false;
        playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);

        int loadCount = (int)(playerUnit.chargeTime.Value);

        playerUnit.loadCount.Value = 0;
        playerUnit.chargeTime.Value = 0.0f;
        playerUnit.EmpGun.Active(false);
        animator.SetLayerWeight(1, 0.0f);
        animator.SetLayerWeight(2, 0.0f);

        if (loadCount == 3)
        {
            if (playerUnit.Decharging == true)
                StopCoroutine(_dechargingCoroutine);

            _dechargingCoroutine = playerUnit.DechargingCoroutine();
            StartCoroutine(_dechargingCoroutine);

            EffectActiveData effectData = MessageDataPooling.GetMessageData<EffectActiveData>();
            effectData.key = "Decharging";
            effectData.position = playerUnit.SteamPosition.position;
            effectData.rotation = Quaternion.LookRotation(-playerUnit.SteamPosition.up);
            effectData.parent = playerUnit.SteamPosition;
            playerUnit.SendMessageEx(MessageTitles.effectmanager_activeeffectsetparent, UniqueNumberBase.GetSavedNumberStatic("EffectManager"), effectData);

            //EffectActiveData data1 = MessageDataPooling.GetMessageData<EffectActiveData>();
            //data1.position = transform.position;
            //data1.rotation = Quaternion.identity;
            //data1.key = "Laser02";
            //playerUnit.SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation, UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data1);

            //playerUnit.SendMessageEx(MessageTitles.effectmanager_activeeffect, UniqueNumberBase.GetSavedNumberStatic("EffectManager"), effectData);
            //Debug.Log("Decahrging effect");

            AttachSoundPlayData dechargingSoundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            dechargingSoundData.id = 1025; dechargingSoundData.localPosition = Vector3.up; dechargingSoundData.parent = playerUnit.Transform; dechargingSoundData.returnValue = false;
            playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), dechargingSoundData);
        }

        if (playerUnit._chargeSoundEmitter != null)
        {
            playerUnit._chargeSoundEmitter.Stop();
            playerUnit._chargeSoundEmitter = null;
        }

        playerUnit.SendMessageEx(MessageTitles.cameramanager_activeplayerfollocamera, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), null);
        playerUnit.Drone.OrderAimHelp(false);
        BoolData data = MessageDataPooling.GetMessageData<BoolData>();
        data.value = false;
        playerUnit.SendMessageEx(MessageTitles.uimanager_activegunui, UniqueNumberBase.GetSavedNumberStatic("UIManager"), data);

        animator.SetTrigger("ResetShot");

        verticalValue = 0.0f;
        horizonValue = 0.0f;


        BoolData aimData = MessageDataPooling.GetMessageData<BoolData>();
        aimData.value = false;
        playerUnit.SendMessageEx(MessageTitles.cameramanager_setAim,UniqueNumberBase.GetSavedNumberStatic("CameraManager"),aimData);

        playerUnit.addibleSpineVector = Vector3.zero;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {       
        if (playerUnit.CurrentSpeed > 0.0f)
            playerUnit.AddEnergy(playerUnit.AimRestoreEnergyValue * Time.fixedDeltaTime);

        RaycastHit hit;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        if (playerUnit.IsGround == true)
        {
            float inputHorizonAmount = Mathf.Abs(animator.GetFloat("InputHorizon"));

            if (playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
            {
                playerUnit.MoveDir = (camForward * playerUnit.InputVertical) + (camRight * playerUnit.InputHorizontal);
                playerUnit.MoveDir.Normalize();
            }
            else
            {
                playerUnit.MoveDir= playerUnit.PrevDir;
                playerUnit.MoveDir.Normalize();
            }

            if(playerUnit.InputVertical == 0.0f)
            {
                playerUnit.CurrentSpeed *= inputHorizonAmount;
            }

            Vector3 aimDir = camForward;
            playerUnit.LookDir = aimDir;

            playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation,
                Quaternion.LookRotation(aimDir, Vector3.up), 30.0f * Time.fixedDeltaTime);

            if (Physics.Raycast(playerUnit.Transform.position + Vector3.up, Vector3.down, out hit, 2f, playerUnit.GrounLayer))
            {
                playerUnit.MoveDir = (Vector3.ProjectOnPlane(playerUnit.MoveDir, hit.normal)).normalized;
            }

            playerUnit.MoveDir *= playerUnit.CurrentSpeed;
            playerUnit.Move(playerUnit.MoveDir,Time.fixedDeltaTime);
        }
        else
        {
            playerUnit.CurrentJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
            playerUnit.CurrentJumpPower = Mathf.Clamp(playerUnit.CurrentJumpPower, playerUnit.MinJumpPower, 50.0f);

            playerUnit.MoveDir = playerUnit.Transform.forward * playerUnit.CurrentSpeed;

            Vector3 plusDir = ((camForward * playerUnit.InputVertical) + (camRight * playerUnit.InputHorizontal));
            playerUnit.Move(plusDir * _fallingControlSenstive,Time.fixedDeltaTime);

            playerUnit.LookDir = ((camForward * playerUnit.InputVertical) + (camRight * playerUnit.InputHorizontal)).normalized;
            if (playerUnit.LookDir != Vector3.zero)
            {
                playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation,
                    Quaternion.LookRotation(playerUnit.LookDir, Vector3.up), Time.fixedDeltaTime * 30.0f);
            }
            else
            {
                playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation,
                    Quaternion.LookRotation(playerUnit.Transform.forward, Vector3.up), Time.fixedDeltaTime * 30.0f);
            }

            playerUnit.Move(playerUnit.MoveDir + (Vector3.up * playerUnit.CurrentJumpPower),Time.fixedDeltaTime);
        }

        _chargeDelayTimer.IncreaseTimerSelf("ChargeDelay", out bool limit, Time.deltaTime);
        if (limit)
        {
            if (playerUnit._chargeSoundEmitter == null)
            {
                //Debug.Log(playerUnit._chargeSoundEmitter);
                AttachSoundPlayData chargeSoundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                chargeSoundPlayData.id = 1013; chargeSoundPlayData.localPosition = Vector3.up; chargeSoundPlayData.parent = playerUnit.Transform; chargeSoundPlayData.returnValue = true;
                playerUnit.SendMessageQuick(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), chargeSoundPlayData);
            }

            if (playerUnit.CanCharge == true)
            {
                playerUnit.chargeTime.Value += Time.deltaTime * (playerUnit.Decharging ? dechargingRatio : 1f);
                //playerUnit.chargeTime.Value = Mathf.Clamp(playerUnit.chargeTime.Value, 0.0f, Mathf.Abs(playerUnit.energy.Value / _gunCost));
                playerUnit.chargeTime.Value = Mathf.Clamp(playerUnit.chargeTime.Value, 0.0f, playerUnit.ChargeConsumeTime);
            }

            SetParameterData setParameterData = MessageDataPooling.GetMessageData<SetParameterData>();
            setParameterData.soundId = 1013; setParameterData.paramId = 10131; setParameterData.value = (playerUnit.chargeTime.Value) * 100f;
            playerUnit.SendMessageEx(MessageTitles.fmod_setParam, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), setParameterData);

            if (_transformingCount < (int)playerUnit.chargeTime.Value)
            {
                //GameManager.Instance.soundManager.Play(1019 + _transformCount, Vector3.up, transform);
                AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                soundData.id = 1019 + _transformingCount; soundData.localPosition = Vector3.up; soundData.parent = playerUnit.Transform; soundData.returnValue = false;
                playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
                _transformingCount = (int)playerUnit.chargeTime.Value;
            }
        }

        playerUnit.PrevDir = playerUnit.MoveDir;
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.InputVertical != 0)
        {
            verticalValue = Mathf.MoveTowards(verticalValue, playerUnit.InputVertical < 0 ? -1f : 1f, Time.deltaTime * 5.0f);
        }
        else
        {
            verticalValue = Mathf.MoveTowards(verticalValue, 0.0f, Time.deltaTime * 5.0f);
        }

        if (playerUnit.InputHorizontal != 0)
        {
            horizonValue = Mathf.MoveTowards(horizonValue, playerUnit.InputHorizontal < 0 ? -1f : 1f, Time.deltaTime * 5.0f);
        }
        else
        {
            horizonValue = Mathf.MoveTowards(horizonValue, 0.0f, Time.deltaTime * 5.0f);
        }

        animator.SetFloat("InputVerticalValue", verticalValue);
        animator.SetFloat("InputHorizonValue", horizonValue);
    }

    public override void OnAim(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if(value.action.WasReleasedThisFrame())
        {
            playerUnit.ChangeState(PlayerUnit.defaultState);
        }
    }

    public override void OnShot(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.CanCharge == true && playerUnit.Energy >= playerUnit.NoramlGunCost)
        {
            if (playerUnit._chargeSoundEmitter != null)
            {
                playerUnit._chargeSoundEmitter.Stop();
                playerUnit._chargeSoundEmitter = null;
            }

            if (playerUnit.chargeTime.Value >= 3 && playerUnit.Energy >= playerUnit.ChargeGunCost)
            {
                playerUnit.EmpGun.LaunchCharge(40.0f);
                playerUnit.AddEnergy(-playerUnit.ChargeGunCost);
            }
            else
            {
                playerUnit.EmpGun.LaunchNormal();
                playerUnit.AddEnergy(-playerUnit.NoramlGunCost);
            }

            FloatData camDist = MessageDataPooling.GetMessageData<FloatData>();
            camDist.value = 0.5f * (float)(playerUnit.chargeTime.Value >= 3 ? 3 : 1);
            playerUnit.SendMessageEx(MessageTitles.cameramanager_setaimcameradistance, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), camDist);

            _chargeDelayTimer.InitTimer("ChargeDelay", 0.0f, _chargeDelayTime);

            AttachSoundPlayData soundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundPlayData.id = 1009 + (playerUnit.chargeTime.Value >= 3?3:1);
            soundPlayData.localPosition = Vector3.up;
            soundPlayData.parent = playerUnit.Transform;
            soundPlayData.returnValue = false;
            playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundPlayData);

            playerUnit.chargeTime.Value = 0.0f;

            SetRadialBlurData blurData = MessageDataPooling.GetMessageData<SetRadialBlurData>();
            blurData.factor = 1.0f;
            blurData.radius = 0.2f;
            blurData.time = 0.8f;
            playerUnit.SendMessageEx(MessageTitles.cameramanager_setradialblur, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), blurData);

            if (playerUnit.chargeTime.Value >= 3)
            {
                SetTimeScaleMsg data = MessageDataPooling.GetMessageData<SetTimeScaleMsg>();
                data.timeScale = 0.0f;
                data.lerpTime = 0.4f;
                data.stopTime = 0.2f;
                data.startTime = 0.02f;
                playerUnit.SendMessageEx(MessageTitles.timemanager_settimescale, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), data);
            }

            playerUnit.CanCharge = false;
        }
    }
}
