using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

public class LevelEdit_TimelinePlayer : MonoBehaviour
{
    public PlayableDirector playableDirector;

    public Transform endTransform;

    public bool playerDisable = false;
    public bool droneDisable = false;

    public void Play()
    {
        GameManager.Instance.PAUSE = true;
        if(playerDisable)
        {
            GameManager.Instance.player.gameObject.SetActive(false);
        }
        if(droneDisable)
        {
            ((PlayerCtrl_Ver2)GameManager.Instance.player).GetDrone().gameObject.SetActive(false);
        }

        CinemachineBrain brain = GameManager.Instance.cameraManager.GetCinemachineBrain();
        TimelineAsset timelineAsset = (TimelineAsset) playableDirector.playableAsset;
        TrackAsset track = timelineAsset.GetOutputTrack(1) ;
        Debug.Log(track.name);
        playableDirector.SetGenericBinding (track, brain);
        playableDirector.Play();
    }

    public void End()
    {
        GameManager.Instance.PAUSE = false;
        GameManager.Instance.player.transform.position = endTransform.position;
        if(playerDisable)
        {
            GameManager.Instance.player.gameObject.SetActive(true);
        }
        if(droneDisable)
        {
            ((PlayerCtrl_Ver2)GameManager.Instance.player).GetDrone().gameObject.SetActive(true);
        }
    }
}
