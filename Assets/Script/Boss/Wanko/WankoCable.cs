using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WankoCable : ObjectBase
{
    public UnityEvent whenCableDeleted;
    public float deleteTime = 5f;

    public GameObject lightning;
    public Collider collider;
    public Rigidbody plug;
    public bool canDelete = false;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.player_EMPHit,(x)=>{
            if(canDelete)
                DeleteCable();
        });

        AddAction(MessageTitles.scan_scanned,(x)=>{

            MD.ScanMakerData data = MessageDataPooling.GetMessageData<MD.ScanMakerData>();
            data.collider = collider;
            
            SendMessageEx(MessageTitles.uimanager_activeScanMaker,GetSavedNumber("UIManager"),data);
        });

        _timeCounter.InitTimer("Delete",deleteTime,deleteTime);
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageEx(MessageTitles.scan_registerScanObject,UniqueNumberBase.GetSavedNumberStatic("Drone"),this);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Delete",out var limit,deltaTime);
        canDelete = !limit;
        lightning.SetActive(canDelete);

    }

    public void DeleteFlag()
    {
        _timeCounter.InitTimer("Delete",0f,deleteTime);
    }

    public void DeleteCable()
    {
        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageQuick(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"),data);

        plug.isKinematic = false;
        plug.transform.SetParent(null);
        plug.AddForce(Vector3.up * 50000f);

        whenCableDeleted?.Invoke();

        Destroy(this.gameObject);
    }
}
