using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEndChagneStateBehavior : StateMachineBehaviour
{
    public enum PlayerStateEnum
    {
        Default,
        Jump,
        RunToStop,
        Aiming,
        Grab,
        ReadyGrab,
        HangLedge,
        LedgeUp,
        ClimbingJump,
        ClimbingUpperLine,
        ReadyClimbingJump,
        Ragdoll,
        HighLandingState,
        Dash,
        DashEnd,
        Dead
    }

    private PlayerUnit _playerUnit;
    public PlayerStateEnum changeState;
    private void Awake()
    {
        _playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        switch(changeState)
        {
            case PlayerStateEnum.Default:
                _playerUnit.ChangeState(PlayerUnit.defaultState);
                break;
            case PlayerStateEnum.Jump:
                _playerUnit.ChangeState(PlayerUnit.defaultState);
                break;
            case PlayerStateEnum.RunToStop:
                _playerUnit.ChangeState(PlayerUnit.runToStopState);
                break;
            case PlayerStateEnum.Aiming:
                _playerUnit.ChangeState(PlayerUnit.aimingState);
                break;
            case PlayerStateEnum.Grab:
                _playerUnit.ChangeState(PlayerUnit.grabState);
                break;
            case PlayerStateEnum.ReadyGrab:
                _playerUnit.ChangeState(PlayerUnit.readyGrabState);
                break;
            case PlayerStateEnum.HangLedge:
                _playerUnit.ChangeState(PlayerUnit.hangLedgeState);
                break;
            case PlayerStateEnum.LedgeUp:
                _playerUnit.ChangeState(PlayerUnit.ledgeUpState);
                break;
            case PlayerStateEnum.ClimbingJump:
                _playerUnit.ChangeState(PlayerUnit.climbingJumpState);
                break;
            case PlayerStateEnum.ClimbingUpperLine:
                _playerUnit.ChangeState(PlayerUnit.climbingUpperLineState);
                break;
            case PlayerStateEnum.ReadyClimbingJump:
                _playerUnit.ChangeState(PlayerUnit.readyClimbingJumpState);
                break;
            case PlayerStateEnum.Ragdoll:
                _playerUnit.ChangeState(PlayerUnit.ragdollState);
                break;
            case PlayerStateEnum.HighLandingState:
                _playerUnit.ChangeState(PlayerUnit.highLandingState);
                break;
            case PlayerStateEnum.Dash:
                _playerUnit.ChangeState(PlayerUnit.dashState);
                break;
            case PlayerStateEnum.DashEnd:
                _playerUnit.ChangeState(PlayerUnit.dashEndState);
                break;
            case PlayerStateEnum.Dead:
                _playerUnit.ChangeState(PlayerUnit.deadState);
                break;
        }
    }
}
