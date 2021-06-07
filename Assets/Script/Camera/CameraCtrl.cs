using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public enum CamMode
{
    Default,
    Aim
}

public class CameraCtrl : MonoBehaviour
{
    public Transform player;
    private PlayerCtrl playerCtrl;

    [SerializeField] private bool isPause;
    [SerializeField] private bool isLookingEvent;
    [SerializeField] private Transform defaultPos;
    [SerializeField] private Transform aimPos;
    [SerializeField] private CamMode camMode = CamMode.Default;
    [SerializeField] private float yawRotateSpeed = 15f;
    [SerializeField] private float pitchRotateSpeed = 15f;
    [SerializeField] private float pitchLimitMin = -13f;
    [SerializeField] private float pitchLimitMax = 40f;
    [SerializeField] private float rotSmooth = 0.2f;
    [SerializeField] private float followSmooth = 8f;
    private float originFollowSmooth;
    float currentYawRotVelocity;
    float currentPitchRotVelocity;

    private Transform mainCamera;

    private Transform lookingEventTarget = null;
    private Quaternion prevRot;
    private bool isReturnRot;

    Vector3 currentRot;
    Vector3 targetRot;

    Vector3 targetPos;

    [Header("Adjust CameraFollow")]
    [SerializeField] [Range(-10.0f, 10.0f)] private float upAdjust = 0.0f;
    [SerializeField] [Range(-10.0f, 10.0f)] private float rightAdjust = 0.0f;
    [SerializeField] [Range(-10.0f, 10.0f)] private float forwardAdjust = 0.0f;

    [SerializeField] private Transform tempHead;


    private Camera currentCamera;
   // private PostProcessVolume postProcessVolume;

    void Start()
    {
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
        currentCamera = Camera.main;
        mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originFollowSmooth = followSmooth;

        if (player.GetComponent<PlayerCtrl>() != null)
        {
            playerCtrl = player.GetComponent<PlayerCtrl>();
        }

        //postProcessVolume = GetComponent<PostProcessVolume>();
    }

    void Update()
    {
        if (isPause == true)
        {
            return;
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}

        //if(Input.GetKeyUp(KeyCode.Escape))
        //{
      
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}

        //if(Input.GetKeyDown(KeyCode.L))
        //{
        //    LookingEvent(tempHead, 3f);
        //}
    }

    private void LateUpdate()
    {
        if(isPause == true)
        {
            return;
        }

        targetPos = player.position + (transform.right * rightAdjust) + (transform.up * upAdjust) + (transform.forward * forwardAdjust);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * followSmooth);

        if (isLookingEvent == false)
        {
            float mouseX = InputManager.Instance.GetCameraAxisX();
            float mouseY = InputManager.Instance.GetCameraAxisY();

            targetRot.y += mouseX * yawRotateSpeed * Time.unscaledDeltaTime;
            targetRot.x += mouseY * pitchRotateSpeed * Time.unscaledDeltaTime;

            if (camMode == CamMode.Default)
                targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);
            else
                targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);

            if (camMode != CamMode.Aim)
            {
                currentRot.x = Mathf.SmoothDamp(currentRot.x, targetRot.x, ref currentPitchRotVelocity, rotSmooth);
                currentRot.y = Mathf.SmoothDamp(currentRot.y, targetRot.y, ref currentYawRotVelocity, rotSmooth);
            }
            else
            {
                currentRot.x = targetRot.x;
                currentRot.y = targetRot.y;
            }

            Quaternion localRotation = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);

            if(isReturnRot == true)
            {
                float angleGap = Quaternion.Angle(transform.rotation, prevRot);
                if(angleGap > 0.1f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, prevRot, 6f * Time.deltaTime);
                }
                else
                {
                    isReturnRot = false;
                }
            }
            else
            {
                transform.rotation = localRotation;
            }

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingEventTarget.position - transform.position,Vector3.up),3f*Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {

        //if (playerCtrl != null)
        //{
        //    if (playerCtrl.GetPlayerBehaviorState() == PlayerBehaviorState.Ground)
        //    {
        //        transform.position = player.position;
        //    }
        //    else
        //    {
        //        transform.position = Vector3.Lerp(transform.position, player.position, Time.fixedUnscaledDeltaTime * 5.0f);
        //    }
        //}
        //else
        //{
        //    transform.position = Vector3.Lerp(transform.position, player.position, Time.fixedUnscaledDeltaTime * 5.0f);
        //}

        ////transform.position = Vector3.Lerp(transform.position, player.position, Time.unscaledDeltaTime * 5.0f);
        ////transform.position = player.position;
        ////transform.position = Vector3.Lerp(transform.position, player.position, Time.unscaledDeltaTime*5.0f);


        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = Input.GetAxis("Mouse Y");

        //targetRot.y += mouseX * yawRotateSpeed * Time.fixedUnscaledDeltaTime;
        //targetRot.x += mouseY * pitchRotateSpeed * Time.fixedUnscaledDeltaTime;

        //if (camMode == CamMode.Default)
        //    targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);
        //else
        //    targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin - 30f, pitchLimitMax + 30f);

        //if (camMode != CamMode.Aim)
        //{
        //    currentRot.x = Mathf.SmoothDamp(currentRot.x, targetRot.x, ref currentPitchRotVelocity, rotSmooth);
        //    currentRot.y = Mathf.SmoothDamp(currentRot.y, targetRot.y, ref currentYawRotVelocity, rotSmooth);
        //}
        //else
        //{
        //    currentRot.x = targetRot.x;
        //    currentRot.y = targetRot.y;
        //}

        //Quaternion localRotation = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
        ////Debug.Log(currentRot.x);
        //transform.rotation = localRotation;
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    public void SetCamMode(CamMode camMode)
    {
        this.camMode = camMode;
    }

    public Camera GetCurrentCamera()
    {
        return currentCamera;
    }

    public CamMode GetCameMode()
    {
        return camMode;
    }

    public void SetFollowSmooth(float value)
    {
        followSmooth = value;
    }

    public void ResetFollowSmooth()
    {
        followSmooth = originFollowSmooth;
    }

    //public void ChangePostProcessProfile(PostProcessProfile profile)
    //{
    //    postProcessVolume.profile = profile;
    //}

    public void LookingEvent(Transform target, float lookingTime)
    {
        if (target == null)
        {
            return;
        }

        prevRot = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
        lookingEventTarget = target;
        StartCoroutine(Looking(lookingTime));
    }

    public void LookingEvent(Transform target)
    {
        if (target == null)
        {
            return;
        }

        prevRot = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
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
        isReturnRot = true;
    }

    public void Pause() { isPause = true; }
    public void Resume() { isPause = false; }

    public void SetTarget(Transform target)
    {
        player = target;
    }
}
