using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneFade : ObjectBase
{
    public string targetScene;

    public override void Assign()
    {
        base.Assign();
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public void JustFade()
    {
        var action = MessageDataPooling.GetMessageData<MD.ActionData>();
        action.value = () =>{};

        SendMessageEx(MessageTitles.uimanager_fadeout, GetSavedNumber("UIManager"), action);
    }

    public void JustSceneLoad()
    {
        MD.StringData data = MessageDataPooling.GetMessageData<MD.StringData>();
        data.value = "OutScene";
        SendMessageEx(MessageTitles.scene_loadSceneNotAsync, GetSavedNumber("SceneManager"), data);
        //SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    public void FadeStart()
    {
        var action = MessageDataPooling.GetMessageData<MD.ActionData>();
        action.value = () =>
        {
            SceneManager.LoadScene(targetScene, LoadSceneMode.Single);

            //MD.StringData data = MessageDataPooling.GetMessageData<MD.StringData>();
            //data.value = targetScene;
            //SendMessageEx(MessageTitles.scene_loadSpecificLevel, GetSavedNumber("SceneManager"), data);
        };

        SendMessageEx(MessageTitles.uimanager_fadeout, GetSavedNumber("UIManager"), action);
    }

}
