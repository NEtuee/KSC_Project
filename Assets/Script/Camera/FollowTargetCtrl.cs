using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetCtrl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float yawRotateSpeed = 120f;
    [SerializeField] private float pitchRotateSpeed = 120f;
    [SerializeField] private float pitchLimitMin = -80f;
    [SerializeField] private float pitchLimitMax = 50f;
    [SerializeField] private float rotSmooth = 0.1f;
    [SerializeField] private float followSmooth = 8f;
    [SerializeField] private bool isPause;

    private Vector3 currentRot;
    private Vector3 targetRot;

    private float currentYawRotVelocity;
    private float currentPitchRotVelocity;

    private Vector3 smoothVelocity;

    [SerializeField]private bool updateMode = false;

    void Start()
    {
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        if(((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod == UpdateMethod.Update)
        {
            updateMode = true;
        }
        else
        {
            updateMode = false;
        }
    }

    void Update()
    {
        if (isPause == true)
        {
            return;
        }

        //transform.position = Vector3.SmoothDamp(transform.position, target.position + Vector3.up, ref smoothVelocity, 3.0f * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, Time.unscaledDeltaTime * followSmooth);
        //transform.position = target.position + Vector3.up;
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.deltaTime);


        float mouseX = InputManager.Instance.GetCameraAxisX();
        float mouseY = InputManager.Instance.GetCameraAxisY();

        targetRot.y += mouseX * yawRotateSpeed * Time.unscaledDeltaTime;
        targetRot.x += mouseY * pitchRotateSpeed * Time.unscaledDeltaTime;

        targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);

        currentRot.x = Mathf.SmoothDamp(currentRot.x, targetRot.x, ref currentPitchRotVelocity, rotSmooth);
        currentRot.y = Mathf.SmoothDamp(currentRot.y, targetRot.y, ref currentYawRotVelocity, rotSmooth);

        //currentRot.x = targetRot.x;
        //currentRot.y = targetRot.y;

        Quaternion localRotation = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
        transform.rotation = localRotation;
    }

    private void FixedUpdate()
    {
        if (updateMode == true)
            return;

        if (isPause == true)
        {
            return;
        }

        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, followSmooth * Time.fixedDeltaTime);
        //transform.position = Vector3.SmoothDamp(transform.position, target.position + Vector3.up, ref smoothVelocity,5.0f*Time.fixedDeltaTime);
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.fixedDeltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up, 8.0f * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (updateMode == false)
            return;

        if (isPause == true)
        {
            return;
        }

        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, followSmooth * Time.deltaTime);
        transform.position = target.position + Vector3.up;
        //transform.position = target.position + Vector3.up;
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.deltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up, 8.0f * Time.deltaTime);
    }

    public void Pause(){ isPause = true; }
    public void Resume() { isPause = false; }
}
