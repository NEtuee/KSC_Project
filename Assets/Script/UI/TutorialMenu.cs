using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialMenu : EscMenu
{
    private Canvas _canvas;

    [SerializeField] private TutorialDataCenter dataCenter;
    [SerializeField] private TutorialVideoPlayer videoPlayer;
    
    
    [SerializeField] private RawImage tutorialVideoRawImage;
    
    [SerializeField] private ButtonGroup climbingGroup;
    [SerializeField] private ButtonGroup moveGroup;
    [SerializeField] private ButtonGroup specialGroup;
    [SerializeField] private ButtonGroup empGroup;
    [SerializeField] private ButtonGroup empLauncherGroup;

    [SerializeField] private TextMeshProUGUI descritionText;

    [Header("TopTab")]
    [SerializeField] private TextBaseButtonUi climbingTabButton;
    [SerializeField] private TextBaseButtonUi moveTabButton;
    [SerializeField] private TextBaseButtonUi specialTabButton;
    [SerializeField] private TextBaseButtonUi empControlButton;
    [SerializeField] private TextBaseButtonUi empButton;


    private void Start()
    {
        Active(false);
    }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public override void Active(bool active)
    {
        _canvas.enabled = active;
        if(active)
        {
            videoPlayer.SetTargetImage(tutorialVideoRawImage);
            climbingTabButton.Select(true);
            climbingTabButton.Interactable = true;
            moveTabButton.Interactable = true;
            specialTabButton.Interactable = true;
            empControlButton.Interactable = true;
            empButton.Interactable = true;
        }
        else
        {
            climbingGroup.Active(false);
            moveGroup.Active(false);
            specialGroup.Active(false);
            empGroup.Active(false);
            empLauncherGroup.Active(false);
            climbingTabButton.Interactable = false;
            moveTabButton.Interactable = false;
            specialTabButton.Interactable = false;
            empControlButton.Interactable = false;
            empButton.Interactable = false;

            videoPlayer.StopVideo();
        }
    }

    public void SetVideoAndText(string key)
    {
        VideoClip clip;
        string descrition;

        dataCenter.GetVideo(key, out clip, out descrition);

        if(clip != null)
        {
            videoPlayer.SetClip(clip, true);
        }

        if(descrition != null)
        {
            descritionText.text = descrition;
        }
    }

    public override void Appear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }
}
