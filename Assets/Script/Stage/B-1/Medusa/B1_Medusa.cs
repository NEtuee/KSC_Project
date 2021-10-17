using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Medusa : PathfollowObjectBase
{
    public LayerMask groundLayer;
    public StateProcessor stateProcessor;
    public Animator animator;

    public Transform target;

    public float pushDist = 4f;
    public bool launch = false;

    public List<IKLegMovement> legMovements = new List<IKLegMovement>();

    private PlayerUnit _player;

    private Vector3 _localPosition;
    private Quaternion _localRotation;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    //private bool _layerWeight = false;

    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.object_kick,(x)=>{
            stateProcessor.StateChange("HitBack");
        });

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            target = _player.transform;
        });

        AddAction(MessageTitles.stage_beforePlayerRespawn,(x)=>{
            TargetFall();
        });

        _localPosition = transform.localPosition;
        _localRotation = transform.localRotation;

        _timeCounter.CreateSequencer("armWeight");
        _timeCounter.AddSequence("armWeight",1f,(x)=>{animator.SetLayerWeight(2,1f);},null);
        _timeCounter.AddSequence("armWeight",1f,(x)=>{animator.SetLayerWeight(2,x);},null);
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        Respawn();
        

        _timeCounter.InitSequencer("armWeight");
        _timeCounter.SkipSequencer("armWeight",2f);
    }

    public void SetIK(bool value)
    {
        foreach(var leg in legMovements)
        {
            leg.iKFabric.enabled = value;
        }
    }

    public void Respawn()
    {
        transform.localPosition = _localPosition;
        transform.localRotation = _localRotation;

        stateProcessor.StateChange("Idle");

        SetIK(false);
    }

    public void Dead()
    {
        if(stateProcessor.currentState != "Dead")
        {
            stateProcessor.StateChange("Dead");
        }
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!launch)
            return;

        stateProcessor.StateProcess(deltaTime);

        if(stateProcessor.currentState != "HitTarget")
        {
            if(pushDist >= GetTargetDistance())
            {
                stateProcessor.StateChange("HitTarget");
            }
        }

        _timeCounter.ProcessSequencer("armWeight",deltaTime);
    }

    public void Launch()
    {
        launch = true;
        stateProcessor.StateChange("Transformation");
    }

    public void TargetFall()
    {
        if(stateProcessor.currentState == "Idle" ||stateProcessor.currentState == "March" || stateProcessor.currentState == "HitTarget")
        {
            stateProcessor.StateChange("MoveBack");
        }
    }

    public float GetTargetDistance()
    {
        return Vector3.Distance(target.position,transform.position);
    }

    public Vector3 GetTargetDirection()
    {
        return (target.position - transform.position).normalized;
    }

    public void Explosion(Vector3 dir,float force)
    {
        _player.Ragdoll.ExplosionRagdoll(force,dir);
    }

    public void ChangeMainAnimation(int key)
    {
        animator.SetTrigger("MainChange");
        animator.SetInteger("MainState",key);
    }

    public void ChangeArmStateAnimation(int key)
    {
        animator.SetTrigger("ArmChange");
        animator.SetInteger("ArmState",key);
    } 

    public void ChangeArmPushAnimation(int key)
    {
        animator.SetTrigger("ArmPushChange");
        animator.SetInteger("ArmPush",key);

        _timeCounter.InitSequencer("armWeight");
    } 
}
