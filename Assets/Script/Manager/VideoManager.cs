using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using MD;

public class VideoManager : ManagerBase
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private bool _isPrepared = false;
    [SerializeField] private TutorialVideoClipAndDescriptSet _tutorialVideoClipAndDescriptionSet;
    private RawImage targetImage;

    private Dictionary<string, VideoClipAndDescripts> _tutorialData = new Dictionary<string, VideoClipAndDescripts>();

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("VideoManager");

        AddAction(MessageTitles.videomanager_settargetimage, (msg) => 
        {
            RawImage targetImage = (RawImage)msg.data;
            SetTargetImage(targetImage);
        });

        AddAction(MessageTitles.videomanager_playvideo, (msg) => 
        {
            StringData data = MessageDataPooling.CastData<StringData>(msg.data);
            PlayVideo(data.value);
        });

        AddAction(MessageTitles.videomanager_stopvideo, (msg)=>StopVideo());
    }

    public override void Initialize()
    {
        base.Initialize();

        if(_videoPlayer == null)
        {
            Debug.LogWarning("Not Set VideoPlayer");
        }

        if(_tutorialVideoClipAndDescriptionSet == null)
        {
            Debug.LogWarning("Not Set VideoSet");
        }

        for (int i = 0; i < _tutorialVideoClipAndDescriptionSet.VideoClipAndDescripts.Length; i++)
        {
            _tutorialData.Add(_tutorialVideoClipAndDescriptionSet.VideoClipAndDescripts[i].key, _tutorialVideoClipAndDescriptionSet.VideoClipAndDescripts[i]);
        }
    }

    private IEnumerator PrepareVideo()
    {
        _isPrepared = false;
        _videoPlayer.Prepare();
        while (!_videoPlayer.isPrepared)
        {
            yield return StartCoroutine(WaitForRealSeconds(0.5f));
        }
        targetImage.texture = _videoPlayer.texture;
        _isPrepared = true;
        Play();
    }

    IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
    }

    public void Play()
    {
        if (_isPrepared == true && _videoPlayer.clip != null)
        {
            _videoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (_isPrepared == true)
        {
            _videoPlayer.Stop();
        }
    }

    public void SetTargetImage(RawImage targetImage)
    {
        //targetImage.texture = _videoPlayer.texture;
        this.targetImage = targetImage;
    }

    public void PlayVideo(string key)
    {
        if (_tutorialData.ContainsKey(key) == false)
        {
            Debug.LogWarning("Not Exist Key : " + key);
            return;
        }

        _videoPlayer.clip = _tutorialData[key].videoClip;

        string description = _tutorialData[key].description;
        description = description.Replace("\\n", "\n");

        StringData data = MessageDataPooling.GetMessageData<StringData>();
        data.value = description;
        SendMessageEx(MessageTitles.uimanager_settutorialdescription, GetSavedNumber("UIManager"), data);
        StartCoroutine(PrepareVideo());
    }
}
