using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectTest_SpearEvent : MonoBehaviour
{
    public PlayerAnimCtrl animCtrl;

    public UnityEvent absorbEnd = new UnityEvent();
    public UnityEvent pierce = new UnityEvent();
    public UnityEvent pull = new UnityEvent();

    public Animator animator;

    void Start()
    {
        animCtrl.BindAbsorbEndEvent(absorbEnd);
        animCtrl.BindPierceEvent(pierce);
        animCtrl.BindPullEvent(pull);
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.Play("Ani_Character_SpecialSpear",0,0f);
            animCtrl.BindAbsorbEndEvent(absorbEnd);
            animCtrl.BindPierceEvent(pierce);
            animCtrl.BindPullEvent(pull);
        }
    }

}
