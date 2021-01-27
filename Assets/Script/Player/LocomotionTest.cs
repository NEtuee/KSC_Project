using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionTest : MonoBehaviour
{
    private Animator animator;
    private Transform mainCam;

    private float inputValue;

    private Vector3 camForward;
    private Vector3 camRight;

    private Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCam = Camera.main.transform;

        inputValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizon = Input.GetAxis("Horizontal");

        animator.SetFloat("InputVertical", vertical);
        animator.SetFloat("InputHorizon", horizon);

        camForward = mainCam.forward;
        camRight = mainCam.right;
        camForward.y = 0;
        camRight.y = 0;

        moveDir = (camForward * vertical) + (camRight * horizon);
        moveDir.Normalize();

        //if (vertical != 0.0f || horizon != 0.0f)
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        inputValue = Mathf.MoveTowards(inputValue, 1f, 1f * Time.deltaTime);
        //    }
        //    else
        //    {
        //        inputValue = Mathf.MoveTowards(inputValue, 0.5f, 1f * Time.deltaTime);
        //    }
        //}
        //else
        //{
        //    inputValue = Mathf.MoveTowards(inputValue, 0.0f, 1f * Time.deltaTime);
        //}

        //animator.SetFloat("Input", inputValue);

        if(moveDir != Vector3.zero)
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 6.0f);
    }
}
