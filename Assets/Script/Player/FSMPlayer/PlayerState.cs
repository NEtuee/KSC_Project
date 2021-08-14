using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public abstract void Enter(PlayerUnit playerUnit);

    public abstract void UpdateState(PlayerUnit playerUnit);

    public abstract void AnimatorMove(Animator animator);

    public abstract void Exit(PlayerUnit playerUnit);
}
