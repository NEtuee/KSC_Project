using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestUiManager : ManagerBase
{
    public GameObject testImage;

    private bool active = false;
    private int getData;

    public override void Assign()
    {
        base.Assign();

        MessageDataPooling.RegisterMessageData<TestData>();

        AddAction(MessageTitles.testuimanager_activeimage, ActiveImage);

        AddAction(MessageTitles.testuimanager_messageTest, (msg) =>
         {
             TestData data = MessageDataPooling.CastData<TestData>(msg.data);
             getData = data.num;
         });

        SaveMyNumber("TestUiManager");
    }

    public override void Initialize()
    {
        //base.Initialize();
        //TestData data = MessageDataPooling.GetMessageData<TestData>();
        //data.num = 100;
        //SendMessageEx(MessageTitles.testuimanager_messageTest, GetSavedNumber("TestUiManager"), data);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        //if (Keyboard.current.iKey.wasPressedThisFrame)
        //{
        //    Message msg = MessagePool.GetMessage();
        //    active = !active;
        //    msg.Set(MessageTitles.testuimanager_activeimage, GetSavedNumber("TestUiManager"), active, this);

        //    SendMessageEx(msg);
        //}

        //if (Keyboard.current.oKey.wasPressedThisFrame)
        //{
        //    TestData data = MessageDataPooling.GetMessageData<TestData>();
        //    data.num = 100;
        //    SendMessageEx(MessageTitles.testuimanager_messageTest, GetSavedNumber("TestUiManager"), data);
        //}

        TestData data = MessageDataPooling.GetMessageData<TestData>();
        data.num = 100;
        SendMessageEx(MessageTitles.testuimanager_messageTest, GetSavedNumber("TestUiManager"), data);
    }

    public void ActiveImage(Message msg)
    {
        bool active = (bool)msg.data;

        testImage.SetActive(active);
    }
}

public class TestData : MessageData
{
    public int num;
}
