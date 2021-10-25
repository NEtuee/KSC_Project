using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Spider : PathfollowObjectBase
{
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public StateProcessor stateProcessor;
    public GraphAnimator graphAnimator;
    public Transform body;
    public Transform target;

    public Core core;

    public float explosionCheckRadius = 3f;
    public float explosionRadius = 5f;

    public bool launch = false;
    public bool setTargetToPlayer = true;

    public Rigidbody shell;
    public Collider shellCollider;

    private Vector3 _shellPosition;
    private PlayerUnit _player;

    private Vector3 _localPosition;
    private Quaternion _localRotation;

    [HideInInspector]public Vector3 backDirection;

    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.object_kick,(x)=>{
            var dir = Vector3.ProjectOnPlane(transform.position - ((PlayerUnit)x.data).transform.position,Vector3.up).normalized;
            HitBack(dir);
        });

        AddAction(MessageTitles.customTitle_start + 2,(x)=>{
            if(!gameObject.activeInHierarchy)
                return;

            var data = MessageDataPooling.CastData<MD.Vector3Data>(x.data);
            var dist = Vector3.Distance(transform.position,data.value);

            if(dist <= 5f)
            {
                stateProcessor.StateChange("HitBack");
            }
            
        });

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            if(setTargetToPlayer)
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
        
        //Respawn();
    }

    public void Respawn()
    {
        if(shell == null)
            shell = shellCollider.GetComponent<Rigidbody>();

        shell.isKinematic = true;
        shell.transform.SetParent(transform);
        shell.transform.localPosition = _shellPosition;
        
        transform.localPosition = _localPosition;
        transform.localRotation = _localRotation;

        shellCollider.enabled = false;
        core.Reactive();

        this.gameObject.SetActive(true);
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

    public void HitBack(Vector3 direction)
    {
        backDirection = direction;
        stateProcessor.StateChange("HitBack");

        var data = MessageDataPooling.GetMessageData<MD.Vector3Data>();
        data.value = transform.position;
        SendBroadcastMessage(MessageTitles.customTitle_start + 2, data, true);
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
        var playerDist = Vector3.Distance(_player.transform.position,transform.position);
        if(playerDist <= explosionRadius)
        {
            _player.Ragdoll.ExplosionRagdoll(force,dir);
        }

        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);        
    }

    public void Launch()
    {
        launch = true;
        stateProcessor.StateChange("Turn");
    }

    public void ChangeAnimation(string key)
    {
        graphAnimator.Play(key,body);
    } 
}
