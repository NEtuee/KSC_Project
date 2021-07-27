using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public struct DistanceBlendProfile
{
    public string name;
    public float targetDistance;
    public float blendDuration;
}

public class CameraManager : ManagerBase
{
    [SerializeField] private CinemachineBrain brain;
    private Transform brainCameraTransfrom;
    [SerializeField] private CinemachineVirtualCameraBase playerFollowCam;
    [SerializeField] private Cinemachine3rdPersonFollow playerFollowCam3rdPersonComponent;
    private Cinemachine3rdPersonFollow playerAimCam3rdPersonComponent;
    private Cinemachine3rdPersonFollow current3rdPersonComponent;
    [SerializeField] private CinemachineVirtualCameraBase playerAimCam;
    [SerializeField] private CinemachineVirtualCamera playerAimCamOrigin;
    [SerializeField] private List<CinemachineVirtualCameraBase> otherCameras = new List<CinemachineVirtualCameraBase>();
    [SerializeField] private Transform spearLunchPos;
    [SerializeField] private Material radialBlur;
    [SerializeField] private bool isBlend;
    [SerializeField] private bool isAttentionCamera;
    [SerializeField] private CinemachineImpulseSource _recoilImpulseSource;
    [SerializeField] private float collisionRadius;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private AnimationCurve blurCurve;
    [SerializeField] private FollowTargetCtrl followTarget;
    
    private bool isBlendCameraDistance;
    private float targetDistance;
    private float distanceBlendStartTime;
    private float distanceBlendDuration;

    private float aimDistanceOrigin;
    private float aimDistanceFactor;
    private float aimDistanceBlendStartTimer;
    private bool isRunningCallBackCoroutine;
    private CinemachineVirtualCameraBase currentActiveCam = null;
    private CinemachineVirtualCameraBase prevActiveCam = null;
    
    private float radialBlurTimer = 0f;
    private float radialBlurTime = 0f;
    private float radialBlurFactor = 0f;
    private bool radialBlurLerp = false;

    private DirectionCollisionEx collisionEx;
    private bool isCameraCollision;
    
    private Dictionary<string, CinemachineVirtualCameraBase> cameraDictionary = new Dictionary<string, CinemachineVirtualCameraBase>();

    private Dictionary<string, bool> renderPassActivation = new Dictionary<string, bool>();
    
    private float cameraSideSmoothVelocity;

    public DistanceBlendProfile[] distanceBlendProfiles;
    private Dictionary<string, DistanceBlendProfile> distanceDic = new Dictionary<string, DistanceBlendProfile>();
    private string _currentCameraDistanceProfileKey;
    //Damping
    private Vector3 prevDamping = Vector3.zero;

    private PlayerCtrl_Ver2 _player;
    private Transform _playerTransfrom;

    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("CameraManager");

        AddAction(MessageTitles.cameramanager_setradialblur, (msg) =>
        {
            SetRadialBlurData data = (SetRadialBlurData)msg.data;
            SetRadialBlur(data.factor, data.radius, data.time);
        });

        AddAction(MessageTitles.cameramanager_activeplayerfollocamera, (msg) => ActivePlayerFollowCamera());
        AddAction(MessageTitles.cameramanager_activeaimcamera, (msg) => ActiveAimCamera());

        AddAction(MessageTitles.cameramanager_setaimcameradistance, (msg) =>
        {
            float factor = (float)msg.data;
            SetAimCameraDistance(factor);
        });

        AddAction(MessageTitles.cameramanager_activevirtualcamera, (msg) =>
        {
            ActiveVirtualCameraData data = (ActiveVirtualCameraData)msg.data;
            ActiveVirtualCamera(data.cam, data.follow);
        });

        AddAction(MessageTitles.cameramanager_setfollowcameradistance, (msg) => 
        {
            string key = (string)msg.data;
            SetFollowCameraDistance(key);
        });

        AddAction(MessageTitles.cameramanager_setzerodamping, (msg) => ZeroDamping());
        AddAction(MessageTitles.cameramanager_restoredamping, (msg) => 
        {
            float lateTime = (float)msg.data;
            RestoreDamping(lateTime);
        });

        AddAction(MessageTitles.cameramanager_setdamping, (msg) => 
        {
            Vector3 damp = (Vector3)msg.data;
            SetDamping(damp);
        });
        AddAction(MessageTitles.cameramanager_generaterecoilimpluse, (msg) => GenerateRecoilImpulse());

        AddAction(MessageTitles.set_setplayer, (msg) => 
        {
            _player = (PlayerCtrl_Ver2)msg.data;
            _playerTransfrom = _player.transform;
        });

