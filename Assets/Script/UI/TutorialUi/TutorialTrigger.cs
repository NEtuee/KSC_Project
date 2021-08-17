using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : UnTransfromObjectBase
{
    [TextArea]
    public string droneText = "";
    public bool isOver = false;
    public bool IsOver { get => IsOver; set => isOver = value; }

    public InGameTutorialCtrl.InGameTutorialType tutorial;
    //public TutorialType tutorialType;
    //private OptionMenuCtrl _uiManager;

    protected override void Start()
    {
        base.Start();

        if (TryGetComponent<Collider>(out Collider collider) == false)
        {
            Debug.LogWarning("Not Exist Collider");
            return;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOver)
            return;

        MD.InGameTutorialTypeData data = MessageDataPooling.GetMessageData<MD.InGameTutorialTypeData>();
        data.type = tutorial;
        SendMessageEx(MessageTitles.uimanager_activeInGameTutorial, GetSavedNumber("UIManager"), data);
        
        isOver = true;
        gameObject.SetActive(false);

        if(droneText != "")
        {
            var stringData = MessageDataPooling.GetMessageData<MD.StringData>();
            stringData.value = droneText;
            SendMessageEx(MessageTitles.playermanager_droneText,GetSavedNumber("PlayerManager"),stringData);
        }
    }
}
