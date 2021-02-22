using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTestAnimScript : MonoBehaviour
{
    private Animator anim;
    private float blend;

    void Start()
    {
        anim = GetComponent<Animator>();
        blend = 0.0f;
    }

    void Update()
    {
        //if(Input.GetKey(KeyCode.W))
        //{
        //    anim.SetBool("IsWalk", true);
        //}
        //else
        //{
        //    anim.SetBool("IsWalk", false);
        //}

        if(Input.GetKey(KeyCode.W))
        {
            blend = Mathf.MoveTowards(blend, 1.0f, 3f * Time.deltaTime);
            blend = Mathf.Clamp(blend, 0.0f, 1.0f);
            anim.SetFloat("Blend", blend);
            anim.SetBool("IsWalk", true);
        }
        else
        {
            anim.SetBool("IsWalk", false);
            blend = Mathf.MoveTowards(blend, 0.0f, 3f * Time.deltaTime);
            blend = Mathf.Clamp(blend, 0.0f, 1.0f);
            anim.SetFloat("Blend", blend);
        }
    }
}
