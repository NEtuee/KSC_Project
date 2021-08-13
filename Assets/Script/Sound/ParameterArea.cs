using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterArea : UnTransfromObjectBase
{
    public LayerMask targetLayer;
    public int parameterCode;
    public float enterValue;
    public float exitValue;

    public bool exitFade = false;

    private bool _fade = true;
    private float _timer = 0f;
    private float _currentFactor = 0f;
    private float _paramFactor = 0f;

    private List<Transform> _collsionList = new List<Transform>();

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.fmod_getGlobalParam,(x)=>{
            var data = MessageDataPooling.CastData<MD.FloatData>(x.data);
            _currentFactor = data.value;
        });
    }


    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void Progress(float deltaTime)
    {
        if(_fade)
        {
            _timer += deltaTime;

            var intData = MessageDataPooling.GetMessageData<MD.IntData>();
            intData.value = parameterCode;
            SendMessageQuick(MessageTitles.fmod_getGlobalParam,GetSavedNumber("FMODManager"),intData);

            if(_paramFactor != _currentFactor)
            {
                _fade = false;
                return;
            }

            _paramFactor = Mathf.Lerp(enterValue, exitValue,_timer);
            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = 0;
            data.paramId = parameterCode;
            data.value = _paramFactor;

            SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);

            //GameManager.Instance.soundManager.SetGlobalParam(parameterCode,Mathf.Lerp(enterValue, exitValue,_timer));

            if(_timer >= 1f)
            {
                _fade = false;
            }
        }
    }

    public void OnTriggerStay(Collider coll)
    {
        if(targetLayer == (targetLayer | (1<<coll.gameObject.layer)))
        {
            _fade = false;
            _collsionList.Add(coll.transform);

            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = 0;
            data.paramId = parameterCode;
            data.value = enterValue;

            SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);

            //GameManager.Instance.soundManager.SetGlobalParam(parameterCode,enterValue);
        }
       
    }

    public void OnTriggerExit(Collider coll)
    {
        if(targetLayer == (targetLayer | (1<<coll.gameObject.layer)))
        {
            _collsionList.Remove(coll.transform);
            if(_collsionList.Count == 0)
            {
                if(!exitFade)
                {
                    var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
                    data.soundId = 0;
                    data.paramId = parameterCode;
                    data.value = exitValue;

                    SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);
                }
                    //GameManager.Instance.soundManager.SetGlobalParam(parameterCode,exitValue);
                else
                {
                    _fade = true;
                    _paramFactor = enterValue;
                    _currentFactor = exitValue;
                    _timer = 0f;
                }
            }
            
        }
        
    }
}
