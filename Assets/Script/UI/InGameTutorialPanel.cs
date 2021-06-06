using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class InGameTutorialPanel : MonoBehaviour
{
    private Canvas _canvas;

    [SerializeField] private TutorialDataCenter dataCenter;
    [SerializeField] private TutorialVideoPlayer videoPlayer;
    [SerializeField] private RawImage tutorialVideoRawImage;
    [SerializeField] private TextMeshProUGUI descritionText;
    [SerializeField] private ButtonGroup buttonGroup;
    public bool notVideo;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void Active(bool active)
    {
        _canvas.enabled = active;
        buttonGroup.Active(active);
        if (active)
        {
            GameManager.Instance.PAUSE = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.optionMenuCtrl.CurrentTutorial = true;
            ((PlayerCtrl_Ver2)GameManager.Instance.player).IsRun = false;

            if(!notVideo)
              videoPlayer.SetTargetImage(tutorialVideoRawImage);

        }
        else
        {
            GameManager.Instance.PAUSE = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (!notVideo)
                videoPlayer.StopVideo();
        }
    }

    public void SetVideoAndText(string key)
    {
        VideoClip clip;
        string descrition;

        dataCenter.GetVideo(key, out clip, out descrition);

        if (clip != null)
        {
            videoPlayer.SetClip(clip, true);
        }

        if (descrition != null)
        {
            descritionText.text = descrition;
        }
    }
}
