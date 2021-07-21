using UnityEngine;

public abstract class ObjectBase : MessageReceiver, IProgress
{
    protected Vector3 _position;
    protected Quaternion _rotation;
    protected Vector3 _scale;

    protected int _currentManagerNumber;

    protected override void Awake()
    {
        base.Awake();

        SyncLocalValue();
        Assign();
        //Initialize();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    public virtual void UpdateTransform()
    {
        if(_position != transform.position || _rotation != transform.rotation)
        {
            transform.SetPositionAndRotation(_position,_rotation);
            transform.localScale = _scale;

            SyncLocalValue();
        }
        
    }

    public virtual void SyncLocalValue()
    {
        _position = transform.position;
        _rotation = transform.rotation;
        _scale = transform.localScale;
    }

    public void RegisterRequest(int managerNumber)
    {
        _currentManagerNumber = managerNumber;
        var msg = MessagePack(MessageTitles.system_registerRequest,_currentManagerNumber,null);
        MasterManager.instance.ReceiveMessage(msg);

#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void WithdrawRequest()
    {
        var msg = MessagePack(MessageTitles.system_withdrawRequest,_currentManagerNumber,uniqueNumber);
        MasterManager.instance.ReceiveMessage(msg);

#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public virtual void Assign(){}
    public virtual void Initialize(){}
    public virtual void Progress(float deltaTime){}
    public virtual void AfterProgress(float deltaTime){}
    public virtual void Release()
    {
        WithdrawRequest();

#if UNITY_EDITOR
        Debug_ClearQueue();
#endif

    }

    public override void Dispose()
    {
        base.Dispose();
        Release();
    }

    protected virtual void OnDestroy()
    {
        Debug.Log("Delete");
        Release();
    }
}
