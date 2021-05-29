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
    [SerializeField] private TutorialPageSet tutorialPageSet;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage targetImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField]private bool _isPrepared = false;
    
    public VideoClip startClimbingVideo;
    public VideoClip secondVideo;

    private Dictionary<string, VideoClipAndDescripts> _tutorialData =
        new Dictionary<string, VideoClipAndDescripts>();

    private Dictionary<string, List<string>> _pageList = new Dictionary<string, List<string>>();
    private List<string> currentTutorialList;
    private int _currentPageNum = 0;
    private bool _tutorialActive;
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
        
        _tutorialActive = false;
        
        InitTutorialData();

        //StartCoroutine(PrepareVideo());
    }

    private void InitTutorialData()
    {
        for (int i = 0; i < tutorialVideoClipAndDescriptSet.VideoClipAndDescripts.Length; i++)
        {
            _tutorialData.Add(tutorialVideoClipAndDescriptSet.VideoClipAndDescripts[i].key,tutorialVideoClipAndDescriptSet.VideoClipAndDescripts[i]);
        }

        foreach (var page in tutorialPageSet.pages) 
        {
            _pageList.Add(page.pageName,page.tutorialList);
        }
    }

    public void Active(bool active)
    {
        _tutorialActive = active;
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

    public void GetAndPrepareVideo(string key, out VideoClip videoClip, out string description)
    {
        if (_tutorialData.ContainsKey(key) == false)
        {
            videoClip = null;
            description = null;
            return;
        }    
        
        var data = _tutorialData[key];
        videoClip = data.videoClip;
        description = data.description;
        description = description.Replace("\\n", "\n");
    }

    public bool SetPage(string key)
    {
        if (_pageList.ContainsKey(key) == false)
            return false;
        
        currentTutorialList = _pageList[key];
        _currentPageNum = 0;
        SetAndPrepareVideo(currentTutorialList[_currentPageNum]);
        return true;
    }

    public bool ThroughPage()
    {
        if (_tutorialActive == false)
            return false;

        if (currentTutorialList == null)
            return false;

        _currentPageNum++;
        if (_currentPageNum >= currentTutorialList.Count)
            _currentPageNum = 0;
        SetAndPrepareVideo(currentTutorialList[_currentPageNum]);

        return true;
    }
}
