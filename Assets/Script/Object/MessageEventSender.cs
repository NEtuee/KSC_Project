using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;
using MessageSender;

public class MessageEventSender : ObjectBase
{
    [HideInInspector] public MessageTitleEnum message;

    [HideInInspector] public MissionUiPack missionUiPack;
    [HideInInspector] public LevelLineActiveBossNamePack levelLineActiveBossNamePack;
    [HideInInspector] public LevelLineSetAlphabetPack levelLineSetAlphabetPack;
    [HideInInspector] public ActiveInformationUiPack activeInformationUiPack;
    [HideInInspector] public SetTimeInformationUiPack setTimeInformationUiPack;
    [HideInInspector] public MissionUiDisspearPack missionUiDisspearPack;
    [HideInInspector] public DialogPack dialogPack;
    [HideInInspector] public DialogSetNamePack dialogSetNamePack;
    public List<DialogPack> dialogPacks = new List<DialogPack>();

    private System.Action _event;

    public override void Assign()
    {
        base.Assign();

        switch (message)
        {
            case MessageTitleEnum.MissionUi:
                {
                    _event = () =>
                    {
                        string data = missionUiPack.key;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.LevelLineActiveBossName:
                {
                    _event = () =>
                    {
                        string data = levelLineActiveBossNamePack.name;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.LevelLineSetAlphabet:
                {
                    _event = () =>
                    {
                        var data = MessageDataPooling.GetMessageData<LevelLineAlphabetData>();
                        data.value = levelLineSetAlphabetPack.alphabet;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.ActiveInformationUi:
                {
                    _event = () =>
                    {
                        string data = activeInformationUiPack.key;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.SetTimeInformationUi:
                {
                    _event = () =>
                    {
                        var data = MessageDataPooling.GetMessageData<FloatData>();
                        data.value = setTimeInformationUiPack.time;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.DisappearMissionUi:
                {
                    _event = () =>
                    {
                        Invoke("DisableMissionUiMessage", missionUiDisspearPack.time);
                    };
                }
                break;
            case MessageTitleEnum.PlayerRagdoll:
                {
                    _event = () =>
                    {
                        SendMessageEx((ushort)message, GetSavedNumber("PlayerManager"), null);
                    };
                }
                break;
            case MessageTitleEnum.Dialog:
                {
                    _event = () =>
                    {
                        var data = MessageDataPooling.GetMessageData<DroneTextKeyAndDurationData>();
                        data.key = dialogPack.key;
                        data.duration = dialogPack.duration;
                        SendMessageEx((ushort)message, GetSavedNumber("PlayerManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.DialogNameSet:
                {
                    _event = () =>
                    {
                        SendMessageEx((ushort)message, GetSavedNumber("PlayerManager"), dialogSetNamePack.name);
                    };
                }
                break;
            case MessageTitleEnum.DialogLoop:
                {
                    _event = () =>
                    {
                        StartCoroutine(MessageLoop());
                    };
                }
                break;
        }
    }

    public IEnumerator MessageLoop()
    {
        for (int i = 0; i < dialogPacks.Count; i++)
        {
            var data = MessageDataPooling.GetMessageData<DroneTextKeyAndDurationData>();
            data.key = dialogPacks[i].key;
            data.duration = dialogPacks[i].duration;
            SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, GetSavedNumber("PlayerManager"), data);
            yield return new WaitForSeconds(dialogPacks[i].duration);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }

    public void Send()
    {
        _event();
    }
}
