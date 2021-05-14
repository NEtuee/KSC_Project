using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
public class TutorialVideoPlayer : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField] private TutorialVideoClipAndDescriptSet tutorialVideoClipAndDescriptSet;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage targetImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField]private bool _isPrepared = false;
    
    public VideoClip startClimbingVideo;
    public VideoClip secondVideo;

    private Dictionary<string, VideoClipAndDescripts> _tutorialData =
        new Dictionary<string, VideoClipAndDescripts>();
    
    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("Not exist VideoPlayer");
            return;
        }

        if (targetImage == null)
        {
            Debug.LogWarning("Not exist TargetImage");
            return;
        }

        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        
        InitTutorialData();

        //StartCoroutine(PrepareVideo());
    }

    private void InitTutorialData()
    {
        for (int i = 0; i < tutorialVideoClipAndDescriptSet.VideoClipAndDescripts.Length; i++)
        {
            _tutorialData.Add(tutorialVideoClipAndDescriptSet.VideoClipAndDescripts[i].key,tutorialVideoClipAndDescriptSet.VideoClipAndDescripts[i]);
        }
    }

    public void Active(bool active)
    {
        if(active)
        {
            PlayVideo();
            GameManager.Instance.PAUSE = true;
            _canvas.enabled = true;
        }
        else
        {
            StopVideo();
            GameManager.Instance.PAUSE = false;
            _canvas.enabled = false;
        }
    }
    
    private IEnumerator PrepareVideo()
    {
        _isPrepared = false;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("Preparing");
        }

        targetImage.texture = videoPlayer.texture;
        _isPrepared = true;
        PlayVideo();
    }

    public void PlayVideo()
    {
        if(_isPrepared == true && videoPlayer.clip != null)
        {
            videoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (_isPrepared == true)
        {
            videoPlayer.Stop();
        }
    }

    public void SetVideo(int num)
    {
        if (num == 1)
        {
            videoPlayer.clip = startClimbingVideo;
        }
        else
        {
            videoPlayer.clip = secondVideo;
        }
    }

    public void SetAndPrepareVideo(int num)
    {
        if (num == 1)
        {
            videoPlayer.clip = startClimbingVideo;
        }
        else
        {
            videoPlayer.clip = secondVideo;
        }

        StartCoroutine(PrepareVideo());
    }

    public bool SetAndPrepareVideo(string key)
    {
        if (_tutorialData.ContainsKey(key) == false)
            return false;
        
        var data = _tutorialData[key];
        videoPlayer.clip = data.videoClip;
        string description = data.description;
        description = description.Replace("\\n", "\n");
        descriptionText.text = data.description;
        
        StartCoroutine(PrepareVideo());
        return true;
    }
}
