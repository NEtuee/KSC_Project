using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using UniRx;
using MD;

namespace MD
{
    public class EMPHitData : MessageData
    {
        public float damage;
    }
}

public class EMPGun : UnTransfromObjectBase
{
    public RectTransform aimTransform;
    public Canvas aimCanvas;

    [SerializeField] private GameObject _gunObject;
    [SerializeField] private GameObject _pelvisGunObject;
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Transform launchPos;
    [SerializeField] private Transform laserEffectPos;
    [SerializeField] private Transform lookAim;
    [SerializeField] private CrossHair crossHair;
    [SerializeField] private float layserRadius = 1.0f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Vector3 launchPosition;
    [SerializeField] private float launchCoolTime = 1.0f;
    [SerializeField] private ParticleSystem chargingEffect;
    private Quaternion launchRotation;

    private Transform mainCam;
    private Vector3 targetPos;

    public GameObject PelvisGunObject => _pelvisGunObject;

    private float aimWeight;
    
    private Transform mainCamera;
    private RaycastHit hit;

    private TimeCounterEx _timer;
    private bool _canLaunch = true;
    public bool CanLaunch => _canLaunch;

    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main.transform;
        if(_gunObject != null)
           _gunObject.SetActive(false);
        playerAnim = GetComponent<Animator>();
        mainCam = Camera.main.transform;

