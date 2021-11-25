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
    private FMODUnity.StudioEventEmitter _soundEmiter = null;
    private SoundInfoItem.SoundParameter _paramInfo = null;

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
            if(_soundEmiter != null)
                _soundEmiter.Stop();
            _soundEmiter = null;
            _paramInfo = null;
        };
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        //SoundPlay();
        
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if (_soundEmiter == null || _paramInfo == null)
            return;

        UpdateParam();
    }

    public void SoundPlay()
    {
        if (_soundEmiter != null)
            _soundEmiter.Stop();
        _soundEmiter = null;
        _paramInfo = null;

        var data = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>();
        data.id = sound;
        data.localPosition = Vector3.zero;
        data.parent = this.transform;
        data.returnValue = true;

        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), data);

        if (param != -1)
        {
            var paramData = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            paramData.soundId = sound;
            paramData.paramId = param;
            SendMessageEx(MessageTitles.fmod_getParamInfo, GetSavedNumber("FMODManager"), paramData);
        }
    }

    public void UpdateParam()
    {
        var dist = Vector3.Distance(_playerTransform.position, transform.position);
        var factor = Mathf.Clamp01(dist / maxDistance);

        _soundEmiter.SetParameter(_paramInfo.name, factor);
    }

    public void OnEnable()
    {
        SoundPlay();

        if(_paramInfo != null)
            UpdateParam();
    }
}
