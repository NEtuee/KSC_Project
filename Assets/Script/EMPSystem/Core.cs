using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class Core : Hitable
{
    public GameObject destroyEffect;

    protected override void Awake()
    {
        base.Awake();

        AddAction(MessageTitles.scan_scanned, (x) => {

            MD.ScanMakerData data = MessageDataPooling.GetMessageData<MD.ScanMakerData>();
            data.collider = collider;

            SendMessageEx(MessageTitles.uimanager_activeScanMaker, GetSavedNumber("UIManager"), data);
            Scanned();
        });

        AddAction(MessageTitles.player_NormalHit, (x) => {

            Destroy();
        });

        AddAction(MessageTitles.player_EMPHit, (x) => {
            Destroy();
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageEx(MessageTitles.scan_registerScanObject,UniqueNumberBase.GetSavedNumberStatic("Drone"),this);
    }

    public override void Destroy()
    {
        //GameManager.Instance.effectManager.Active("CannonExplosion", transform.position);
        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"),data);
        collider.enabled = false;
        renderer.enabled = false;
        isOver = true;

        whenDestroy.Invoke();
    }

    public void Reactive()
    {
        collider.enabled = true;
        renderer.enabled = true;
        isOver = false;
    }

    public override void Hit()
    {
    }

    public override void Hit(float damage)
    {
        hp -= damage;

        if (hp <= 0f)
        {
            Destroy();
        }
    }

    public override void Hit(float damage, out bool isDestroy)
    {
        hp -= damage;

        isDestroy = false;
        if(hp <= 0f)
        {
            isDestroy = true;
            Destroy();
        }
    }

    public override void Scanned()
    {
    }
}