        _timer = new TimeCounterEx();
        _timer.InitTimer("CoolTime", launchCoolTime, launchCoolTime);
        //GetComponent<PlayerCtrl_Ver2>().chargeTime.Subscribe(value => { 
        //    if(value >= 3f)
        //    {
        //        //crossHair.Third();
        //        SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 3);
        //    }
        //    else if(value >= 2f)
        //    {
        //        //crossHair.Second();
        //        SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 2);
        //    }
        //    else if (value >= 1f)
        //    {
        //        //crossHair.First();
        //        SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 1);
        //    }
        //});

    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));
    }

    void Update()
    {
        //Debug.Log("PlayerUnit LateUpdate");

        //Vector3 rayPosition = aimTransform.position;
        //var ray = Camera.main.ScreenPointToRay(rayPosition);
        //if (Physics.Raycast(ray, out hit, 100f, hitLayer))
        //{
        //    targetPos = hit.point;
        //}
        //else
        //{
        //    targetPos = mainCam.position + mainCam.forward * 100.0f;
        //}

        //launchPos.rotation = Quaternion.LookRotation(targetPos - launchPos.position);

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    GameManager.Instance.cameraManager.GenerateRecoilImpulse();
        //}

        _timer.IncreaseTimerSelf("CoolTime", out bool limit, Time.deltaTime);
        if(limit)
        {
            _canLaunch = true;
        }
    }

    public void UpdateLaunchPos()
    {
        Vector3 rayPosition = aimTransform.position;
        var ray = Camera.main.ScreenPointToRay(rayPosition);
        if (Physics.Raycast(ray, out hit, 100f, hitLayer))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = mainCam.position + mainCam.forward * 100.0f;
        }

        launchPos.rotation = Quaternion.LookRotation(targetPos - launchPos.position);
        launchPosition = launchPos.position;
        launchRotation = launchPos.rotation;
    }

    public void LaunchLaser(float damage)
    {
        if (playerAnim != null)
        {
            playerAnim.SetTrigger("Shot");
        }

        //GameManager.Instance.cameraManager.GenerateRecoilImpulse();
        SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, GetSavedNumber("CameraManager"), null);
        //GameManager.Instance.effectManager.Active("Laser02", laserEffectPos.position, laserEffectPos.rotation);
        //GameManager.Instance.effectManager.Active("Laser_Level2", laserEffectPos.position, laserEffectPos.rotation);

        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.position = laserEffectPos.position;
        data.rotation = laserEffectPos.rotation;
        data.parent = null;
        if (damage <=40f)
        {
            //GameManager.Instance.effectManager.Active("Laser02", laserEffectPos.position, laserEffectPos.rotation);
            data.key = "Laser02";
            SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation, GetSavedNumber("EffectManager"), data);
            //InputManager.Instance.GamePadSetVibrate(0.2f,0.6f);
        }
        else if (damage <= 80f)
        {
            //GameManager.Instance.effectManager.Active("Laser_Level2", laserEffectPos.position, laserEffectPos.rotation);
            //InputManager.Instance.GamePadSetVibrate(0.3f,0.8f);
            data.key = "Laser_Level2";
            SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation, GetSavedNumber("EffectManager"), data);
        }
        else if (damage <= 120f)
        {
            //GameManager.Instance.effectManager.Active("Laser_Level3", laserEffectPos.position, laserEffectPos.rotation);
            //InputManager.Instance.GamePadSetVibrate(0.4f,1.0f);
            data.key = "Laser_Level3";
            SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation, GetSavedNumber("EffectManager"), data);
        }


        //var rayPosition = aimCanvas.worldCamera.ScreenToWorldPoint(aimTransform.position);
        Vector3 rayPosition = aimTransform.position;
        var ray = Camera.main.ScreenPointToRay(rayPosition);
        
        if (Physics.Raycast(ray, out hit, 100.0f))
        //if (Physics.SphereCast(mainCamera.position,layserRadius, mainCamera.forward, out hit, 1000.0f,hitLayer))
        {
            //GameManager.Instance.effectManager.Active("LaserHit",hit.point);
            EffectActiveData hitData = MessageDataPooling.GetMessageData<EffectActiveData>();
            hitData.key = "LaserHit";
            hitData.position = hit.point;
            hitData.rotation = Quaternion.identity;
            hitData.parent = null;
            SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), hitData);

            if(hit.collider.TryGetComponent<MessageReceiver>(out var receiver))
            {
                var empData = MessageDataPooling.GetMessageData<FloatData>();
                empData.value = damage;

                SendMessageEx(receiver,MessageTitles.player_EMPHit,empData);
            }
            if(hit.collider.TryGetComponent<MessageEmpTarget>(out var empTarget))
            {
                var empData = MessageDataPooling.GetMessageData<FloatData>();
                empData.value = damage;

                SendMessageEx(empTarget.parent,MessageTitles.player_EMPHit,empData);
            }
            if (hit.collider.TryGetComponent<Hitable>(out Hitable hitable))
            {
                hitable.Hit(damage);
                crossHair.ActiveHitMark();
                //GameManager.Instance.soundManager.Play(1022, hit.point);
                SoundPlayData soundData = MessageDataPooling.GetMessageData<SoundPlayData>();
                soundData.id = 1022; soundData.position = hit.point; soundData.returnValue = false; soundData.dontStop = false;
                SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData);
            }
            else
            {
                //GameManager.Instance.soundManager.Play(1023, hit.point);
                SoundPlayData soundData = MessageDataPooling.GetMessageData<SoundPlayData>();
                soundData.id = 1023; soundData.position = hit.point; soundData.returnValue = false; soundData.dontStop = false;
                SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData);
            }
        }

        crossHair.Launch();

        if (gunAnim != null)
        {
            gunAnim.SetTrigger("ToZero");
        }
    }

    public void LaunchNormal()
    {
        if (_canLaunch == false)
            return;

        if (playerAnim != null)
        {
            playerAnim.SetTrigger("Shot");
        }

        SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, GetSavedNumber("CameraManager"), null);

        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.position = launchPosition;
        data.rotation = launchRotation;
        data.parent = null;
        data.key = "Laser02";
        SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation, GetSavedNumber("EffectManager"), data);
       
        Vector3 rayPosition = aimTransform.position;
        var ray = Camera.main.ScreenPointToRay(rayPosition);

        if (Physics.Raycast(ray, out hit, 1000.0f,hitLayer))
        {
            EffectActiveData hitData = MessageDataPooling.GetMessageData<EffectActiveData>();
            hitData.key = "LaserHit";
            hitData.position = hit.point;
            hitData.rotation = Quaternion.identity;
            hitData.parent = null;
            SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), hitData);

            crossHair.ActiveHitMark();

            if (hit.collider.TryGetComponent<MessageReceiver>(out var receiver))
            {
                SendMessageEx(receiver, MessageTitles.player_NormalHit, null);
            }
        }

        crossHair.Launch();

        SetRadialBlurData blurData = MessageDataPooling.GetMessageData<SetRadialBlurData>();
        blurData.factor = 1.0f;
        blurData.radius = 0.2f;
        blurData.time = 0.8f;
        SendMessageEx(MessageTitles.cameramanager_setradialblur, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), blurData);

        SendMessageEx(MessageTitles.gamepadVibrationManager_vibrationByKey, GetSavedNumber("GamepadVibrationManager"),"NormalShot");

        if (gunAnim != null)
        {
            //gunAnim.SetTrigger("ToZero");
            gunAnim.SetTrigger("Normal");
        }

        _canLaunch = false;
        _timer.InitTimer("CoolTime", 0.0f, launchCoolTime);
    }

    public void LaunchCharge(float damage)
    {
        if (_canLaunch == false)
            return;

        if (playerAnim != null)
        {
            playerAnim.SetTrigger("ChargeShot");
        }

        SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, GetSavedNumber("CameraManager"), null);
  
        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.position = launchPosition;
        data.rotation = launchRotation;
        data.parent = null;

        data.key = "Laser_Level3";
        SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation, GetSavedNumber("EffectManager"), data);
       
        Vector3 rayPosition = aimTransform.position;
        var ray = Camera.main.ScreenPointToRay(rayPosition);

        if (Physics.Raycast(ray, out hit, 1000.0f,hitLayer))
        {
            EffectActiveData hitData = MessageDataPooling.GetMessageData<EffectActiveData>();
            hitData.key = "ChargeHit";
            hitData.position = hit.point;
            hitData.rotation = Quaternion.identity;
            hitData.parent = null;
            SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), hitData);

            crossHair.ActiveHitMark();

            if (hit.collider.TryGetComponent<MessageReceiver>(out var receiver))
            {
                var empData = MessageDataPooling.GetMessageData<FloatData>();
                empData.value = damage;

                SendMessageEx(receiver, MessageTitles.player_EMPHit, empData);
                SendMessageEx(receiver, MessageTitles.player_NormalHit, null);
            }
            else
            {
                //GameManager.Instance.soundManager.Play(1023, hit.point);
                SoundPlayData soundData = MessageDataPooling.GetMessageData<SoundPlayData>();
                soundData.id = 1023; soundData.position = hit.point; soundData.returnValue = false; soundData.dontStop = false;
                SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData);
            }
        }

        crossHair.Launch();
        SetRadialBlurData blurData = MessageDataPooling.GetMessageData<SetRadialBlurData>();
        blurData.factor = 1.0f;
        blurData.radius = 0.2f;
        blurData.time = 0.8f;
        SendMessageEx(MessageTitles.cameramanager_setradialblur, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), blurData);
        SendMessageEx(MessageTitles.gamepadVibrationManager_vibrationByKey, GetSavedNumber("GamepadVibrationManager"), "ChargeShot");

        if (gunAnim != null)
        {
            //gunAnim.SetTrigger("ToZero");
            gunAnim.SetTrigger("Charge");
        }

        _canLaunch = false;
        _timer.InitTimer("CoolTime", 0.0f, launchCoolTime);
    }

    public void LaunchImpact()
    {
        //impactEffect.Play();
        
    }

    public void Active(bool active)
    {
        //this.gameObject.SetActive(active);
        if (_gunObject != null)
        {
            _gunObject.SetActive(active);
        }

        if(_pelvisGunObject != null)
        {
            _pelvisGunObject.SetActive(!active);
        }

        if (chargingEffect != null)
        {
            if (active == true)
            {
                chargingEffect.gameObject.SetActive(true);
                chargingEffect.Play();
            }
            else
            {
                chargingEffect.gameObject.SetActive(false);
                chargingEffect.Stop();
            }
        }

        //if(crossHair != null)
        //{
        //    crossHair.SetActive(active);
        //}
        BoolData data = MessageDataPooling.GetMessageData<BoolData>();
        data.value = active;
        SendMessageEx(MessageTitles.uimanager_activecrosshair, GetSavedNumber("UIManager"), data);

        if(gunAnim != null)
        {
            if(active == true)
                gunAnim.SetTrigger("Active");
            else
                gunAnim.SetTrigger("Disable");
        }
    }

    public void GunLoad()
    {
        if (gunAnim != null)
        {
            gunAnim.SetTrigger("Next");
        }
    }

    public void GunOff()
    {
        if (gunAnim != null)
        {
            gunAnim.SetTrigger("Off");
        }
    }

    public void EndShot()
    {

    }
}
