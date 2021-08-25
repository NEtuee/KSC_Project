using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState : MonoBehaviour
{

    public abstract void Enter(PlayerUnit playerUnit, Animator animator);

    public abstract void UpdateState(PlayerUnit playerUnit, Animator animator);

    public abstract void AnimatorMove(PlayerUnit playerUnit, Animator animator);

    public abstract void FixedUpdateState(PlayerUnit playerUnit, Animator animator);

    public abstract void Exit(PlayerUnit playerUnit, Animator animator);

    public virtual void OnJump(PlayerUnit playerUnit, Animator animator)
    {
    }

    public virtual void OnAim(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
    }

    public virtual void OnShot(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
    }
}
