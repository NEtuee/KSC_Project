using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Spider : PathfollowObjectBase
{
    public LayerMask groundLayer;
    public StateProcessor stateProcessor;
    public GraphAnimator graphAnimator;
    public Transform body;
    public Transform target;

    public float explosionCheckRadius = 3f;
    public float explosionRadius = 5f;

    public bool launch = false;

    public Rigidbody shell;
    public Collider shellCollider;

    private Vector3 _shellPosition;
    private PlayerUnit _player;

    private Vector3 _localPosition;
    private Quaternion _localRotation;

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

        _shellPosition = shell.transform.localPosition;
        _localPosition = transform.localPosition;
        _localRotation = transform.localRotation;
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        Respawn();
    }

    public void Respawn()
    {
        shell.isKinematic = true;
        shell.transform.SetParent(transform);
        shell.transform.localPosition = _shellPosition;
        
        transform.localPosition = _localPosition;
        transform.localRotation = _localRotation;

        shellCollider.enabled = false;

        stateProcessor.StateChange("Idle");
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!launch)
            return;

        stateProcessor.StateProcess(deltaTime);
        graphAnimator.Progress(deltaTime);

        var dist = Vector3.Distance(transform.position,target.position);
        if(dist <= explosionCheckRadius && stateProcessor.currentState != "HitBack"
            && stateProcessor.currentState != "ExplosionWait"
            && stateProcessor.currentState != "MoveForward")
        {
            stateProcessor.StateChange("ExplosionWait");
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
        if(GetTargetDistance() <= explosionRadius)
        {
            _player.Ragdoll.ExplosionRagdoll(force,dir);
        }
        
    }

    public void ChangeAnimation(string key)
    {
        graphAnimator.Play(key,body);
    } 
}
