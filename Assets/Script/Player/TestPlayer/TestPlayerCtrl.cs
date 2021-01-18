using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerCtrl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20.0f;

    [SerializeField] private bool useGravity = true;

    private Vector3 moveDir;

    private float inputVertical;
    private float inputHorizontal;

    private Vector3 camForward;
    private Vector3 camRight;

    private Transform mainCameraTrasform;
    private CharacterController controller;
    void Start()
    {
        mainCameraTrasform = Camera.main.transform;
        moveDir = Vector3.zero;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");

        camForward = mainCameraTrasform.forward;
        camRight = mainCameraTrasform.right;
        camForward.y = 0;
        camRight.y = 0;

        if (useGravity)
        {
            if (controller.isGrounded)
            {
                moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                moveDir.Normalize();
                moveDir *= moveSpeed;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    moveDir.y = jumpForce;
                }
            }

            moveDir.y -= gravity * Time.deltaTime;

            controller.Move(moveDir * Time.deltaTime);
        }
        else
        {
            moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
            moveDir.Normalize();
            moveDir *= moveSpeed;

            if (Input.GetKey(KeyCode.Space))
            {
                moveDir.y = jumpForce;
            }
            else
            {
                moveDir.y = -jumpForce;
            }

            controller.Move(moveDir * Time.deltaTime);
        }
    }
}
