using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class WeakPoint : Hitable
{
    public bool isActive = false;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.player_NormalHit, (msg) =>
         {
             Destroy();
         });

        AddAction(MessageTitles.scan_scanned, (x) => {
            if (isOver)
                return;

            MD.ScanMakerData data = MessageDataPooling.GetMessageData<MD.ScanMakerData>();
            data.collider = collider;

            SendMessageEx(MessageTitles.uimanager_activeScanMaker, GetSavedNumber("UIManager"), data);
            Scanned();
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));

        SendMessageQuick(MessageTitles.scan_registerScanObject, UniqueNumberBase.GetSavedNumberStatic("Drone"), this);
        SendMessageEx(MessageTitles.set_gunTargetMessageObject, UniqueNumberBase.GetSavedNumberStatic("FollowTargetCtrl"), this.transform);
    }

    public override void Destroy()
    {
        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);
        collider.enabled = false;
        renderer.enabled = false;
        isOver = true;

        whenDestroy.Invoke();
    }

    public override void Hit()
    {

    }

    public override void Hit(float damage)
    {
    }

    public override void Hit(float damage, out bool isDestroy)
    {
        throw new System.NotImplementedException();
    }

    public override void Scanned()
    {
        ScanMakerData data = MessageDataPooling.GetMessageData<ScanMakerData>();
        data.collider = collider;
        SendMessageEx(MessageTitles.uimanager_activeScanMaker, GetSavedNumber("UIManager"), data);

        whenScanned?.Invoke();
    }

    public void Reactive()
    {
        //whenReactive(this.gameObject);

        if (collider != null)
            collider.enabled = true;

        if (renderer != null)
        {
            renderer.enabled = true;
        }
        isOver = false;
        isActive = false;
    }
}