        AddAction(MessageTitles.cameramanager_getCameraManager, (msg) =>
         {
             SendMessageQuick((MessageReceiver)msg.sender, MessageTitles.set_setCameraManager, this);
         });
    }

    public override void Initialize()
    {
        base.Initialize();

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        brainCameraTransfrom = brain.transform;
        Debug.Log(GameManager.Instance);
        GameManager.Instance.cameraManager = this;

        collisionEx = new DirectionCollisionEx(playerFollowCam.transform, collisionRadius, collisionLayer);

        otherCameras.Add(playerFollowCam);
        otherCameras.Add(playerAimCam);

        InitializeCameraAtGameStart();

        if (_player.updateMethod == UpdateMethod.FixedUpdate)
        {
            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
            brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
        }
        else
        {
            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.LateUpdate;
        }

        for (int i = 0; i < distanceBlendProfiles.Length; i++)
        {
            distanceDic.Add(distanceBlendProfiles[i].name, distanceBlendProfiles[i]);
        }

        SetFollowCameraDistance("Default");
        aimDistanceOrigin = playerAimCamOrigin.m_Lens.FieldOfView;
    }

    private void Update()
    {
        //UpdateCameraSide((GameManager.Instance.GetInputHorizontal() + 1f) * 0.5f);
        // if(isAttentionCamera)
        // {
        //     UpdateCameraSide();
        // }

        var cross = Vector3.Cross(brainCameraTransfrom.forward,Vector3.up);
        var side = Vector3.Dot(cross.normalized,_playerTransfrom.forward);
        var currSide = playerFollowCam3rdPersonComponent.CameraSide;

        playerFollowCam3rdPersonComponent.CameraSide = Mathf.Lerp(currSide, 0.5f - (side * 0.5f),4f * Time.deltaTime);

        RadialBlurLerpZero(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        BlendDistanceFollowCamera();
        BlendDistanceAimCamera();
    }

    public void SetBrainCameraPosition(Vector3 position)
    {
        brainCameraTransfrom.position = position;
    }

    public void SetActiveScreenEffect(string target, bool value)
    {
        if (!renderPassActivation.ContainsKey(target))
            renderPassActivation.Add(target, value);
        else
            renderPassActivation[target] = value;
    }

    public bool CheckScreenEffectActive(string target)
    {
        if (!renderPassActivation.ContainsKey(target))
            renderPassActivation.Add(target, true);
        else
            return renderPassActivation[target];

        return false;
    }
    
    
    
    public void RadialBlurLerpZero(float deltaTime)
    {
        if (!radialBlurLerp)
            return;
        
        radialBlurTimer += deltaTime;
        float factor = blurCurve.Evaluate(radialBlurTimer) * radialBlurFactor;
        // var factor = Mathf.Lerp(radialBlurFactor, 0f, radialBlurTimer / radialBlurTime);
        //
        // if (radialBlurTimer >= radialBlurTime)
        // {
        //     factor = 0f;
        //     radialBlurLerp = false;
        //  }
        radialBlur.SetFloat("_EffectAmount",factor);

        radialBlurLerp = radialBlurTimer < 1f;
    }
    
    public void SetRadialBlur(float factor = 0.2f, float radius = 0.1f, float time = 0.8f)
    {
        radialBlurFactor = factor;
        radialBlurTimer = 0f;
        radialBlurTime = time;
        radialBlurLerp = true;
        //radialBlur.SetFloat("_EffectAmount",factor);
        radialBlur.SetFloat("_Radius", radius);
    }
    
    private void InitializeCameraAtGameStart()
    { 
        playerAimCam.gameObject.SetActive(false);
        foreach(var cam in otherCameras)
        {
            cam.gameObject.SetActive(false);
        }

        playerFollowCam.gameObject.SetActive(true);

        currentActiveCam = playerFollowCam;

        playerFollowCam3rdPersonComponent = playerFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        playerAimCam3rdPersonComponent = playerAimCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        current3rdPersonComponent = playerFollowCam3rdPersonComponent;
    }
    

    public bool ActivePlayerFollowCamera()
    {
        if (playerFollowCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerFollowCam;
        currentActiveCam.gameObject.SetActive(true);

        current3rdPersonComponent = playerFollowCam3rdPersonComponent;

        return true;
    }

    public bool ActivePlayerFollowCamera(Action doneBlendCall)
    {
        if (playerFollowCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerFollowCam;
        currentActiveCam.gameObject.SetActive(true);
        current3rdPersonComponent = playerFollowCam3rdPersonComponent;

        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    public bool ActiveAimCamera()
    {
        if (playerAimCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerAimCam;
        currentActiveCam.gameObject.SetActive(true);
        current3rdPersonComponent = playerAimCam3rdPersonComponent;
        return true;
    }

    public bool ActiveVirtualCamera(CinemachineVirtualCameraBase cam, Cinemachine3rdPersonFollow follow = null)
    {
        if (cam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = cam;
        currentActiveCam.gameObject.SetActive(true);
        current3rdPersonComponent = follow;
        return true;
    }

    public bool ActiveAimCamera(Action doneBlendCall)
    {
        if (playerAimCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerAimCam;
        currentActiveCam.gameObject.SetActive(true);
        current3rdPersonComponent = playerAimCam3rdPersonComponent;
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    public bool BackToPrevCamera()
    {
        if(prevActiveCam == null)
        {
            return false;
        }

        CinemachineVirtualCameraBase temp;
        temp = currentActiveCam;
        currentActiveCam.gameObject.SetActive(false);
        currentActiveCam = prevActiveCam;
        prevActiveCam = temp;
        currentActiveCam.gameObject.SetActive(true);

        return true;
    }

    public bool BackToPrevCamera(Action doneBlendCall)
    {
        if (prevActiveCam == null)
        {
            return false;
        }

        CinemachineVirtualCameraBase temp;
        temp = currentActiveCam;
        currentActiveCam.gameObject.SetActive(false);
        currentActiveCam = prevActiveCam;
        prevActiveCam = temp;
        currentActiveCam.gameObject.SetActive(true);
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    public bool ActiveCamera(CinemachineVirtualCameraBase activeCamera)
    {
        if (activeCamera == null)
            return false;

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = activeCamera;
        currentActiveCam.gameObject.SetActive(true);
        return true;
    }

    public bool ActiveCamera(CinemachineVirtualCameraBase activeCamera, Action doneBlendCall)
    {
        if (activeCamera == null)
            return false;

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = activeCamera;
        currentActiveCam.gameObject.SetActive(true);
        if(isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    public bool ActiveCamera(string cameraKey)
    {
        if(cameraDictionary.ContainsKey(cameraKey) == false)
        {
            Debug.Log(cameraKey + " does not exist.");
            return false;
        }

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = cameraDictionary[cameraKey];
        currentActiveCam.gameObject.SetActive(true);
        return true;
    }

    public bool ActiveCamera(string cameraKey, Action doneBlendCall)
    {
        if (cameraDictionary.ContainsKey(cameraKey) == false)
        {
            Debug.Log(cameraKey + " does not exist.");
            return false;
        }

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = cameraDictionary[cameraKey];
        currentActiveCam.gameObject.SetActive(true);
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    public void RegisterCameraKey(string key, CinemachineVirtualCameraBase camera)
    {
        cameraDictionary.Add(key, camera);
    }

    public CinemachineBrain GetCinemachineBrain() {return brain;}

    public bool IsCameraBlending()
    {
        return brain.IsBlending;
    }

    public Vector3 GetCameraPosition() { return brainCameraTransfrom.position; }
    public Quaternion GetCameraRotation() { return brainCameraTransfrom.rotation; }

    IEnumerator BlendingCallBack(Action action)
    {
        isRunningCallBackCoroutine = true;
        yield return null;
        while(brain.IsBlending)
        {
            yield return null;
        }
        action();
        isRunningCallBackCoroutine = false;
    }

    public Transform GetSpearLunchTransform()
    {
        if (spearLunchPos != null)
            return spearLunchPos;
        else
            return null;
    }

    public void SetCameraSide(float value)
    {
        playerFollowCam3rdPersonComponent.CameraSide = value;
        Debug.Log(value);
    }

    // public void UpdateCameraSide()
    // {
    //     if (GameManager.Instance.bossTransform == null)
    //         return;

    //     Vector3 camForward = brainCameraTransfrom.forward;
    //     camForward.y = 0;
    //     camForward.Normalize();
    //     Vector3 toBossDir = (GameManager.Instance.bossTransform.position - brainCameraTransfrom.position);
    //     toBossDir.y = 0f;
    //     toBossDir.Normalize();

    //     float targetFactor;
    //     float angle = Vector3.Dot(camForward, toBossDir);
    //     float min = 0.8f;
    //     float max = 1.0f;

    //     if (angle >= min && angle <= max)
    //     {
    //         float factor = max - angle;
    //         targetFactor = factor / (1f - min);
    //     }
    //     else if (angle < min && angle >= -0.2f)
    //     {
    //         targetFactor = 1.0f;
    //     }
    //     else
    //     {
    //         targetFactor = 0.0f;
    //     }

    //     if (Vector3.Cross(camForward, toBossDir).y < 0)
    //     {
    //         targetFactor *= -0.5f;
    //     }
    //     else
    //     {
    //         targetFactor *= 0.5f;
    //     }

    //     playerFollowCam3rdPersonComponent.CameraSide = Mathf.SmoothDamp(playerFollowCam3rdPersonComponent.CameraSide, 0.5f + targetFactor, ref cameraSideSmoothVelocity, 300f*Time.deltaTime);
    // }

    public void SetAimCameraDistance(float factor)
    {
        //playerAimCamOrigin.m_Lens.FieldOfView += distance;
        aimDistanceFactor = factor;
        aimDistanceBlendStartTimer = 0f;
    }

    public void SetFollowCameraDistance(float targetDistance, float blendDuration)
    {
        isBlendCameraDistance = true;
        this.targetDistance = targetDistance;
        distanceBlendDuration = blendDuration;
        distanceBlendStartTime = Time.time;
    }

    public void SetFollowCameraDistance(string key)
    {
        if (distanceDic.ContainsKey(key) == false)
            return;

        if (_currentCameraDistanceProfileKey == key)
            return;

        _currentCameraDistanceProfileKey = key;

        isBlendCameraDistance = true;
        targetDistance = distanceDic[key].targetDistance;
        distanceBlendDuration = distanceDic[key].blendDuration;
        distanceBlendStartTime = Time.time;
    }

    private void BlendDistanceFollowCamera()
    {
        
        float t = (Time.time - distanceBlendStartTime) / distanceBlendDuration;
        float currentDist = playerFollowCam3rdPersonComponent.CameraDistance;

        playerFollowCam3rdPersonComponent.CameraDistance = Mathf.SmoothStep(currentDist, targetDistance, t);
        if (t >= 1.0f)
        {
            isBlendCameraDistance = false;
        }
        
        var prev = isCameraCollision;
        var offset = playerFollowCam3rdPersonComponent.ShoulderOffset;
        offset.x = 0f;
        var dir = -(Camera.main.transform.rotation * Vector3.forward).normalized;
        
        isCameraCollision =
                        //collisionEx.Cast(GameManager.Instance.followTarget.transform.position + offset,dir,playerFollowCam3rdPersonComponent.CameraDistance + 1f, out var dist, out var center);
                        collisionEx.Cast(followTarget.transform.position, dir, playerFollowCam3rdPersonComponent.CameraDistance + 1f, out var dist, out var center);

        if (isCameraCollision)
        {
            //dist += 1f;
            if (dist <= playerFollowCam3rdPersonComponent.CameraDistance)
                playerFollowCam3rdPersonComponent.CameraDistance = dist;
        }

    }

    public void SetUpdateMethod(CinemachineBrain.UpdateMethod update,CinemachineBrain.BrainUpdateMethod blend)
    {
        brain.m_UpdateMethod = update;
        brain.m_BlendUpdateMethod = blend;
    }

    public void SetUpdateMethod()
    {
        if(_player.updateMethod == UpdateMethod.FixedUpdate)
        {
            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
            brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
        }
        else
        {
            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.LateUpdate;
        }
    }

    private void BlendDistanceAimCamera()
    {
        if (aimDistanceBlendStartTimer >= 1f)
            return;
        
        aimDistanceBlendStartTimer += Time.deltaTime;
        float factor = animationCurve.Evaluate(aimDistanceBlendStartTimer) * aimDistanceFactor;
        playerAimCamOrigin.m_Lens.FieldOfView = aimDistanceOrigin + factor;
    }

    public void ZeroDamping()
    {
        prevDamping = playerFollowCam3rdPersonComponent.Damping;
        playerFollowCam3rdPersonComponent.Damping = Vector3.zero;
    }

    public void RestoreDamping()
    {
        playerFollowCam3rdPersonComponent.Damping = prevDamping;
    }

    public void RestoreDamping(float lateTime)
    {
        StartCoroutine(LateRestoreDampingCoroutine(lateTime));
    }

    IEnumerator LateRestoreDampingCoroutine(float lateTime)
    {
        yield return new WaitForSeconds(lateTime);
        RestoreDamping();
    }

    public void SetDamping(Vector3 damp)
    {
        playerFollowCam3rdPersonComponent.Damping = damp;
    }

    public float GetCameraDistance()
    {
        return current3rdPersonComponent.CameraDistance;
    }

    public void GenerateRecoilImpulse()
    {
        if (_recoilImpulseSource == null)
            return;
        _recoilImpulseSource.GenerateImpulse();
    }
}

public struct SetRadialBlurData
{
    public float factor;
    public float radius;
    public float time;
}

public struct ActiveVirtualCameraData
{
   public CinemachineVirtualCameraBase cam;
   public Cinemachine3rdPersonFollow follow;
}