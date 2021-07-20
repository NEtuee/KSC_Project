using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestUiManager : ManagerBase
{
    public GameObject testImage;

    private bool active = false;

    public override void Assign()
    {
        base.Assign();
        AddAction(MessageTitles.testuimanager_activeimage, ActiveImage);

        SaveMyNumber("TestUiManager");
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            Message msg = MessagePool.GetMessage();
            active = !active;
            msg.Set(MessageTitles.testuimanager_activeimage, GetSavedNumber("TestUiManager"), active, this);

            SendMessageEx(msg);
        }
    }

    public void ActiveImage(Message msg)
    {
        bool active = (bool)msg.data;

        testImage.SetActive(active);
    }
}
