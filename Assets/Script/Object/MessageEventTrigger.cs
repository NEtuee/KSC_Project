using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;
using MessageSender;

public class MessageEventTrigger : ObjectBase
{
    [SerializeField] private bool onDisable = true;
    [SerializeField] private LayerMask triggerLayer;
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

    private System.Action _triggerEvent;
    private Collider _collider;

    public override void Assign()
    {
        base.Assign();

        _collider = GetComponent<Collider>();

        switch (message)
        {
            case MessageTitleEnum.MissionUi:
                {
                    _triggerEvent = () =>
                    {
                        string data = missionUiPack.key;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.LevelLineActiveBossName:
                {
                    _triggerEvent = () =>
                    {
                        string data = levelLineActiveBossNamePack.name;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.LevelLineSetAlphabet:
                {
                    _triggerEvent = () =>
                    {
                        var data = MessageDataPooling.GetMessageData<LevelLineAlphabetData>();
                        data.value = levelLineSetAlphabetPack.alphabet;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.ActiveInformationUi:
                {
                    _triggerEvent = () =>
                    {
                        string data = activeInformationUiPack.key;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.SetTimeInformationUi:
                {
                    _triggerEvent = () =>
                    {
                        var data = MessageDataPooling.GetMessageData<FloatData>();
                        data.value = setTimeInformationUiPack.time;
                        SendMessageEx((ushort)message, GetSavedNumber("UIManager"), data);
                    };
                }
                break;
            case MessageTitleEnum.DisappearMissionUi:
                {
                    _triggerEvent = () =>
                    {
                        Invoke("DisableMissionUiMessage", missionUiDisspearPack.time);
                    };
                }
                break;
            case MessageTitleEnum.Dialog:
                {
                    _triggerEvent = () =>
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
                    _triggerEvent = () =>
                    {
                        SendMessageEx((ushort)message, GetSavedNumber("PlayerManager"), dialogSetNamePack.name);
                    };
                }
                break;
            case MessageTitleEnum.DialogLoop:
                {
                    _triggerEvent = () =>
                    {
                        StartCoroutine(MessageLoop());
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


    public void DisableMissionUiMessage()
    {
        SendMessageEx(MessageTitles.uimanager_DisappearMissionUi, GetSavedNumber("UIManager"), null);
    }

    public IEnumerator MessageLoop()
    {
        for(int i = 0; i < dialogPacks.Count; i++)
        {
            var data = MessageDataPooling.GetMessageData<DroneTextKeyAndDurationData>();
            data.key = dialogPacks[i].key;
            data.duration = dialogPacks[i].duration;
            SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, GetSavedNumber("PlayerManager"), data);
            yield return new WaitForSeconds(dialogPacks[i].duration);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if((triggerLayer.value & (1<<other.gameObject.layer))>0)
        {
            _triggerEvent();
            if (onDisable == true)
                _collider.enabled = false;
        }
    }
}


namespace MessageSender
{
    [System.Serializable]
    public class MissionUiPack
    {
        public string key;
    }

    [System.Serializable]
    public class LevelLineActiveBossNamePack
    {
        public string name;
    }

    [System.Serializable]
    public class LevelLineSetAlphabetPack
    {
        public LevelLineUI.Alphabet alphabet;
    }

    [System.Serializable]
    public class ActiveInformationUiPack
    {
        public string key;
    }

    [System.Serializable]
    public class SetTimeInformationUiPack
    {
        public float time;
    }

    [System.Serializable]
    public class MissionUiDisspearPack
    {
        public float time;
    }

    [System.Serializable]
    public class DialogPack
    {
        public string key;
        public float duration;
    }

    [System.Serializable]
    public class DialogSetNamePack
    {
        public string name;
    }


    public enum MessageTitleEnum
    {
        MissionUi = MessageTitles.uimanager_AppearMissionUiAndSetKey,
        LevelLineActiveBossName = MessageTitles.uimanager_ActiveLeveLineUIAndSetBossName,
        LevelLineSetAlphabet = MessageTitles.uimanager_SetLevelLineAlphabet,
        ActiveInformationUi = MessageTitles.uimanager_AppearInformationUi,
        SetTimeInformationUi = MessageTitles.uimanager_SetShowTimeInformationUi,
        DisappearMissionUi = MessageTitles.uimanager_DisappearMissionUi,
        PlayerRagdoll = MessageTitles.playermanager_ragdoll,
        Dialog = MessageTitles.playermanager_droneTextAndDurationByKey,
        DialogNameSet = MessageTitles.playermanager_SetDialogName,
        DialogLoop
    }
}