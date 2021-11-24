using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MessageSender;

[CustomEditor(typeof(MessageEventTrigger))]
[CanEditMultipleObjects]
public class MessageEventTriggerEditor : Editor
{
    public LevelLineUI.Alphabet alphabet;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MessageEventTrigger eventTrigger = (MessageEventTrigger)target;

        MessageTitleEnum title = eventTrigger.message;

        eventTrigger.message = (MessageTitleEnum)EditorGUILayout.EnumPopup("Message", title);


        if (eventTrigger.message == MessageTitleEnum.MissionUi)
        {
            eventTrigger.missionUiPack.key = EditorGUILayout.TextField("Key",eventTrigger.missionUiPack.key);
        }
        else if(eventTrigger.message == MessageTitleEnum.LevelLineActiveBossName)
        {
            eventTrigger.levelLineActiveBossNamePack.name = EditorGUILayout.TextField("BossName",eventTrigger.levelLineActiveBossNamePack.name);
        }
        else if (eventTrigger.message == MessageTitleEnum.LevelLineSetAlphabet)
        {
            alphabet = eventTrigger.levelLineSetAlphabetPack.alphabet;
            eventTrigger.levelLineSetAlphabetPack.alphabet = (LevelLineUI.Alphabet)EditorGUILayout.EnumPopup("Alphabet",alphabet);
        }
        else if (eventTrigger.message == MessageTitleEnum.ActiveInformationUi)
        {
            eventTrigger.activeInformationUiPack.key = EditorGUILayout.TextField("Key", eventTrigger.activeInformationUiPack.key);
        }
        else if (eventTrigger.message == MessageTitleEnum.SetTimeInformationUi)
        {
            eventTrigger.setTimeInformationUiPack.time = EditorGUILayout.FloatField("Time", eventTrigger.setTimeInformationUiPack.time);
        }
        else if (eventTrigger.message == MessageTitleEnum.DisappearMissionUi)
        {
            eventTrigger.missionUiDisspearPack.time = EditorGUILayout.FloatField("DisappearTime", eventTrigger.missionUiDisspearPack.time);
        }
        else if (eventTrigger.message == MessageTitleEnum.PlayerRagdoll)
        {

        }
        else if(eventTrigger.message == MessageTitleEnum.Dialog)
        {
            eventTrigger.dialogPack.key = EditorGUILayout.TextField("Key", eventTrigger.dialogPack.key);
            eventTrigger.dialogPack.duration = EditorGUILayout.FloatField("Duration", eventTrigger.dialogPack.duration);
        }
        else if (eventTrigger.message == MessageTitleEnum.DialogNameSet)
        {
            eventTrigger.dialogSetNamePack.name = EditorGUILayout.TextField("Name", eventTrigger.dialogSetNamePack.name);
        }
        else if (eventTrigger.message == MessageTitleEnum.DialogLoop)
        {

        }
    }
}
