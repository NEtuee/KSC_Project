using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimControll : MonoBehaviour
{
    public List<Animator> animators;

    public void Start()
    {
        AnimatorOff();
    }

    public void AnimatorOn()
    {
        foreach(var anim in animators)
        {
            anim.enabled = true;
        }
    }

    public void AnimatorOff()
    {
        foreach(var anim in animators)
        {
            anim.enabled = false;
        }
    }
}
