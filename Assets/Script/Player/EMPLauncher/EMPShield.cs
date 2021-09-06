using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class EMPShield : Hitable
{
    public enum SoundType
    {
        Shield,
        Bomb
    }

    public SoundType soundType = SoundType.Shield;
    public GameObject destroyEffect;
    public bool isCore = false;
    public bool isActive = false;
    public bool shieldEffect = true;
    public bool isImmortal = false;
    [SerializeField] private bool debug;
    private Color _initColor;
    [ColorUsage(true, true)] public Color secondColor;
    [ColorUsage(true, true)] public Color thirdColor;
    public float intencity;
    private float factor;
    public float secondWpoValue = 0.5f;
    public float thirdWpoValue = 0.8f;
    private float shakeTime = 0.0f;
    private Vector3 originalPosition;
    private float originalWpo;
    private int _hitCount = 0;
    private float _initWpo;

    private ParticleSystem shieldParticle;
    //private Collider collider;

    private Material mat;

    public delegate void WhenReactive(GameObject scanable);
    private WhenReactive whenReactive;
    protected override void Awake()
    {
        base.Awake();

        collider = GetComponent<Collider>();
        if(renderer != null)
            mat = renderer.material;

        if(shieldEffect)
        {
            Color color2 = mat.GetColor("_color2");
            _initColor = mat.GetColor("_color");;
            color2.a = 0f;
            mat.SetColor("_color2",color2);

            originalWpo = mat.GetFloat("_WPO");
            _initWpo = originalWpo;
        }

        factor = Mathf.Pow(2, intencity);

        SetDistortion();
        StartCoroutine(HitEffect());

        var drone = GameObject.FindGameObjectWithTag("Drone")?.GetComponent<DroneScaner>();
        if(drone != null)
        {
            whenReactive += drone.AddScanableObjets;
        }

        //shieldParticle = GetComponent<ParticleSystem>();
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTime > 0.0f)
        {
            transform.position = (Vector3)Random.insideUnitCircle * 0.2f + originalPosition;
            shakeTime -= Time.deltaTime;
            if (shakeTime <= 0.0f)
            {
                transform.localPosition = originalPosition;
            }
        }

        //if(debug)
        //{
        //    //Vector3 cameraPosition = GameManager.Instance.GetPlayerPosition();
        //    Vector3 cameraPosition = ((PlayerCtrl_Ver2)GameManager.Instance.player).GetPlayerCenter();

        //    Bounds bound = collider.bounds;
        //    Vector3 extents = bound.extents;
        //    Vector3 point;
        //    point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);
        //    //2
        //    point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //    //3
        //    point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //    //4
        //    point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //    //5
        //    point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //    //6
        //    point = bound.center + new Vector3(extents.x, extents.y, extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //    //7
        //    point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //    //8
        //    point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        //    Debug.DrawLine(cameraPosition, point, Color.red);

        //}
    }

    private void FixedUpdate()
    {
        //CheckVisible();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("EMP"))
        {
            Hit();
        }
    }

    public override void Hit() 
    {
        if(isActive == false)
            StartCoroutine(ActiveEffect());

        originalPosition = transform.localPosition;
        hp -= 100f;
        shakeTime = 0.1f;
        _hitCount++;
        SetDistortion();
        StartCoroutine(HitEffect());
        if (hp <= 0f && !isImmortal)
        {
            Destroy();

            //Destroy(gameObject);
        }

        whenHit.Invoke();
    }

    public override void Hit(float damage)
    {
        if (isActive == false)
            StartCoroutine(ActiveEffect());

        originalPosition = transform.localPosition;
        hp -= damage;
        _hitCount++;
        SetDistortion();
        StartCoroutine(HitEffect());
        //shakeTime = 0.1f;
        if (hp <= 0f && !isImmortal)
        {
            Destroy();
            //Destroy(gameObject);
        }
        else
        {
            whenHit.Invoke();
        }
    }

    public override void Hit(float damage, out bool isDestroy)
    {
        if (isActive == false)
            StartCoroutine(ActiveEffect());

        originalPosition = transform.localPosition;

        if (isCore == true)
        {
            hp -= 100.0f;
        }
        else
        {
            hp -= damage;
        }
        //shakeTime = 0.1f;
        _hitCount++;
        SetDistortion();
        StartCoroutine(HitEffect());
        isDestroy = false;
        if (hp <= 0f && !isImmortal)
        {
            isDestroy = true;
            Destroy();
        }

        whenHit.Invoke();
    }

    public void Reactive()
    {
        //whenReactive(this.gameObject);

        if(collider != null)
            collider.enabled = true;

        if(renderer != null)
        {
            Debug.Log("Check");
            renderer.enabled = true;
            mat = renderer.material;
        }
        isOver = false;
        isActive = false;

        if(mat != null)
        {
            mat.SetColor("_color",_initColor);
            mat.SetFloat("_WPO",_initWpo);
        }

        if(shieldEffect)
        {
            Color color2 = mat.GetColor("_color2");
            _initColor = mat.GetColor("_color");;
            color2.a = 0f;
            mat.SetColor("_color2",color2);

            originalWpo = mat.GetFloat("_WPO");
            _initWpo = originalWpo;
        }
        
    }

    public override void Destroy()
    {
        if(soundType == SoundType.Shield)
        {
            //GameManager.Instance.soundManager.Play(1515, new Vector3(0, 1, 0), transform);
            //GameManager.Instance.soundManager.Play(1518, new Vector3(0, 1, 0), transform);
            //GameManager.Instance.soundManager.Play(1501,transform.position);
            AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData.id = 1515; soundData.localPosition = new Vector3(0,1,0); soundData.parent = transform; soundData.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData); 
            AttachSoundPlayData soundData2 = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData2.id = 1518; soundData2.localPosition = new Vector3(0, 1, 0); soundData2.parent = transform; soundData2.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData2);
            SoundPlayData soundData3 = MessageDataPooling.GetMessageData<SoundPlayData>();
            soundData3.id = 1501; soundData3.position = transform.position; soundData3.returnValue = false; soundData3.dontStop = false;
            SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData3);
        }
        else if(soundType == SoundType.Bomb)
        {
            SoundPlayData soundData = MessageDataPooling.GetMessageData<SoundPlayData>();
            soundData.id = 1700; soundData.position = transform.position; soundData.returnValue = false; soundData.dontStop = false;
            SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData);
            //GameManager.Instance.soundManager.Play(1700,transform.position);
        }
        

        //Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
        //GameManager.Instance.effectManager.Active("CannonExplosion", transform.position);
        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);
        collider.enabled = false;
        if(renderer != null)
            renderer.enabled = false;
        isOver = true;

        whenDestroy.Invoke();
    }

    public override void Scanned()
    {
        //mat.SetColor("_BaseColor", scanColor);
        if(!gameObject.activeInHierarchy)
            return;
            
        if (isActive == false)
        {
            SetDistortion();
            StartCoroutine(HitEffect());
            StartCoroutine(ActiveEffect());
        }

        ScanMakerData data = MessageDataPooling.GetMessageData<ScanMakerData>();
        data.collider = collider;
        // data.center = collider.bounds;
        // data.min = collider.bounds.min;
        // data.max = collider.bounds.max;
        SendMessageEx(MessageTitles.uimanager_activeScanMaker, GetSavedNumber("UIManager"),data);

        whenScanned?.Invoke();
    }

    IEnumerator ActiveEffect()
    {
        if(!shieldEffect)
            yield break;

        isActive = true;
        Color color2 = mat.GetColor("_color2");
        float target = 0.043f;
        float current = 0.0f;

        while (current < target)
        {
            current += 0.05f * Time.deltaTime;
            color2.a = current;

            mat.SetColor("_color2",color2);

            yield return null;
        }
    }

    IEnumerator HitEffect()
    {
        if(!shieldEffect)
            yield break;

        float wpo = 0.1f;
        mat.SetFloat("_WPO", wpo);
        
        while(wpo > originalWpo)
        {
            wpo -= 2.0f * Time.deltaTime;
            mat.SetFloat("_WPO", wpo);

            yield return null;
        }

        mat.SetFloat("_WPO", originalWpo);
    }

    //private void OnBecameVisible()
    //{
    //    //if (visibleCheck == false)
    //    //    return;

    //    //Debug.Log("visibe");

    //    visible = true;
    //}

    //private void OnBecameInvisible()
    //{
    //    //if (visibleCheck == false)
    //    //    return;
    //    //        Debug.Log("Invisible");
    //    //#if UNITY_EDITOR
    //    //        if (Camera.current != null)
    //    //        {
    //    //            if (Camera.current.name == "SceneCamera")
    //    //                return;
    //    //        }
    //    //#endif

    //    visible = false;
    //}

    private void OnWillRenderObject()
    {
        //if(Camera.current != null)
        //{
        //    visible = false;
        //    currentRenderCamera = Camera.current.name;
        //    if (Camera.current.name == "Preview Camera")
        //    {
        //        visible = true;
        //    }
        //}
        //else
        //{
        //    currentRenderCamera = "null";
        //    visible = true;
        //}
    }

    public bool GetIsVisible()
    {
        return visible;
    }

    public bool CheckInCamera()
    {
        Vector3 cameraPosition = ((PlayerCtrl_Ver2)GameManager.Instance.player).GetPlayerCenter();
        Bounds bound = renderer.bounds;
        Vector3 extents = bound.extents;

        Vector3 point;

        //1
        point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
        if(Physics.Linecast(point,cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //2
        point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //3
        point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //4
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //5
        point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //6
        point = bound.center + new Vector3(extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //7
        point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //8
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }

        return false;
    }

    private void CheckVisible()
    {
        //Vector3 cameraPosition = GameManager.Instance.cameraManger.GetCameraPosition();
        //Vector3 cameraPosition = GameManager.Instance.GetPlayerPosition();
        Vector3 cameraPosition = ((PlayerCtrl_Ver2)GameManager.Instance.player).GetPlayerCenter();
        Bounds bound = renderer.bounds;
        Vector3 extents = bound.extents;

        RaycastHit hit;

        Vector3 point;

        //1
        point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }

        //2
        point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //3
        point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //4
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //5
        point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //6
        point = bound.center + new Vector3(extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //7
        point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //8
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }

        visible = false;
        return;
    }

    private void SetDistortion()
    {
        if(!shieldEffect)
            return;
        if(hp <= 20.0f)
        {
            originalWpo = secondWpoValue;
            mat.SetColor("_color", thirdColor );
            //GameManager.Instance.soundManager.Play(1516, new Vector3(0, 1, 0), transform);
            AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData.id = 1516; soundData.localPosition = new Vector3(0, 1, 0); soundData.parent = transform; soundData.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
        }
        else if(hp <= 60.0f)
        {
            originalWpo = thirdWpoValue;
            mat.SetColor("_color", secondColor);
            //GameManager.Instance.soundManager.Play(1517, new Vector3(0, 1, 0), transform);
            AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData.id = 1517; soundData.localPosition = new Vector3(0, 1, 0); soundData.parent = transform; soundData.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
        }
    }
}
