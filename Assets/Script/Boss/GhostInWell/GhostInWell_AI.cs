using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInWell_AI : PathfollowObjectBase
{
    public Animator animator;
    public PlayerUnit target;

    public Transform recognizeStartPoint;
    public StateProcessor stateProcessor;

    public bool passageCheck = false;

    public List<IKLegMovement> arms = new List<IKLegMovement>();

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.set_setplayer,(x)=>{
            target = (PlayerUnit)x.data;
        });

        AddAction(MessageTitles.player_EMPHit,(x)=>{
            if(stateProcessor.currentState == "RandomMove")
            {
                stateProcessor.StateChange("ChaseMove");
            }
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        stateProcessor.StateChange("RandomMove");
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(target == null)
            return;
        stateProcessor.StateProcess(deltaTime);
    }

    public void EnableMovement()
    {
        foreach(var arm in arms)
        {
            arm.enabled = true;
        }
    }

    public void DisableMovement()
    {
        foreach(var arm in arms)
        {
            arm.DisableMovement();
        }
    }

    public void EnterPassage()
    {
        if((stateProcessor.currentState == "ChaseMove" || stateProcessor.currentState == "MeleeAttack"))
        {
            stateProcessor.StateChange("MoveToPassage");
        }
    }

    public void StayPassage()
    {
        passageCheck = true;
        if((stateProcessor.currentState == "ChaseMove" || stateProcessor.currentState == "MeleeAttack"))
        {
            stateProcessor.StateChange("MoveToPassage");
        }
        // if(stateProcessor.currentState == "ChaseMove" || stateProcessor.currentState == "MeleeAttack")
        // {
        //     stateProcessor.StateChange("MoveToPassage");
        // }
    }

    public void ChaseMove()
    {
        stateProcessor.StateChange("ChaseMove");
    }

    public void HeadOut()
    {
        stateProcessor.StateChange("HeadOut");
    }

    public void Dead()
    {
        DisableMovement();
        animator.enabled = false;
        this.enabled = false;
    }

    public void ExitPassage()
    {
        passageCheck = false;
        // if(stateProcessor.currentState == "ChaseMove" || stateProcessor.currentState == "MeleeAttack")
        // {
        //     stateProcessor.StateChange("MoveToPassage");
        // }
    }

    public bool CheckTargetInArea(float recogAngle, float recogDist)
    {
        var dir = (target.transform.position - recognizeStartPoint.position).normalized;
        var dist = Vector3.Distance(target.transform.position, recognizeStartPoint.position);
        var angle = MathEx.abs(Vector3.SignedAngle(recognizeStartPoint.forward,dir,transform.up));

        return (angle <= recogAngle) && (dist <= recogDist);
    }

    public bool CheckTargetInAreaCenter(float recogAngle, float recogDist)
    {
        var dir = (target.transform.position - recognizeStartPoint.position).normalized;
        var dist = Vector3.Distance(target.transform.position, Vector3.zero);
        var angle = MathEx.abs(Vector3.SignedAngle(recognizeStartPoint.forward,dir,transform.up));

        return (angle <= recogAngle) && (dist <= recogDist);
    }

    public void AnimationChange(int code)
    {
        animator.SetTrigger("Change");
        animator.SetInteger("Animation",code);
    }
}
