using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private bool isPause;
    [SerializeField] private float smooth = 10f;
    [Range(0, 1)] [SerializeReference] private float adjustValue = 0.9f;
    private float minDistance = 1f;
    [SerializeField]private float maxDistance = 6.5f;
    private Vector3 dollyDir;
    [SerializeField]private float distance;
    private int collisionIgnore;
    [SerializeField] private bool isShaking = false;
    private float shakingPower = 0.0f;
    [SerializeField]private float timer = 0.0f;
    [SerializeField]private float shakingTime = 2.5f;
    [SerializeField]private float maxPower = 0.5f;
    [SerializeField]private float maxShakingDistance = 100f;
    [SerializeField] private Transform defaultPos;
    [SerializeField] private Transform aimPos;
    [SerializeField] private bool forceNow;

    Vector3 currentVelocity;

    private CameraCtrl camRoot;

    [SerializeField] private bool isLookingEvent;
    private Transform lookingEventTarget;

    [Header("LookAt Adjust")]
    [SerializeField] [Range(-5.0f, 5.0f)] private float upAdjust = 0.0f;
    [SerializeField] [Range(-5.0f, 5.0f)] private float rightAdjust = 0.0f;
    [SerializeField] [Range(-5.0f, 5.0f)] private float forwardAdjust = 0.0f;

    [Header("AimPos Adjust")]
    [SerializeField] [Range(-5.0f, 5.0f)] private float upAdjustFromPlayer = 0.0f;
    [SerializeField] [Range(-5.0f, 5.0f)] private float rightAdjustFromPlayer = 0.0f;
    [SerializeField] [Range(-5.0f, 5.0f)] private float forwardAdjustFromPlayer = 0.0f;

    private Vector3 lookTarget;
    private Transform player;
    private Transform root;

    //포스트 프로세싱 참조
    [Header("Post Process")]
    //private ColorGrading colorGradingSettings;
    //private Bloom bloomSettings;

    [SerializeField] private float intensity_origin;
    [SerializeField] private float threshold_origin;
    [SerializeField] private float post_exposure_origin;

    [SerializeField] private float intensity_target;
    [SerializeField] private float threshold_target;
    [SerializeField] private float post_exposure_target;

    // Start is called before the first frame update
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        minDistance = 1f;
        //maxDistance = Vector3.Distance(Camera.main.transform.position, Camera.main.transform.parent.position);
        camRoot = transform.parent.GetComponent<CameraCtrl>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        root = transform.parent;
        collisionIgnore = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Camera") | 1 << LayerMask.NameToLayer("Enemy") | 1<<LayerMask.NameToLayer("DetectCollider") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("Trigger")| 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Characters"));
    }

    private void Start()
    {
        //transform. = root.InverseTransformPoint(player.transform.position + (player.transform.forward * forwardAdjustFromPlayer) + (player.transform.right * rightAdjustFromPlayer) + (player.transform.up * upAdjustFromPlayer));
        aimPos = root;
        lookingEventTarget = root;

        //LoadPostProcess();
    }

    //private void LoadPostProcess()
    //{
    //    PostProcessVolume volume = GetComponent<PostProcessVolume>();
    //    if(volume.profile == null)
    //    {
    //        Debug.Log("Cant Load PostProcess Volume");
    //        return;
    //    }

    //    bool foundSettings = volume.profile.TryGetSettings<ColorGrading>(out colorGradingSettings);
    //    if(!foundSettings)
    //    {
    //        Debug.Log("Cant Load ColorGrading");
    //        return;
    //    }

    //    foundSettings = volume.profile.TryGetSettings<Bloom>(out bloomSettings);
    //    if(!foundSettings)
    //    {
    //        Debug.Log("Cant Load Bloom");
    //        return;
    //    }

    //    intensity_origin = bloomSettings.intensity.GetValue<float>();
    //    threshold_origin = bloomSettings.threshold.GetValue<float>();

    //    post_exposure_origin = colorGradingSettings.postExposure.GetValue<float>();
    //}

    // Update is called once per frame
    void LateUpdate()
    {
        if(isPause == true)
        {
            if (isShaking == true)
            {
                transform.localPosition = (Vector3)Random.insideUnitCircle * shakingPower + transform.localPosition;
            }

            return;
        }

        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);

        RaycastHit hit;

        //this.transform.rotation = Quaternion.LookRotation(this.transform.parent.position - this.transform.position);

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, collisionIgnore))
        {
            //if(!hit.transform.gameObject.layer.Equals("Player"))
            //Debug.Log(hit.collider);
            //Debug.Log(hit.distance);
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        // transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
        //transform.localPosition = dollyDir * distance * 0.9f;
        //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, dollyDir * distance * 0.9f, ref currentVelocity, Time.deltaTime * smooth);
        if (forceNow == false)
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            maxDistance += wheel * 1000f * Time.deltaTime;
            maxDistance = Mathf.Clamp(maxDistance, 2f, 15f);
        }

        if(camRoot.GetCameMode() == default)
        {
            //transform.localPosition = dollyDir * distance * 0.9f;
            if(!isLookingEvent)
                transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance * adjustValue, Time.deltaTime * smooth);

        }
        else
        {
            if (transform.position != aimPos.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, aimPos.position, Time.unscaledDeltaTime * 25f);
            }
            //else
            //{
            //    GameManager.Instance.timeManager.OnBulletTime();
            //}
        }

        if(isShaking == true)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * shakingPower + transform.localPosition;
        }

        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            shakingPower = maxPower*timer / shakingTime;
        }
        else
        {
            timer = 0.0f;
            isShaking = false;
        }

        //lookTarget = player.transform.position + (player.transform.forward * forwardAdjust) + (player.transform.right * rightAdjust) + (player.transform.up * upAdjust);
        //transform.LookAt(lookTarget);
    }

    private void Update()
    {
        if(isPause == true)
        {
            return;
        }

        lookTarget = root.transform.position + (root.transform.forward * forwardAdjust) + (root.transform.right * rightAdjust) + (root.transform.up * upAdjust);

        if (isLookingEvent == true)
        {
            //transform.LookAt(lookingEventTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingEventTarget.position - transform.position), 5f * Time.deltaTime);
            return;
        }
            
        if (camRoot.GetCameMode() == default)
        {
            //transform.LookAt(lookTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), 20f * Time.deltaTime);
        }

    }

    public void OnShake(float factor, float time)
    {
        shakingPower = factor;
        timer = time;
        isShaking = true;
    }

    public void OnShake(Vector3 monsterPos)
    {
        float dist = (monsterPos - transform.position).magnitude;

        float ratio = dist / maxShakingDistance;

        if(ratio > 1.0f)
        {
            return;
        }
        else
        {
            shakingPower = maxPower * (1 - ratio);
            timer = shakingTime;
            isShaking = true;
        }
    }

    public void LookingEvent(Transform target, float lookingTime)
    {
        if(target == null)
        {
            return;
        }

        lookingEventTarget = target;
        StartCoroutine(Looking(lookingTime));
    }

    public void LookingEvent(Transform target)
    {
        if(target == null)
        {
            return;
        }

        lookingEventTarget = target;
        isLookingEvent = true;
    }

    public void LookingEventEnd()
    {
        isLookingEvent = false;
    }

    IEnumerator Looking(float lookingTime)
    {
        isLookingEvent = true;
        yield return new WaitForSeconds(lookingTime);
        isLookingEvent = false;
    }

    public void ZoomLock()
    {
        forceNow = true;
    }

    public void ZoomUnLock()
    {
        forceNow = false;
    }

    public void ForceZoomOut()
    {
        forceNow = true;
        StartCoroutine(ZoomOut());
    }

    IEnumerator ZoomOut()
    {
        while (maxDistance < 15f)
        {
            maxDistance += 4f * Time.deltaTime;
            maxDistance = Mathf.Clamp(maxDistance, 2f, 15f);

            yield return null;
        }
        forceNow = false;
    }

    public void Pause() { isPause = true; }
    public void Resume() { isPause = false; }

    //public void RequstChangePP(float time)
    //{
    //    StartCoroutine(ChangePostProcess(time));
    //}

    //IEnumerator ChangePostProcess(float changeTime)
    //{
    //    float currentTime = 0.0f;
    //    float currentIntensity = intensity_origin;
    //    float currentThreshold = threshold_origin;
    //    float currentPostExposure = post_exposure_origin;

    //    while(currentTime <= changeTime)
    //    {
    //        float ratio = currentTime / changeTime;

    //        currentIntensity = Mathf.MoveTowards(intensity_origin, intensity_target, ratio);
    //        currentThreshold = Mathf.MoveTowards(threshold_origin, threshold_target, ratio);
    //        currentPostExposure = Mathf.MoveTowards(post_exposure_origin, post_exposure_target, ratio);

    //        bloomSettings.intensity.value = currentIntensity;
    //        bloomSettings.threshold.value = currentThreshold;
    //        colorGradingSettings.postExposure.value = currentPostExposure;

    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    bloomSettings.intensity.value = intensity_target;
    //    bloomSettings.threshold.value = threshold_target;
    //    colorGradingSettings.postExposure.value = post_exposure_target;
    //}

    //public void RequstReturnPP(float time)
    //{
    //    StartCoroutine(ReturnPostProcess(time));
    //}

    //IEnumerator ReturnPostProcess(float changeTime)
    //{
    //    float currentTime = 0.0f;
    //    float currentIntensity = bloomSettings.intensity.GetValue<float>();
    //    float currentThreshold = bloomSettings.threshold.GetValue<float>();
    //    float currentPostExposure = colorGradingSettings.postExposure.GetValue<float>();

    //    while (currentTime <= changeTime)
    //    {
    //        float ratio = currentTime / changeTime;

    //        currentIntensity = Mathf.MoveTowards(intensity_target, intensity_origin, ratio);
    //        currentThreshold = Mathf.MoveTowards(threshold_target, threshold_origin, ratio);
    //        currentPostExposure = Mathf.MoveTowards(post_exposure_target,post_exposure_origin, ratio);

    //        bloomSettings.intensity.value = currentIntensity;
    //        bloomSettings.threshold.value = currentThreshold;
    //        colorGradingSettings.postExposure.value = currentPostExposure;

    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    bloomSettings.intensity.value = intensity_origin;
    //    bloomSettings.threshold.value = threshold_origin;
    //    colorGradingSettings.postExposure.value = post_exposure_origin;
    //}

    public bool GetIsLookingEvent()
    {
        return isLookingEvent;
    }
}
