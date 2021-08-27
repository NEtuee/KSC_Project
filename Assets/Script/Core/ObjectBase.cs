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
    protected ObjectTransform _objTransform = new ObjectTransform();

    protected int _currentManagerNumber;

    protected override void Awake()
    {
        base.Awake();
        Assign();
        //Initialize();
    }

    protected virtual void Start()
    {
        Initialize();
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
    public virtual void FixedProgress(float deltaTime){}
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
