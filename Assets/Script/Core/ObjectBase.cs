using UnityEngine;

public abstract class ObjectBase : MessageReceiver, IProgress
{
    [System.Serializable]
    public class ObjectTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
    protected ObjectTransform _objTransform;

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
        if(_objTransform.position != transform.position || _objTransform.rotation != transform.rotation)
        {
            transform.SetPositionAndRotation(_objTransform.position,_objTransform.rotation);
            transform.localScale = _objTransform.scale;

            SyncLocalValue();
        }
        
    }

    public virtual void SyncLocalValue()
    {
        _objTransform.position = transform.position;
        _objTransform.rotation = transform.rotation;
        _objTransform.scale = transform.localScale;
    }

    public void SendMessageQuick(Message msg)
    {
        MasterManager.instance.HandleMessageQuick(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
    }

    public void SendMessageQuick(ushort title, int target, System.Object data)
    {
        var msg = MessagePack(title,target,data);
        MasterManager.instance.HandleMessageQuick(msg);
#if UNITY_EDITOR
        Debug_AddSendedQueue(msg);
#endif
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
        Dispose();
    }
}
