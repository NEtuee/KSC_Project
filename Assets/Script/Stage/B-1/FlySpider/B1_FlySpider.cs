using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_FlySpider : ObjectBase
{
    public StateProcessor stateProcessor;
    public IKFootPointRotator footPointRotator;
    
    public float explosionRadius = 3f;
    public float explosionPower = 150f;
    public bool launch = false;

    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 direction;

    private PlayerUnit _player;

    private Vector3 _localPosition;
    private Quaternion _localRotation;


    public override void Assign()
    {
        base.Assign();

        stateProcessor.InitializeProcessor(this);

        _localPosition = transform.localPosition;
        _localRotation = transform.localRotation;

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            target = _player.transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!launch)
            return;

        stateProcessor.StateProcess(deltaTime);
    }

    public void Respawn()
    {
        transform.localPosition = _localPosition;
        transform.localRotation = _localRotation;

        footPointRotator.enabled = true;

        this.gameObject.SetActive(true);
        stateProcessor.StateChange("Idle");

        launch = false;
    }

    public void Explosion()
    {
        if(GetTargetDistance() <= explosionRadius)
        {
            var dir = (target.position - transform.position).normalized;
            _player.Ragdoll.ExplosionRagdoll(explosionPower,dir);
        }
        
        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);

        Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public float GetTargetDistance()
    {
        return Vector3.Distance(target.position,transform.position);
    }
}
