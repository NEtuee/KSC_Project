using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_RagdollCollider : UnTransfromObjectBase
{
    public enum EventType
    {
        Collision,
        Grab
    }

    public enum RagdollType
    {
        Default,
        ElectricShock,
    }

    public enum HitForcePointType
    {
        CollisionPoint,
        CenterPosition,
    }

    public EventType eventType;
    public RagdollType ragdollType;
    public HitForcePointType hitForcePointType;
    public LayerMask targetLayer;

    public float hitDamage = 5f;
    public float shockTime = 1f;
    public float hitForce = 10f;
    public float collisionDistance = 1f;

    public bool collisionEvent = false;

    public bool deleteY = false;

    private Collider _myCollider;
    private PlayerUnit _player;
    private PlayerRagdoll _ragdoll;

    public override void Assign()
    {
        base.Assign();
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            _ragdoll = _player.GetComponent<PlayerRagdoll>();
        });

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);

        _myCollider = GetComponent<Collider>();
    }


    public override void Progress(float deltaTime)
    {
        if(_ragdoll.state == PlayerRagdoll.RagdollState.Ragdoll)
                return;

        if(eventType == EventType.Grab)
        {
            var childCount = transform.childCount;
            for(int i = 0; i < childCount; ++i)
            {
                if (((1 << transform.GetChild(i).gameObject.layer) & targetLayer.value) != 0)
                {
                    var dir = Vector3.zero;
                    if(hitForcePointType == HitForcePointType.CollisionPoint)
                    {
                        dir = GetCollisionPointDirection();
                    }
                    else if(hitForcePointType == HitForcePointType.CenterPosition)
                    {
                        dir = GetTargetDirection();
                    }

                    ExplosionRagdoll(dir);
                }
            }
        }
        else if(eventType == EventType.Collision)
        {
            var playerPos = _ragdoll.transform.position;
            var closest = _myCollider.ClosestPoint(playerPos);
            var dist = Vector3.Distance(playerPos,closest);

            if(dist <= collisionDistance)
            {
                var dir = Vector3.zero;
                if(hitForcePointType == HitForcePointType.CollisionPoint)
                {
                    dir = (_ragdoll.transform.position - closest).normalized;
                }
                else if(hitForcePointType == HitForcePointType.CenterPosition)
                {
                    dir = GetTargetDirection();
                }

                ExplosionRagdoll(dir);
            }
        }
        
    }

    public void OnCollisionEnter(Collision coll)
    {
        if(!collisionEvent)
            return;
        if(_ragdoll.state == PlayerRagdoll.RagdollState.Ragdoll)
            return;

        if (targetLayer == (targetLayer | (1 << coll.gameObject.layer)))
        {
            var dir = Vector3.zero;
            if(hitForcePointType == HitForcePointType.CollisionPoint)
            {
                dir = coll.GetContact(0).normal;//(GameManager.Instance.player.transform.position - coll.GetContact(0).point).normalized;
            }
            else if(hitForcePointType == HitForcePointType.CenterPosition)
            {
                dir = GetTargetDirection();
            }

            ExplosionRagdoll(dir);
        }
    }

    public Vector3 GetCollisionPointDirection()
    {
        var playerPos = _ragdoll.transform.position;
        var closest = _myCollider.ClosestPoint(playerPos);
        return (_ragdoll.transform.position - closest).normalized;
    }

    public Vector3 GetTargetDirection()
    {
        var dir = ((_ragdoll.transform.position + Vector3.up * 3f) - transform.position).normalized; 
        if(deleteY)
        {
            dir.y = 0f;
            dir = dir.normalized;
        }

        return dir;
    }

    public void ExplosionRagdoll(Vector3 dir)
    {
        _player.transform.SetParent(null);
        _player.TakeDamage(hitDamage);

        if(ragdollType == RagdollType.ElectricShock)
            _ragdoll.SetPlayerShock(shockTime);
        _ragdoll.ExplosionRagdoll(hitForce,(dir).normalized);
    }
}
