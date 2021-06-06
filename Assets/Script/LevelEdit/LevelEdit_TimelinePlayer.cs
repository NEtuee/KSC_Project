using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LevelEdit_TimelinePlayer : MonoBehaviour
{
    public PlayableDirector playableDirector;

    public Transform endTransform;

    public bool playerDisable = false;
    public bool droneDisable = false;

    public void Play()
    {
        //GameManager.Instance.PAUSE = true;

        StartTrigger();

        GameManager.Instance.optionMenuCtrl.respawnFadeCtrl.FadeInOut(() => {
            GameManager.Instance.cameraManager.SetUpdateMethod(CinemachineBrain.UpdateMethod.SmartUpdate,CinemachineBrain.BrainUpdateMethod.LateUpdate);

            CinemachineBrain brain = GameManager.Instance.cameraManager.GetCinemachineBrain();
            TimelineAsset timelineAsset = (TimelineAsset) playableDirector.playableAsset;
            TrackAsset track = timelineAsset.GetOutputTrack(1) ;
            playableDirector.SetGenericBinding (track, brain);
            playableDirector.Play();
        });

    }

    public void End()
    {
        GameManager.Instance.optionMenuCtrl.respawnFadeCtrl.FadeInOut(() => {
            //GameManager.Instance.PAUSE = false;
            GameManager.Instance.cameraManager.SetUpdateMethod();
            GameManager.Instance.player.transform.position = endTransform.position;
            EndTrigger();
        });
    }

    public void LoadSceneFromManager(string target)
    {
        SceneManager.LoadScene(target,LoadSceneMode.Single);
    }

    public void LoadNextLevel()
    {
        EndTrigger();
        GameManager.Instance.asynSceneManager.LoadNextlevelFrom();
    }

    public void StartTrigger()
    {
        if(playerDisable)
        {
            GameManager.Instance.player.gameObject.SetActive(false);
        }
        if(droneDisable)
        {
            ((PlayerCtrl_Ver2)GameManager.Instance.player).GetDrone().gameObject.SetActive(false);
        }
    }

    public void EndTrigger()
    {
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
