using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LevelEdit_TimelinePlayer : UnTransfromObjectBase
{
    public PlayableDirector playableDirector;

    public Transform endTransform;

    public bool playerDisable = false;

    private CinemachineBrain _mainCamBrain;
    private CameraManager _camManager;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setCameraManager,(x)=>{
            _camManager = (CameraManager)x.data;
            _mainCamBrain = _camManager.GetCinemachineBrain();
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.cameramanager_getCameraManager,GetSavedNumber("CameraManager"),null);
    }

    public void Play()
    {
        //GameManager.Instance.PAUSE = true;
        //SendMessageEx(MessageTitles.pl)

        StartTrigger();

        var actionData = MessageDataPooling.GetMessageData<MD.ActionData>();
        actionData.value = ()=>{
            var data = MessageDataPooling.GetMessageData<MD.BrainUpdateMethodData>();
            data.update = CinemachineBrain.UpdateMethod.SmartUpdate;
            data.blend = CinemachineBrain.BrainUpdateMethod.LateUpdate;
            SendMessageEx(MessageTitles.cameramanager_setBrainUpdateMethod,GetSavedNumber("CameraManager"),data);

            TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;
            TrackAsset track = timelineAsset.GetOutputTrack(1) ;
            playableDirector.timeUpdateMode = DirectorUpdateMode.GameTime;
            playableDirector.SetGenericBinding (track, _mainCamBrain);
            playableDirector.Play();
        };

        SendMessageEx(MessageTitles.uimanager_fadeinout,GetSavedNumber("UIManager"),actionData);

        //GameManager.Instance.optionMenuCtrl.respawnFadeCtrl.FadeInOut(() => {
        //    GameManager.Instance.cameraManager.SetUpdateMethod(CinemachineBrain.UpdateMethod.SmartUpdate,CinemachineBrain.BrainUpdateMethod.LateUpdate);

        //    CinemachineBrain brain = GameManager.Instance.cameraManager.GetCinemachineBrain();
        //    TimelineAsset timelineAsset = (TimelineAsset) playableDirector.playableAsset;
        //    TrackAsset track = timelineAsset.GetOutputTrack(1) ;
        //    playableDirector.SetGenericBinding (track, brain);
        //    playableDirector.Play();
        //});

    }

    public void End()
    {
        var actionData = MessageDataPooling.GetMessageData<MD.ActionData>();
        actionData.value = ()=>{
            EndTrigger();

            if(endTransform != null)
            {
                var data = MessageDataPooling.GetMessageData<MD.PositionRotation>();
                data.position = endTransform.position;
                data.rotation = endTransform.rotation;
                SendMessageEx(MessageTitles.playermanager_setPlayerTransform,GetSavedNumber("PlayerManager"),data);
            }
            
        };

        SendMessageEx(MessageTitles.uimanager_fadeinout,GetSavedNumber("UIManager"),actionData);

        //GameManager.Instance.optionMenuCtrl.respawnFadeCtrl.FadeInOut(() => {
        //    //GameManager.Instance.PAUSE = false;
        //    GameManager.Instance.cameraManager.SetUpdateMethod();
        //    GameManager.Instance.player.transform.position = endTransform.position;
        //    EndTrigger();
        //});
    }

    public void LoadSceneFromManager(string target)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(Camera.main.gameObject, activeScene);
        SceneManager.MoveGameObjectToScene(GameManager.Instance.followTarget.gameObject, activeScene);
        SceneManager.MoveGameObjectToScene(GameManager.Instance.player.gameObject, activeScene);
        SceneManager.MoveGameObjectToScene(((PlayerCtrl_Ver2)GameManager.Instance.player).GetDrone().gameObject, activeScene);

        SceneManager.LoadScene(target,LoadSceneMode.Single);
    }

    public void LoadNextLevel()
    {
        EndTrigger();
        SendMessageEx(MessageTitles.scene_loadNextLevel,GetSavedNumber("SceneManager"),null);
    }

    public void StartTrigger()
    {
        if(playerDisable)
        {
            SendMessageEx(MessageTitles.playermanager_hidePlayer,GetSavedNumber("PlayerManager"),false);
        }
    }

    public void EndTrigger()
    {
        SendMessageEx(MessageTitles.cameramanager_setBrainUpdateMethod,GetSavedNumber("CameraManager"),null);
        if(playerDisable)
        {
            SendMessageEx(MessageTitles.playermanager_hidePlayer,GetSavedNumber("PlayerManager"),true);
        }
    }
}
