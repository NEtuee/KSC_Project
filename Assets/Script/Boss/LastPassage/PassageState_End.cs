using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageState_End : PassageStateBase
{
    public override string stateIdentifier => "End";

    public string sceneCode;
    public float startTime;

    private bool _load = false;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("Start",0f,startTime);
        _load = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Start",out var limit, deltaTime);
        if(limit && !_load)
        {
            var data = MessageDataPooling.GetMessageData<MD.StringData>();
		    data.value = sceneCode;

	        target.SendMessageEx(MessageTitles.scene_loadSpecificLevel,
                UniqueNumberBase.GetSavedNumberStatic("SceneManager"),data);

            _load = true;
        }

        

    }
}
