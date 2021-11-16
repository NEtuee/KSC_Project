using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MessageEventTrigger))]
[CanEditMultipleObjects]
public class MessageEventTriggerEditor : Editor
{
    public LevelLineUI.Alphabet alphabet;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MessageEventTrigger eventTrigger = (MessageEventTrigger)target;

        MessageEventTrigger.Message title = eventTrigger.message;

        eventTrigger.message = (MessageEventTrigger.Message)EditorGUILayout.EnumPopup("Message", title);


        if (eventTrigger.message == MessageEventTrigger.Message.MissionUi)
        {
            eventTrigger.missionUiPack.key = EditorGUILayout.TextField("Key",eventTrigger.missionUiPack.key);
        }
        else if(eventTrigger.message == MessageEventTrigger.Message.LevelLineActiveBossName)
        {
            eventTrigger.levelLineActiveBossNamePack.name = EditorGUILayout.TextField("BossName",eventTrigger.levelLineActiveBossNamePack.name);
        }
        else if (eventTrigger.message == MessageEventTrigger.Message.LevelLineSetAlphabet)
        {
            alphabet = eventTrigger.levelLineSetAlphabetPack.alphabet;
            eventTrigger.levelLineSetAlphabetPack.alphabet = (LevelLineUI.Alphabet)EditorGUILayout.EnumPopup("Alphabet",alphabet);
        }
        else if (eventTrigger.message == MessageEventTrigger.Message.ActiveInformationUi)
        {
            eventTrigger.activeInformationUiPack.key = EditorGUILayout.TextField("Key", eventTrigger.activeInformationUiPack.key);
        }
        else if (eventTrigger.message == MessageEventTrigger.Message.SetTimeInformationUi)
        {
            eventTrigger.setTimeInformationUiPack.time = EditorGUILayout.FloatField("Time", eventTrigger.setTimeInformationUiPack.time);
        }
        else if (eventTrigger.message == MessageEventTrigger.Message.DisappearMissionUi)
        {
            eventTrigger.missionUiDisspearPack.time = EditorGUILayout.FloatField("DisappearTime", eventTrigger.missionUiDisspearPack.time);
        }
    }
}
