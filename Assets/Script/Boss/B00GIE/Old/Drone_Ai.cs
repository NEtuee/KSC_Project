
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Drone_Ai : IKPathFollowBossBase
{
    public Transform bombPoint;
    public BoogieBomb bomb;
    public Collider coll;
    
    public string path;

    public bool drop = false;

    public void Setup()
    {
        GetPathStart(path);
        _pathLoop = false;
        if (_targetTransform != null)
        {
            transform.position = _targetTransform.position;
            transform.rotation = Quaternion.LookRotation(_currentPath.GetDirectionToNextPoint(0, false).normalized);
        }
        
        drop = false;
        bomb.coll.enabled = false;
        bomb.rig.isKinematic = true;
        bomb.transform.position = bombPoint.position;
        bomb.transform.rotation = bombPoint.rotation;
        bomb.gameObject.SetActive(true);
    }

    public void Update()
    {
        Progress(Time.deltaTime);
    }

    public void Progress(float deltaTime)
    {
        FollowPath(deltaTime);
    }
    
    public override void Hit(float damage)
    {
        coll.enabled = false;
        bomb.coll.enabled = true;
        bomb.rig.isKinematic = false;
        bomb.transform.SetParent(null);

        drop = true;
        
        gameObject.SetActive(false);
    }
}
