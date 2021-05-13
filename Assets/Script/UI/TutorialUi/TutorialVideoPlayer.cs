using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage targetImage;

    private bool _isPrepared = false;
    
    public VideoClip startClimbingVideo;

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

        StartCoroutine(PrepareVideo());
    }
    
    protected IEnumerator PrepareVideo()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        targetImage.texture = videoPlayer.texture;
    }
}
