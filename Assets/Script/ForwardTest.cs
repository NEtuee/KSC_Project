using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardTest : MonoBehaviour
{
    CharacterController controller;

    public void Start()
    {
        controller = GetComponent<CharacterController>();

    }
    void FixedUpdate()
    {
        controller.Move(transform.forward * Time.deltaTime);
    }
}