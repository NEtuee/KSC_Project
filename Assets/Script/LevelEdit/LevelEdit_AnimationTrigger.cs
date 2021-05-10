using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_AnimationTrigger : LevelEdit_Trigger
{
    public Animator animatorControll;
    public Animation animationControll;

    public void PlayMainClip()
    {
        animationControll.Play();
    }

    public void PlayTarget(string target)
    {
        animationControll.Play(target);
    }

    public void SetTrigger(string target)
    {
        animatorControll.SetTrigger(target);
    }

    public void SetBool(string target)
    {
        animatorControll.SetBool(target,!animatorControll.GetBool(target));
    }
}
