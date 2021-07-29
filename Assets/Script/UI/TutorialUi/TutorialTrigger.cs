using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : UnTransfromObjectBase
{
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
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOver)
            return;

        SendMessageEx(MessageTitles.uimanager_activeInGameTutorial, GetSavedNumber("UIManager"), tutorial);
        
        isOver = true;
        gameObject.SetActive(false);
    }
}
