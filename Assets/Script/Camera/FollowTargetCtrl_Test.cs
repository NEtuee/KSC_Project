using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowTargetCtrl_Test : UnTransfromObjectBase
{
    [SerializeField] private bool visible = true;
    public bool Visible => visible;
    [SerializeField] private Transform target;
    [SerializeField] private float yawRotateSpeed = 120f;
    [SerializeField] private float pitchRotateSpeed = 120f;
    [SerializeField] private float pitchLimitMin = -80f;
    [SerializeField] private float pitchLimitMax = 50f;
    [SerializeField] private float rotSmooth = 0.1f;
    [SerializeField] private float followSmooth = 8f;
    [SerializeField] private bool isPause;

    private float _mouseX;
    private float _mouseY;

    public float YawRotateSpeed
    {
        get => yawRotateSpeed;
        set => yawRotateSpeed = value;
    }

    public float PitchRotateSpeed
    {
        get => pitchRotateSpeed;
        set => pitchRotateSpeed = value;
    }

    private Vector3 currentRot;
    private Vector3 targetRot;

    private float currentYawRotVelocity;
    private float currentPitchRotVelocity;

    private Vector3 smoothVelocity;

    [SerializeField]private bool updateMode = false;

    public override void Assign()
    {
        base.Assign();

    }

    public override void Initialize()
    {
        base.Initialize();

        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
    }


    void Update()
    {
        //if (updateMode == false)
        //    return;

        //if (!visible)
        //    return;
        ////if (Input.GetKeyDown(KeyCode.Alpha0))
        ////{
        ////    transform.rotation = Camera.main.transform.rotation;
        ////    transform.position = Camera.main.transform.position;
        ////    currentRot = Camera.main.transform.rotation.eulerAngles;
        ////    targetRot = Camera.main.transform.rotation.eulerAngles;
        ////    Resume();
        ////}

        ////transform.position = Vector3.SmoothDamp(transform.position, target.position + Vector3.up, ref smoothVelocity, 3.0f * Time.deltaTime);
        ////transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, Time.unscaledDeltaTime * followSmooth);
        ////transform.position = target.position + Vector3.up;
        ////transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.deltaTime);



        ////float mouseX = InputManager.Instance.GetCameraAxisX();
        ////float mouseY = InputManager.Instance.GetCameraAxisY();

        //targetRot.y += _mouseX * yawRotateSpeed * Time.unscaledDeltaTime;
        //targetRot.x += _mouseY * pitchRotateSpeed * Time.unscaledDeltaTime;

        //targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);

        //currentRot.x = Mathf.SmoothDamp(currentRot.x, targetRot.x, ref currentPitchRotVelocity, rotSmooth);
        //currentRot.y = Mathf.SmoothDamp(currentRot.y, targetRot.y, ref currentYawRotVelocity, rotSmooth);

        ////currentRot.x = targetRot.x;
        ////currentRot.y = targetRot.y;

        //Quaternion localRotation = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
        //transform.rotation = localRotation;
    }

    private void FixedUpdate()
    { 

        if (!visible)
            return;

        transform.position = target.position + Vector3.up;

        //float mouseX = InputManager.Instance.GetCameraAxisX();
        //float mouseY = InputManager.Instance.GetCameraAxisY();

        targetRot.y += _mouseX * yawRotateSpeed * Time.fixedUnscaledDeltaTime;
        targetRot.x += _mouseY * pitchRotateSpeed * Time.fixedUnscaledDeltaTime;

        targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);

        currentRot.x = Mathf.SmoothDamp(currentRot.x, targetRot.x, ref currentPitchRotVelocity, rotSmooth);
        currentRot.y = Mathf.SmoothDamp(currentRot.y, targetRot.y, ref currentYawRotVelocity, rotSmooth);

        //currentRot.x = targetRot.x;
        //currentRot.y = targetRot.y;

        Quaternion localRotation = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
        transform.rotation = localRotation;

        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, followSmooth * Time.fixedDeltaTime);
        //transform.position = Vector3.SmoothDamp(transform.position, target.position + Vector3.up, ref smoothVelocity,5.0f*Time.fixedDeltaTime);
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.fixedDeltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up, 8.0f * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        //if (updateMode == false)
        //    return;

        ////transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, followSmooth * Time.deltaTime);
        ////transform.position = target.position + Vector3.up + Vector3.up * ((PlayerCtrl_Ver2)GameManager.Instance.player).GetFootStepOffset() * 0.1f;
        //transform.position = target.position + Vector3.up;
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.deltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up, 8.0f * Time.deltaTime);
    }

    public void Pause(){ isPause = true; }
    public void Resume() { isPause = false; }

    public void SetForceRotation(Vector3 rot)
    {
        currentRot = rot;
        targetRot = rot;
    }

    public void SetYawRotateSpeed(float speed)
    {
        yawRotateSpeed = speed;
    }

    public void SetPitchRotateSpeed(float speed)
    {
        pitchRotateSpeed = speed;
    }

    public void SetPitchYaw(float pitch, float yaw)
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
    }

    public void SetPitchYawPosition(float pitch, float yaw, Vector3 position)
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
        transform.position = position;
    }

    public void OnCamera(InputAction.CallbackContext value)
    {
        Vector2 inputVector = value.ReadValue<Vector2>();
        _mouseY = inputVector.y;
        _mouseX = inputVector.x;
    }
}
