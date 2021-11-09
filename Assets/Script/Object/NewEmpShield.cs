using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MD;

public class NewEmpShield : Scanable
{
    [SerializeField] private bool awakeOnVisible = false;
    [SerializeField] private int destroyCount = 1;
    private int curCount = 0;
    [SerializeField] private bool isActive = false;
    private bool isVisible = false;

    public bool IsActive => isActive;

    private Material _mat;
    private Collider _coll;
    private Renderer _renderer;

    public UnityEvent whenHit;
    public UnityEvent whenDestroy;
    public UnityEvent whenScanned;

    public delegate void WhenReactive(GameObject scanable);
    private WhenReactive whenReactive;

    public override void Assign()
    {
        base.Assign();

        _renderer = GetComponent<Renderer>();
        _mat = _renderer.material;
        _coll = GetComponent<Collider>();

        if(awakeOnVisible)
        {
            isActive = true;
            VisibleVisual();
        }
        else
        {
            isActive = true;
            isVisible = false;
            _mat.SetFloat("_Fade", 0.0f);
        }

        var drone = GameObject.FindGameObjectWithTag("Drone")?.GetComponent<DroneScaner>();
        if (drone != null)
        {
            whenReactive += drone.AddScanableObjets;
        }

        AddAction(MessageTitles.scan_scanned, (x) => {

            MD.ScanMakerData data = MessageDataPooling.GetMessageData<MD.ScanMakerData>();
            data.collider = collider;

            SendMessageEx(MessageTitles.uimanager_activeScanMaker, GetSavedNumber("UIManager"), data);
            Scanned();
        });

        AddAction(MessageTitles.player_EMPHit, (x) => {
            var damage = MessageDataPooling.CastData<MD.FloatData>(x.data).value;
            Hit();
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));

        SendMessageQuick(MessageTitles.scan_registerScanObject, UniqueNumberBase.GetSavedNumberStatic("Drone"), this);
        SendMessageEx(MessageTitles.set_gunTargetMessageObject, UniqueNumberBase.GetSavedNumberStatic("FollowTargetCtrl"), this.transform);

        SendMessageEx(MessageTitles.uimanager_activeTargetMakerUiAndSetTarget, UniqueNumberBase.GetSavedNumberStatic("UIManager"), this.transform);
    }

    public void Reactive()
    {
        _coll.enabled = true;
        _renderer.enabled = true;
        isActive = true;
        isVisible = false;

        curCount = 0;
    }

    public void Hit()
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if (isActive == false)
            VisibleVisual();

        curCount++;

        if(curCount >= destroyCount)
        {
            Destroy();
        }

        whenHit.Invoke();
    }

    public void VisibleVisual()
    {
        isVisible = true;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float alpha = _mat.GetFloat("_Fade");

        while(alpha < 1.0f)
        {
            alpha = Mathf.MoveTowards(alpha, 1.0f, 1.0f * Time.deltaTime);
            _mat.SetFloat("_Fade",alpha);
            yield return null;
        }
    }

    public void Destroy()
    {
        AttachSoundPlayData soundData2 = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData2.id = 1518; soundData2.localPosition = new Vector3(0, 1, 0); soundData2.parent = transform; soundData2.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData2);
        SoundPlayData soundData3 = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundData3.id = 1501; soundData3.position = transform.position; soundData3.returnValue = false; soundData3.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData3);

        EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);
        _coll.enabled = false;
        _renderer.enabled = false;
        isVisible = false;
        isActive = false;

        _mat.SetFloat("_Fade", 0.0f);

        whenDestroy.Invoke();
    }

    public override void Scanned()
    {
        if (gameObject.activeInHierarchy == false)
            return;

        VisibleVisual();

        ScanMakerData data = MessageDataPooling.GetMessageData<ScanMakerData>();
        data.collider = collider;
        SendMessageEx(MessageTitles.uimanager_activeScanMaker, GetSavedNumber("UIManager"), data);

        whenScanned?.Invoke();
    }
}
