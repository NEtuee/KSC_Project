using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LevelEdit_TimelinePlayer : UnTransfromObjectBase
{
    public static bool CUTSCENEPLAY;

    public PlayableDirector playableDirector;
    public float endSignalTime;

    public string skipTrigger = "";

    public List<GameObject> activeLists = new List<GameObject>();
    public List<GameObject> endActiveLists = new List<GameObject>();
    public Transform endTransform;
    public Transform endCamTransform;
    public Transform birdyEndPosition;

    public bool loadNextLevel = false;
    public bool startFade = true;
    public bool playerDisable = false;
    public bool birdyCanMove = true;
    public bool ragdoll = false;

    private CinemachineBrain _mainCamBrain;
    private CameraManager _camManager;

    private bool _skip = false;


    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setCameraManager,(x)=>{
            _camManager = (CameraManager)x.data;
            _mainCamBrain = _camManager.GetCinemachineBrain();
        });

        AddAction(MessageTitles.boolTrigger_getTrigger, (x) =>
        {
            var sendData = MessageDataPooling.CastData<MD.TriggerData>(x.data);
            if(sendData != null)
            {
                _skip = sendData.trigger;
            }
        });

    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("CutsceneManager"));
        SendMessageQuick(MessageTitles.cameramanager_getCameraManager,GetSavedNumber("CameraManager"),null);

        if(skipTrigger != "")
        {
            var data = MessageDataPooling.GetMessageData<MD.StringData>();
            data.value = skipTrigger;
            SendMessageEx(MessageTitles.boolTrigger_getTrigger, GetSavedNumber("GlobalTriggerManager"), data);
        }
        
    }

    public void Play()
    {
        //GameManager.Instance.PAUSE = true;
        //SendMessageEx(MessageTitles.pl)

        StartTrigger();

        if (startFade)
        {
            var actionData = MessageDataPooling.GetMessageData<MD.ActionData>();
            actionData.value = ()=>{
                if(!_skip)
                {
                    SetActiveObjects(true);

                    var data = MessageDataPooling.GetMessageData<MD.BrainUpdateMethodData>();
                    data.update = CinemachineBrain.UpdateMethod.SmartUpdate;
                    data.blend = CinemachineBrain.BrainUpdateMethod.LateUpdate;
                    SendMessageEx(MessageTitles.cameramanager_setBrainUpdateMethod, GetSavedNumber("CameraManager"), data);

                    TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;
                    TrackAsset track = timelineAsset.GetOutputTrack(1);
                    playableDirector.timeUpdateMode = DirectorUpdateMode.GameTime;
                    playableDirector.SetGenericBinding(track, _mainCamBrain);
                    playableDirector.Play();
                }
                else
                {
                    var fadeInData = MessageDataPooling.GetMessageData<MD.ActionData>();
                    fadeInData.value = () => {
                        EndSet();

                    };

                    SendMessageEx(MessageTitles.uimanager_fadein, GetSavedNumber("UIManager"), fadeInData);
                }
                
                if (endTransform != null)
                {
                    var positionData = MessageDataPooling.GetMessageData<MD.PositionRotation>();
                    positionData.position = endTransform.position;
                    positionData.rotation = endTransform.rotation;
                    SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), positionData);

                    MD.PitchYawPositionData camData = MessageDataPooling.GetMessageData<MD.PitchYawPositionData>();
                    camData.position = endTransform.position;
                    camData.pitch = endTransform.eulerAngles.x;
                    camData.yaw = endTransform.eulerAngles.y;
                    SendMessageEx(MessageTitles.cameramanager_setYawPitchPosition, GetSavedNumber("CameraManager"), camData);
                }

                //if(_skip)
                //{
                //    End();
                //}
            };

            SendMessageEx(MessageTitles.uimanager_fadeinout,GetSavedNumber("UIManager"),actionData);
        }
        else
        {
            if(!_skip)
            {
                SetActiveObjects(true);

                var data = MessageDataPooling.GetMessageData<MD.BrainUpdateMethodData>();
                data.update = CinemachineBrain.UpdateMethod.SmartUpdate;
                data.blend = CinemachineBrain.BrainUpdateMethod.LateUpdate;
                SendMessageEx(MessageTitles.cameramanager_setBrainUpdateMethod, GetSavedNumber("CameraManager"), data);

                TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;
                TrackAsset track = timelineAsset.GetOutputTrack(1);
                playableDirector.timeUpdateMode = DirectorUpdateMode.GameTime;
                playableDirector.SetGenericBinding(track, _mainCamBrain);
                playableDirector.Play();
            }
            else
            {
                var fadeInData = MessageDataPooling.GetMessageData<MD.ActionData>();
                fadeInData.value = () => {
                    EndSet();

                };

                SendMessageEx(MessageTitles.uimanager_fadein, GetSavedNumber("UIManager"), fadeInData);
            }
            
            if (endTransform != null)
            {
                var positionData = MessageDataPooling.GetMessageData<MD.PositionRotation>();
                positionData.position = endTransform.position;
                positionData.rotation = endTransform.rotation;
                SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), positionData);

                MD.PitchYawPositionData camData = MessageDataPooling.GetMessageData<MD.PitchYawPositionData>();
                camData.position = endTransform.position;
                camData.pitch = endTransform.eulerAngles.x;
                camData.yaw = endTransform.eulerAngles.y;
                SendMessageEx(MessageTitles.cameramanager_setYawPitchPosition, GetSavedNumber("CameraManager"), camData);
            }

            //if (_skip)
            //{
            //    End();
            //    //playableDirector.time = endSignalTime-1;
            //}
        }

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
        if(loadNextLevel)
        {
            var actionData = MessageDataPooling.GetMessageData<MD.ActionData>();
            actionData.value = () => {
                SendMessageEx(MessageTitles.cutscene_stop, GetSavedNumber("CutsceneManager"), null);

                playableDirector.Stop();
                CUTSCENEPLAY = false;

                //SendMessageQuick(MessageTitles.cameramanager_setBrainUpdateMethod, GetSavedNumber("CameraManager"), null);
                //SendMessageQuick(MessageTitles.cutscene_stop, GetSavedNumber("CutsceneManager"), null);
                //playableDirector.Stop();
                //SendMessageQuick(MessageTitles.playermanager_hidePlayer, GetSavedNumber("PlayerManager"), true);

                SetBirdyCanMove(birdyCanMove);
                LoadNextLevel();
            };

            SendMessageEx(MessageTitles.uimanager_fadeinout, GetSavedNumber("UIManager"), actionData);
        }
        else
        {
            var actionData = MessageDataPooling.GetMessageData<MD.ActionData>();
            actionData.value = () => {
                EndSet();

            };

            SendMessageEx(MessageTitles.uimanager_fadeinout, GetSavedNumber("UIManager"), actionData);
        }

        //GameManager.Instance.optionMenuCtrl.respawnFadeCtrl.FadeInOut(() => {
        //    //GameManager.Instance.PAUSE = false;
        //    GameManager.Instance.cameraManager.SetUpdateMethod();
        //    GameManager.Instance.player.transform.position = endTransform.position;
        //    EndTrigger();
        //});
    }

    public void EndSet()
    {
        EndTrigger();
        SetActiveObjects(false);
        foreach(var item in endActiveLists)
        {
            item.SetActive(true);
        }

        if(birdyEndPosition != null)
        {
            var data = MessageDataPooling.GetMessageData<MD.PositionRotation>();
            data.position = birdyEndPosition.position;
            data.rotation = birdyEndPosition.rotation;
            SendMessageEx(MessageTitles.playermanager_setDroneTransform, GetSavedNumber("PlayerManager"), data);
        }

        if (ragdoll)
        {
            SendMessageEx(MessageTitles.playermanager_ragdoll, GetSavedNumber("PlayerManager"), null);
        }

        if(endCamTransform != null)
        {
            MD.PitchYawPositionData camData = MessageDataPooling.GetMessageData<MD.PitchYawPositionData>();
            camData.position = endCamTransform.position;
            camData.pitch = endCamTransform.rotation.eulerAngles.x;
            camData.yaw = endCamTransform.rotation.eulerAngles.y;
            SendMessageEx(MessageTitles.cameramanager_setYawPitchPosition, GetSavedNumber("CameraManager"), camData);
        }
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

    public void SetActiveObjects(bool active)
    {
        foreach(var item in activeLists)
        {
            item.SetActive(active);
        }
    }

    public void LoadNextLevel()
    {
        EndTrigger();
        SendMessageEx(MessageTitles.scene_loadNextLevel,GetSavedNumber("SceneManager"),null);
    }

    public void StartTrigger()
    {
        SendMessageEx(MessageTitles.cutscene_play, GetSavedNumber("CutsceneManager"), this);
        SendMessageEx(MessageTitles.playermanager_resetScreenEffects, GetSavedNumber("PlayerManager"), null);

        if (playerDisable)
        {

            SendMessageEx(MessageTitles.playermanager_hidePlayer,GetSavedNumber("PlayerManager"),false);
            var sideLock = MessageDataPooling.GetMessageData<MD.BoolData>();
            sideLock.value = true;
            SendMessageEx(MessageTitles.cameramanager_cameraSideLock, GetSavedNumber("CameraManager"), sideLock);
            //var rotateLock = MessageDataPooling.GetMessageData<MD.BoolData>();
            //rotateLock.value = true;
            //SendMessageEx(MessageTitles.cameramanager_cameraRotateLock, GetSavedNumber("CameraManager"), rotateLock);
            SendMessageEx(MessageTitles.playermanager_DeactivateInput, GetSavedNumber("PlayerManager"), null);

            var canvasEnable = MessageDataPooling.GetMessageData<MD.BoolData>();
            canvasEnable.value = false;
            SendMessageEx(MessageTitles.uimanager_activePlayUi, GetSavedNumber("UIManager"), canvasEnable);
        }
        CUTSCENEPLAY = true;
    }

    public void EndTrigger()
    {
        SendMessageEx(MessageTitles.cameramanager_setBrainUpdateMethod,GetSavedNumber("CameraManager"),null);
        SendMessageEx(MessageTitles.cutscene_stop, GetSavedNumber("CutsceneManager"), null);
                
        playableDirector.Stop();

        if (playerDisable)
        {
            Debug.Log("TLqkf");
            SendMessageEx(MessageTitles.playermanager_hidePlayer,GetSavedNumber("PlayerManager"),true);
            SendMessageEx(MessageTitles.cameramanager_initCameraPositionAndRotation, GetSavedNumber("CameraManager"), null);
            SendMessageEx(MessageTitles.player_animatiorStateChangeDefault, GetSavedNumber("Player"), null);
            var sideLock = MessageDataPooling.GetMessageData<MD.BoolData>();
            sideLock.value = false;
            SendMessageEx(MessageTitles.cameramanager_cameraSideLock, GetSavedNumber("CameraManager"), sideLock);
            //var rotateLock = MessageDataPooling.GetMessageData<MD.BoolData>();
            //rotateLock.value = false;
            //SendMessageEx(MessageTitles.cameramanager_cameraRotateLock, GetSavedNumber("CameraManager"), rotateLock);
            SendMessageEx(MessageTitles.playermanager_ActiveInput, GetSavedNumber("PlayerManager"), null);

            var canvasEnable = MessageDataPooling.GetMessageData<MD.BoolData>();
            canvasEnable.value = true;
            SendMessageEx(MessageTitles.uimanager_activePlayUi, GetSavedNumber("UIManager"), canvasEnable);
        }
        CUTSCENEPLAY = false;

        SetBirdyCanMove(birdyCanMove);
    }

    public void SetBirdyCanMove(bool value)
    {
        var data = MessageDataPooling.GetMessageData<MD.BoolData>();
        data.value = value;
        SendMessageEx(MessageTitles.playermanager_setDroneCanMove, GetSavedNumber("PlayerManager"), data);
    }
}
