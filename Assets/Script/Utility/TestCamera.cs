using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float yawRotateSpeed = 15f;
    [SerializeField] private float pitchRotateSpeed = 15f;
    [SerializeField] private float pitchLimitMin = -13f;
    [SerializeField] private float pitchLimitMax = 40f;

    Vector3 currentRot;
    Vector3 targetRot;
    void Start()
    {
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
    }

    void Update()
    {
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 moveDir = transform.forward * inputVertical + transform.right * inputHorizontal;
        moveDir.Normalize();

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        targetRot.y += mouseX * yawRotateSpeed * Time.unscaledDeltaTime;
        targetRot.x += mouseY * pitchRotateSpeed * Time.unscaledDeltaTime;

        currentRot.x = targetRot.x;
        currentRot.y = targetRot.y;

        Quaternion localRotation = Quaternion.Euler(currentRot.x, currentRot.y, 0.0f);
        transform.rotation = localRotation;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}
