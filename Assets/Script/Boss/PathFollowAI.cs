using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowAI : IKPathFollowBossBase
{
    public enum MoveType
    {
        Land,
        Sky
    }
    public MoveType moveType = MoveType.Land;
    public string path;
    public bool progress;
    public bool loop;
    public bool canPause = false;

    public void Start()
    {
        GetPathStart(path);
        _pathLoop = loop;
    }

    void Update()
    {
        if (GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Update)
            return;

        if(canPause && GameManager.Instance.PAUSE)
            return;
        
        Progress(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Fixed)
            return;

        if(canPause && GameManager.Instance.PAUSE)
            return;
        
        Progress(Time.fixedDeltaTime);
    }

    public void SetProgress(bool value)
    {
        progress = value;
    }

    public void Progress(float deltaTime)
    {
        if(!progress)
            return;
        
        FollowPath(deltaTime);
    }

    public override bool Move(Vector3 direction, float speed, float deltaTime, float legMovementSpeed = 4f)
    {
        _accelSpeed = Mathf.Lerp(_accelSpeed,speed,0.2f);
        var angle = Vector3.SignedAngle(transform.forward,_targetDirection,transform.up);
        if(moveType == MoveType.Land)
        {
            if(MathEx.abs(angle) > _turnAccuracy)
            {
                if(angle > 0)
                    Turn(true,this.transform,deltaTime);
                else
                    Turn(false,this.transform,deltaTime);
            }
        }
        else if(moveType == MoveType.Sky)
        {
            var target = _currentPath.GetPoint(_targetPoint);
            var dir = (target.GetPoint() - transform.position).normalized;
            var look = Quaternion.LookRotation(dir);
            
            var rot = Quaternion.Lerp(transform.rotation, look, deltaTime * 2f).eulerAngles;
            var euler = transform.eulerAngles;
            float currVelocity = 0f;
            euler.x = rot.x;
            euler.y = rot.y;
            euler.z = Mathf.SmoothDampAngle(euler.z,target.transform.eulerAngles.z,ref currVelocity, 0.5f);
            transform.eulerAngles = euler;
        }
        
        


        transform.position += direction * (_accelSpeed * deltaTime);
        

        return true;
    }
}
