using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : ManagerBase
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private bool _isPrepared = false;
    [SerializeField] private TutorialVideoClipAndDescriptSet _tutorialVideoClipAndDescriptionSet;

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
            string key = (string)msg.data;
            PlayVideo(key);
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
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("Preparing");
        }
        _isPrepared = true;
        PlayVideo();
    }

    public void PlayVideo()
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
        targetImage.texture = _videoPlayer.texture;
    }

    public void PlayVideo(string key)
    {
        if (_tutorialData.ContainsKey(key) == false)
        {
            Debug.LogWarning("Not Exist Key : " + key);
            return;
        }

        _videoPlayer.clip = _tutorialData[key].videoClip;
    }
}
