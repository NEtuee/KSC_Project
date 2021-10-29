using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimationControl : MonoBehaviour
{
    public Animator animator;

    public void ChangeAnimation(int code)
    {
        animator.SetTrigger("Change");
        animator.SetInteger("Target",code);
    }

    public void SetOpenPose()
    {
        animator.SetTrigger("First");
        animator.SetBool("IsOpen",true);
    }

    public void SetClosePose()
    {
        animator.SetTrigger("First");
        animator.SetBool("IsOpen",false);
    }

    public void Open()
    {
        ChangeAnimation(0);
    }

    public void Close()
    {
        ChangeAnimation(1);
    }
}
