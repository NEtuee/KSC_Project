using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_RagdollCollider : MonoBehaviour
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
        CenterPosition
    }

    public EventType eventType;
    public RagdollType ragdollType;
    public HitForcePointType hitForcePointType;
    public LayerMask targetLayer;

    public float hitDamage = 5f;
    public float shockTime = 1f;
    public float hitForce = 10f;
    public float collisionDistance = 1f;

    private Collider _myCollider;
    private PlayerRagdoll _ragdoll;

    public void Start()
    {
        _ragdoll = GameManager.Instance.player.GetComponent<PlayerRagdoll>();
        _myCollider = GetComponent<Collider>();
    }


    public void Update()
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
                    Debug.Log(dir);
                }
                else if(hitForcePointType == HitForcePointType.CenterPosition)
                {
                    dir = GetTargetDirection();
                }

                ExplosionRagdoll(dir);
            }
        }
        
    }

    // public void OnCollisionEnter(Collision coll)
    // {
    //     if(eventType != EventType.Collision)
    //         return;
    //     if(_ragdoll.state == PlayerRagdoll.RagdollState.Ragdoll)
    //         return;

    //     if (((1 << coll.gameObject.layer) & targetLayer.value) != 0)
    //     {
    //         var dir = Vector3.zero;
    //         if(hitForcePointType == HitForcePointType.CollisionPoint)
    //         {
    //             dir = -coll.GetContact(0).normal;//(GameManager.Instance.player.transform.position - coll.GetContact(0).point).normalized;
    //         }
    //         else if(hitForcePointType == HitForcePointType.CenterPosition)
    //         {
    //             dir = GetTargetDirection();
    //         }

    //         ExplosionRagdoll(dir);
    //     }
    // }

    public Vector3 GetCollisionPointDirection()
    {
        var playerPos = _ragdoll.transform.position;
        var closest = _myCollider.ClosestPoint(playerPos);
        return (_ragdoll.transform.position - closest).normalized;
    }

    public Vector3 GetTargetDirection()
    {
        return (GameManager.Instance.player.transform.position - transform.position).normalized;
    }

    public void ExplosionRagdoll(Vector3 dir)
    {
        GameManager.Instance.player.transform.SetParent(null);

        GameManager.Instance.player.TakeDamage(hitDamage);

        if(ragdollType == RagdollType.ElectricShock)
            _ragdoll.SetPlayerShock(shockTime);
        _ragdoll.ExplosionRagdoll(hitForce,(dir).normalized);
    }
}
