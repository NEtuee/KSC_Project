using UnityEngine;

public abstract class ObjectBase : MessageReceiver, IProgress
{
    protected Vector3 _position;
    protected Quaternion _rotation;
    protected Vector3 _scale;

    protected override void Awake()
    {
        base.Awake();

        SyncLocalValue();
        Assign();
    }

    public void UpdateTransform()
    {
        if(_position != transform.position || _rotation != transform.rotation)
        {
            transform.SetPositionAndRotation(_position,_rotation);
            transform.localScale = _scale;

            SyncLocalValue();
        }
        
    }

    public void SyncLocalValue()
    {
        _position = transform.position;
        _rotation = transform.rotation;
        _scale = transform.localScale;
    }

    public virtual void Assign(){}
    public virtual void Initialize(){}
    public virtual void Progress(float deltaTime){}
    public virtual void AfterProgress(float deltaTime){}
    public virtual void Release(){}
}
