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

        Debug.Log("assign");

        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
        {
            PositionRotation data = MessageDataPooling.GetMessageData<PositionRotation>();
            data.position = loadedPlayerPosition.position;
            data.rotation = loadedPlayerPosition.rotation;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);

            PitchYawPositionData camData = MessageDataPooling.GetMessageData<PitchYawPositionData>();
            camData.yaw = loadedPlayerPosition.rotation.eulerAngles.y;
            camData.pitch = loadedPlayerPosition.rotation.eulerAngles.x;
            camData.position = loadedPlayerPosition.position;
            SendMessageEx(MessageTitles.cameramanager_setYawPitchPosition, GetSavedNumber("CameraManager"), camData);

            Vector3Data position = MessageDataPooling.GetMessageData<Vector3Data>();
            position.value = loadedPlayerPosition.position;
            SendMessageEx(MessageTitles.cameramanager_setBrainCameraPosition, GetSavedNumber("CameraManager"), position);
            //Debug.Log("send");
        });

        AddAction(MessageTitles.boolTrigger_getTriggerAsset,GetStageTriggerAsset);
        AddAction(MessageTitles.boolTrigger_getTrigger,GetStageTrigger);
        AddAction(MessageTitles.boolTrigger_setTrigger,SetStageTrigger);

        AddAction(MessageTitles.stage_getPath,GetPath);

        AddAction(MessageTitles.scene_afterSceneChange,(x)=>{
            //Debug.Log("GOGOGOGOGOGOGOGOGO");
        });
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
