using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

[RequireComponent(typeof(PathManagerEx))]
public class StageManagerEx : ManagerBase
{
    public BooleanTrigger stageTriggerAsset;
    public PathManagerEx pathManager;
    
    public Elevator entranceElevator;
    public Elevator exitElevator;
    public Transform loadedPlayerPosition;

    protected override void Awake()
    {
        base.Awake();
        SaveMyNumber("StageManager", true);
        RegisterRequest();
    }

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.scene_afterSceneChange, (msg) =>
        {
            var loadTransform = entranceElevator == null ? loadedPlayerPosition : entranceElevator.transform;
            var rotation = loadTransform.rotation;

            if(entranceElevator != null)
            {
                entranceElevator.Open();
                rotation.eulerAngles += new Vector3(0f,180f,0f);
            }

            PositionRotation data = MessageDataPooling.GetMessageData<PositionRotation>();
            data.position = loadTransform.position;
            data.rotation = rotation;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);

            SendMessageEx(MessageTitles.player_animatiorStateChangeDefault, GetSavedNumber("Player"), null);

            PitchYawPositionData camData = MessageDataPooling.GetMessageData<PitchYawPositionData>();
            camData.position = loadTransform.position;
            camData.pitch = rotation.eulerAngles.x;
            camData.yaw = rotation.eulerAngles.y;
            SendMessageEx(MessageTitles.cameramanager_setYawPitchPosition, GetSavedNumber("CameraManager"), camData);
            SendMessageEx(MessageTitles.cameramanager_initCameraPositionAndRotation, GetSavedNumber("CameraManager"), null);
        });

        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
        {
            var broad = MessagePack(msg.title, boradcastWithoutSenderNumber, msg.data);
            HandleBroadcastMessage(broad);
        });
        AddAction(MessageTitles.boolTrigger_getTriggerAsset,GetStageTriggerAsset);
        AddAction(MessageTitles.boolTrigger_getTrigger,GetStageTrigger);
        AddAction(MessageTitles.boolTrigger_setTrigger,SetStageTrigger);

        AddAction(MessageTitles.stage_getPath,GetPath);

        AddAction(MessageTitles.stage_droneSpecial,(msg)=>{
            var broad = MessagePack(msg.title, boradcastWithoutSenderNumber, msg.data);
            HandleBroadcastMessage(broad);
        });
    }
    
    public override void HandleMessage(Message msg)
    {
        base.HandleMessage(msg);
    }

    public void SetTrigger(string target)
    {
        var trigger = MessageDataPooling.GetMessageData<MD.TriggerData>();
        trigger.name = target;
        trigger.trigger = true;

        SendMessageEx(MessageTitles.boolTrigger_setTrigger, UniqueNumberBase.GetSavedNumberStatic("GlobalTriggerManager"), trigger);
    }


#region StageTrigger
    public void GetStageTriggerAsset(Message msg)
    {
        var receiver = (MessageReceiver)msg.sender;
        var send = MessagePack(MessageTitles.boolTrigger_getTriggerAsset,receiver.uniqueNumber,stageTriggerAsset);
        SendMessageQuick(receiver,send);
    }

    public void GetStageTrigger(Message msg)
    {
        if(stageTriggerAsset == null)
        {
            return;
        }

        var receivedData = MessageDataPooling.CastData<MD.StringData>(msg.data);
        var trigger = stageTriggerAsset.FindTrigger(receivedData.value);

        if(trigger == null)
        {
            Debug.Log("Trigger does not exists");
            return;
        }

        var sendData = MessageDataPooling.GetMessageData<MD.TriggerData>();
        sendData.name = trigger.name;
        sendData.trigger = trigger.trigger;

        var receiver = (MessageReceiver)msg.sender;
        var send = MessagePack(MessageTitles.boolTrigger_getTrigger,receiver.uniqueNumber,sendData);
        SendMessageQuick(receiver,send);
    }

    public void SetStageTrigger(Message msg)
    {
        if(stageTriggerAsset == null)
        {
            return;
        }

        var receivedData = MessageDataPooling.CastData<MD.TriggerData>(msg.data);
        var trigger = stageTriggerAsset.FindTrigger(receivedData.name);

        if(trigger == null)
        {
            Debug.Log("Trigger does not exists");
            return;
        }

        trigger.trigger = receivedData.trigger;
    }
#endregion

#region PathManager

    public void GetPath(Message msg)
    {
        var receiver = (MessageReceiver)msg.sender;
        var data = MessageDataPooling.CastData<MD.StringData>(msg.data);
        var path = GetPath(data.value);

        if(path == null)
        {
            Debug.Log("Path does not Exists : " + data.value);
            return;
        }

        SendMessageQuick(receiver,MessageTitles.stage_getPath,path);
    }

    public PathManagerEx GetPathManager() {return pathManager;}

    public void AddPoint(string path, MovePointEx point) {pathManager.AddPoint(path, point);}
    public void DeletePoint(string path, int pos) {pathManager.DeletePoint(path, pos);}

    public MovePointEx GetPoint(string path, int pos) {return pathManager.GetPoint(path, pos);}
    public List<MovePointEx> GetPointList(string path) {return pathManager.GetList(path);}
    public PathManagerEx.PathClass GetPath(string path) {return pathManager.FindPath(path);}

#endregion

}
