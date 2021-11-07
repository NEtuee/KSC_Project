using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_FlySpiderState_Rush : B1_FlySpiderStateBase
{
    public override string stateIdentifier => "Rush";

    public LayerMask collisionLayer;

    public float rushSpeed = 5f;
    public float detectRadius = 1f;
    public float rotateFactor = 10f;

    private float _lifeTime;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        target.footPointRotator.enabled = false;
        _lifeTime = 5f;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        target.transform.rotation = Quaternion.Lerp(transform.rotation, 
                    (Quaternion.FromToRotation(transform.forward, target.direction) * transform.rotation),deltaTime * rotateFactor);

        var coll = Physics.OverlapSphere(target.transform.position,detectRadius,collisionLayer);
        if(coll != null && coll.Length != 0)
        {
            target.Explosion();
        }
        else
        {
            _lifeTime -= deltaTime;
            if(_lifeTime <= 0f)
            {
                target.Explosion();
            }
        }

        target.transform.position += target.direction * rushSpeed * deltaTime;
    }
}
