using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBossBase : Hitable
{
    public float rotationSpeed = 180f;
    public float groundCheckRadius = 5f;
    public float frontMoveSpeed = 5f;

    public LayerMask groundLayer;
    public LayerMask targetLayer;
    
    public Transform groundRayPoint;

    public List<IKLegMovement> legs = new List<IKLegMovement>();

    protected TimeCounterEx _timeCounter = new TimeCounterEx();
    protected PlayerUnit _player;
    protected Transform _target;

    protected Vector3 _centerPosition;
    protected float _targetDistance;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            _target = _player.transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);
        _centerPosition = transform.position;
    }

    public override void Hit()
    {
        
    }

    public override void Hit(float damage)
    {
    }

    public override void Hit(float damage, out bool isDestroy)
    {
        isDestroy = false;
    }

    public override void Destroy()
    {

    }

    public override void Scanned()
    {
    }

    protected void SetIKActive(bool value)
    {
        foreach(var ik in legs)
        {
            ik.SetIKActive(value);
        }
    }

    public void ActiveEffect(string key, Vector3 position, Quaternion rotation, Transform parent)
    {
        var hitData = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        hitData.key = key;
        hitData.position = position;
        hitData.rotation = rotation;
        hitData.parent = parent;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), hitData);
    }

    public void SoundPlay(int code, Transform parent, Vector3 position)
    {
        if(parent != null)
        {
            var data = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>();

            data.id = code;
            data.localPosition = (Vector3)position;
            data.parent = parent;
            data.returnValue = true;

            SendMessageEx(MessageTitles.fmod_attachPlay,UniqueNumberBase.GetSavedNumberStatic("FMODManager"),data);
        }
        else
        {
            var data = MessageDataPooling.GetMessageData<MD.SoundPlayData>();

            data.id = code;
            data.position = (Vector3)position;
            data.dontStop = false;
            data.returnValue = true;

            SendMessageEx(MessageTitles.fmod_play,UniqueNumberBase.GetSavedNumberStatic("FMODManager"),data);
        }
    }
    
    
    protected void SetLegHitGroundSound(int code)
    {
        foreach (var leg in legs)
        {
            leg.legHitToGround += pos =>
            {
                SoundPlay(code,null,pos);
                //_soundManager.Play(code, pos);
            };
        }
    }
    

    public void TargetFrontMove(float deltaTime)
    {
        var dir = (_target.position - transform.position);
        dir = Vector3.ProjectOnPlane(dir,Vector3.up).normalized;
        if(_targetDistance >= 2f)
        {
            Move(dir, frontMoveSpeed, deltaTime);
        }
    }

    public bool CenterMove(float deltaTime)
    {
        var dir = (_centerPosition - transform.position).normalized;
        var centerDist = Vector3.Distance(_centerPosition,transform.position);
        if(centerDist >= 1f)
        {
            Move(dir, frontMoveSpeed, deltaTime);
            return false;
        }

        return true;
    }

    public void SetLegMovementSpeed(float speed)
    {
        foreach(var leg in legs)
        {
            leg.legSpeed = speed;
        }
    }

    public virtual bool MoveForward(float speed, float deltaTime)
    {
        transform.position += transform.forward * (speed * deltaTime);

        return true;
    }

    public virtual bool Move(Vector3 direction, float speed, float deltaTime, float legMovementSpeed = 4f)
    {
        if(!ThereIsGround((direction * (groundCheckRadius * 0.5f)) * MathEx.normalize(speed),10f))
        {
            return false;
        }

        SetLegMovementSpeed(legMovementSpeed + (MathEx.clampOverZero(MathEx.abs(speed) - legMovementSpeed) * 0.4f));

        transform.position += direction * (speed * deltaTime);

        return true;
    }

    public void Turn(bool isLeft, Transform target, float deltaTime)
    {
        Turn(target, rotationSpeed * deltaTime * (isLeft ? 1f : -1f));
    }

    public void Turn(bool isLeft, Transform target, float deltaTime, Vector3 axis)
    {
        Turn(target, rotationSpeed * deltaTime * (isLeft ? 1f : -1f),axis);
    }

    public void Turn(Transform target, float factor, Vector3 axis)
    {
        target.RotateAround(target.position,axis,factor);
    }

    public void Turn(Transform target, float factor)
    {
        target.RotateAround(target.position,target.up,factor);
    }

    public bool GroundCheck(out RaycastHit hit,float dist)
    {
        Ray ray = new Ray();
        ray.origin = groundRayPoint.position;
        ray.direction = -transform.up;

        return Physics.Raycast(ray,out hit, dist, groundLayer);
    }

    public bool ThereIsGround(Vector3 position, float dist)
    {
        Ray ray = new Ray();
        ray.origin = groundRayPoint.position + position;
        ray.direction = -transform.up;

        return Physics.Raycast(ray,dist,groundLayer);
    }
}
