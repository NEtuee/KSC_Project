using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaddyLongLegs_AI : IKBossBase
{
    public GraphAnimator animator;
    public Transform body;
    public Transform humanSpine;

    public void Start()
    {
        animator.Play("UpDownLoop",body);
        animator.Play("HumanRotateLoop",humanSpine);
    }
}
