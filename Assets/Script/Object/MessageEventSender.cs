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
