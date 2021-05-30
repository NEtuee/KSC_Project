using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
public class TutorialVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage targetImage;
    [SerializeField]private bool _isPrepared = false;
    
    private void Start()
    {

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

    public void SetClip(VideoClip videoClip, bool setAndPlay = false)
    {
        videoPlayer.clip = videoClip;
        _isPrepared = false;
        if (setAndPlay == false)
            return;

        StartCoroutine(PrepareVideo());
    }

    public void SetTargetImage(RawImage targetImage)
    {
        this.targetImage = targetImage;
    }
}
