using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class PlayerAnimCtrl : MonoBehaviour
{
    //[SerializeField] private PlayerCtrl owner;
    [SerializeField] private PlayerCtrl_State owner;
    private Animator animator;
    private PlayerEffectCtrl effectCtrl;
    private HandIKCtrl handIKCtrl;

    private UnityEvent pierceEvent;
    private UnityEvent pullEvent;
    private UnityEvent absorbEndEvent;

    private CinemachineImpulseSource impulseSource;

    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [SerializeField] private List<string> runSoundStringList = new List<string>();
    [SerializeField] private List<string> walkSoundStringList = new List<string>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        effectCtrl = GetComponent<PlayerEffectCtrl>();
        handIKCtrl = GetComponent<HandIKCtrl>();
        //owner = GetComponent<PlayerCtrl>();
        owner = GetComponent<PlayerCtrl_State>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void StartClimbing()
    {
        //Debug.Log("Start");
        //animator.applyRootMotion = true;
        owner.SetIsMustClimbing(true);
        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsReverseClimbing", false);
        animator.SetBool("IsLedgeMoveLeft", false);
        animator.SetBool("IsLedgeMoveRight", false);

        if(handIKCtrl != null)
        {
            handIKCtrl.DisableLeftHandIk();
            handIKCtrl.DisableRightHandIk();
        }
    }

    private void EndClimbing()
    {
        //animator.applyRootMotion = false;
        //Debug.Log("End");
        owner.SetIsMustClimbing(false);
        animator.SetBool("IsLedgeMoving", false);
    }

    private void StartRolling()
    {
        owner.StartRolling();
    }

    private void EndRolling()
    {
        owner.ReleaseRolling();
    }

    private void StartLedgeClimbing()
    {
        //Debug.Log("StartLedgeClimbing");
        animator.applyRootMotion = true;
        owner.SetIsClimbingLedge(true);
    }

    private void EndLedgeClimbing()
    {
        animator.applyRootMotion = false;
        owner.SetIsClimbingLedge(false);
    }

    private void EnableGroundCheck()
    {
        //owner.SwitchGroundCheck(true);
    }

    private void HangingLedgeEvent()
    {
        owner.SaveHandPosition();
    }

    private void AdjustLedgeOffset()
    {
        owner.AdjustLedgeOffset();
    }

    private void CanInput()
    {
        owner.SetIsCanInputClimbing(true);
    }
    
    private void CantInput()
    {
        owner.SetIsCanInputClimbing(false);
    }

    private void StartSideClimbing()
    {
        animator.applyRootMotion = true;
        owner.SetIsMustClimbing(true);
        owner.SetIsCanInputClimbing(false);

        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsReverseClimbing", false);
        animator.SetBool("IsLedgeMoveLeft", false);
        animator.SetBool("IsLedgeMoveRight", false);
        animator.SetBool("IsLedgeMoving", false);

    }

    private void EndSideClimbing()
    {
        animator.applyRootMotion = false;
        owner.SetIsMustClimbing(false);
        owner.SetIsCanInputClimbing(true);
    }

    private void StartUpClimbing()
    {
        animator.applyRootMotion = true;
        owner.SetIsMustClimbing(true);
        owner.SetIsCanInputClimbing(false);

        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsReverseClimbing", false);
        animator.SetBool("IsLedgeMoveLeft", false);
        animator.SetBool("IsLedgeMoveRight", false);
        animator.SetBool("IsLedgeMoving", false);

        animator.SetBool("Left", !animator.GetBool("Left"));

        if (handIKCtrl != null)
        {
            handIKCtrl.DisableLeftHandIk();
            handIKCtrl.DisableRightHandIk();
        }
    }

    private void EndUpClimbing()
    {
        animator.applyRootMotion = false;
        owner.SetIsMustClimbing(false);
        owner.SetIsCanInputClimbing(true);
    }

    private void StartDownClimbing()
    {
        animator.applyRootMotion = true;
        owner.SetIsMustClimbing(true);
        owner.SetIsCanInputClimbing(false);

        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsReverseClimbing", false);
        animator.SetBool("IsLedgeMoveLeft", false);
        animator.SetBool("IsLedgeMoveRight", false);
        animator.SetBool("IsLedgeMoveRight", false);
        animator.SetBool("IsLedgeMoving", false);

        animator.SetBool("Left", !animator.GetBool("Left"));

    }

    private void EndDownClimbing()
    {
        animator.applyRootMotion = false;
        owner.SetIsMustClimbing(false);
        owner.SetIsCanInputClimbing(true);
    }

    private void StartLedgeMove()
    {
        animator.applyRootMotion = true;
        owner.SetIsMustClimbing(true);
        owner.SetIsCanInputClimbing(false);
        owner.SetIsLedgeSideMove(true);

        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsReverseClimbing", false);
        animator.SetBool("IsLedgeMoveLeft", false);
        animator.SetBool("IsLedgeMoveRight", false);
    }

    private void EndLedgeMove()
    {
        animator.applyRootMotion = false;
        owner.SetIsMustClimbing(false);
        owner.SetIsCanInputClimbing(true);
        owner.SetIsLedgeSideMove(false);

        animator.SetBool("IsLedgeMoving", false);
    }

    private void LeftWalkFootStep()
    {
        RandomWalkSoundPlay(leftFoot);
        //impulseSource.GenerateImpulse();
    }

    private void RightWalkFootStep()
    {
        RandomWalkSoundPlay(rightFoot);
        //impulseSource.GenerateImpulse();
    }

    private void LeftFootStep()
    {
        if(effectCtrl != null)
        {
            effectCtrl.PlayLeftFootStep();
        }
        RandomRunSoundPlay(leftFoot);

        //impulseSource.GenerateImpulse();
    }

    private void RightFootStep()
    {
        if (effectCtrl != null)
        {
            effectCtrl.PlayRightFootStep();
        }
        RandomRunSoundPlay(rightFoot);
        //impulseSource.GenerateImpulse();
    }

    private void StartAbsorb()
    {
        owner.ActiveSetSpecialSpearVisual_Absorb(true);
    }

    private void PierceSpear()
    {
        pierceEvent?.Invoke();
    }

    private void PullSpear()
    {
        pullEvent?.Invoke();
    }

    private void EndAbsorb()
    {
        owner.ActiveSetSpecialSpearVisual_Absorb(false);

        owner.EndAbsorb();
        absorbEndEvent?.Invoke();

        pierceEvent = null;
        pullEvent = null;
        absorbEndEvent = null;
    }

    private void EndSttager()
    {
        owner.EndStagger();
    }

    private void LeftHandTrace()
    {
        if(handIKCtrl != null)
        {
            //Debug.Log("LeftTrace");
            handIKCtrl.EnableLeftTrace();
        }
    }

    private void RightHandTrace()
    {
        if(handIKCtrl != null)
        {
            //Debug.Log("RightTrace");
            handIKCtrl.EnableRightTrace();
        }
    }

    private void StartRopeClimbing(int left)
    {
        if(left == 1)
        {
            owner.SetRopeHandInfo(true);
        }
        else
        {
            owner.SetRopeHandInfo(false);
        }
    }

    private void EndRopeClimbing()
    {
        owner.SetIsCanInputClimbing(true);
    }

    private void EndTurnBack()
    {
        owner.ChangeState(PlayerCtrl_State.PlayerState.Default);
    }

    public void BindPierceEvent( UnityEvent pierceEvent)
    {
        this.pierceEvent = pierceEvent;
    }

    public void BindPullEvent(UnityEvent pullEvent)
    {
        this.pullEvent = pullEvent;
    }
    
    public void BindAbsorbEndEvent(UnityEvent absorbEvent)
    {
        this.absorbEndEvent = absorbEvent;
    }
    
    private void RandomRunSoundPlay(Transform transform)
    {
        int randResult = Random.Range(0, runSoundStringList.Count);
        if(AudioManager.instance != null)
            AudioManager.instance.Play(runSoundStringList[randResult], transform.position);
    }

    private void RandomWalkSoundPlay(Transform transform)
    {
        int randResult = Random.Range(0, walkSoundStringList.Count);
        if (AudioManager.instance != null)
            AudioManager.instance.Play(walkSoundStringList[randResult], transform.position);
    }

    private void EndGetUp()
    {
        GameManager.Instance.PauseControl(false);

    }
}
