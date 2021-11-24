using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Debug = UnityEngine.Debug;

public class AlertSoundPlayer : ObjectBase
{
    public int sound;
    public int param = -1;

    public float maxDistance = 30f;

    private Transform _playerTransform;
    private FMODUnity.StudioEventEmitter _soundEmiter;
    private SoundInfoItem.SoundParameter _paramInfo;

    public override void Assign()
    {
        base.Assign();
        AddAction(MessageTitles.set_setplayer, (x) => {
            _playerTransform = ((PlayerUnit)x.data).transform;
        });
        AddAction(MessageTitles.fmod_soundEmitter, (x) =>
        {
            _soundEmiter = (FMODUnity.StudioEventEmitter)x.data;
        });
        AddAction(MessageTitles.fmod_getParamInfo, (x) =>
        {
            _paramInfo = (SoundInfoItem.SoundParameter)x.data;
        });

        whenDeactive += () =>
        {
            _soundEmiter.Stop();
        };
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        var data = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>();
        data.id = sound;
        data.localPosition = Vector3.zero;
        data.parent = this.transform;
        data.returnValue = true;

        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), data);

        if(param != -1)
        {
            var paramData = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            paramData.soundId = sound;
            paramData.paramId = param;
            SendMessageEx(MessageTitles.fmod_getParamInfo, GetSavedNumber("FMODManager"), paramData);
        }
        
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if (_soundEmiter == null || param == -1)
            return;

        UpdateParam();
    }

    public void UpdateParam()
    {
        var dist = Vector3.Distance(_playerTransform.position, transform.position);
        var factor = Mathf.Clamp01(dist / maxDistance);

        _soundEmiter.SetParameter(_paramInfo.name, factor);
    }

    public void OnEnable()
    {
        if (_soundEmiter == null)
            return;

        _soundEmiter.Play();
        UpdateParam();
    }
}
