using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimationControl : MonoBehaviour
{
    public Animator animator;
    public GameObject lightParent;

    public MeshRenderer[] lightMesh;

    public Material lightOnMat;
    public Material lightOffMat;

    public bool autoProc = false;

    private bool _move = false;
    private bool _active = false;
    private bool _current = false;

    public void Update()
    {
        if (!autoProc)
            return;

        if(!_move)
        {
            if(_active != _current)
            {
                if (_active)
                    Open();
                else
                    Close();
            }
        }
    }

    public void SetActive(bool value)
    {
        _active = value;
    }

    public void EndMove()
    {
        _move = false;
    }

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

    public void SetLight(bool value)
    {
        //lightParent.SetActive(value);

        foreach(var item in lightMesh)
        {
            item.material = value ? lightOnMat : lightOffMat;
        }
    }

    public void Open()
    {
        _move = true;
        _current = true;

        ChangeAnimation(0);
    }

    public void Close()
    {
        _move = true;
        _current = false;

        ChangeAnimation(1);
    }
}
