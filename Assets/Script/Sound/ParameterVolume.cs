using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParameterVolume : UnTransfromObjectBase
{
    public enum ParameterChangeType
    {
        VerticalCenter,
        HorizontalTop,
        Both
    }

    public enum CenterAlign
    {
        Bottom,
        Cetner,
        Top
    }

    public ParameterChangeType changeType;
    public CenterAlign align;

    public int paramterCode;
    public bool parameterClear = true;
    public bool inVolume = false;
    public float max;

    public float radius;
    public float height;

    public AnimationCurve fadeCurve;

    private float _factor;

    private Transform _targetTransform;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _targetTransform = ((PlayerUnit)x.data).transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);
    }

    public override void Progress(float deltaTime)
    {
        if(IsInVolume())
        {
            inVolume = true;
            SetParameter();
        }
        else if(inVolume)
        {
            parameterClear = true;
            inVolume = false;
        }
        else if(parameterClear)
        {
            float velo = 0f;
            _factor = Mathf.SmoothDamp(_factor,0f,ref velo,0.2f);

            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = 0;
            data.paramId = paramterCode;
            data.value = _factor;

            if(_factor <= 0.01f)
            {
                parameterClear = false;
            }

            SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);

            //GameManager.Instance.soundManager.SetGlobalParam(paramterCode,_factor);
        }

        
    }

    public void SetParameter()
    {
        if(changeType == ParameterChangeType.VerticalCenter)
        {
            var pos = MathEx.DeleteYPos(transform.position);
            var target = MathEx.DeleteYPos(GetTargetPosition());

            var dist = Vector3.Distance(pos,target);
            var factor = dist / radius;
            factor = 1f - factor;
            factor = fadeCurve.Evaluate(factor);

            _factor = max * factor;

            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = 0;
            data.paramId = paramterCode;
            data.value = _factor;

            SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);
            //GameManager.Instance.soundManager.SetGlobalParam(paramterCode,_factor);
        }
        else if(changeType == ParameterChangeType.HorizontalTop)
        {
            var targetY = GetTargetPosition().y;
            var factor = (targetY - GetBottom()) / height;
            factor = fadeCurve.Evaluate(factor);
            
            _factor = max * factor;

            var data = MessageDataPooling.GetMessageData<MD.SetParameterData>();
            data.soundId = 0;
            data.paramId = paramterCode;
            data.value = _factor;

            SendMessageEx(MessageTitles.fmod_setGlobalParam,GetSavedNumber("FMODManager"),data);

            //GameManager.Instance.soundManager.SetGlobalParam(paramterCode,_factor);
        }
        else if(changeType == ParameterChangeType.Both)
        {
            
        }
    }

    public bool IsInVolume()
    {
        var targetY = GetTargetPosition().y;

        if(targetY >= GetBottom() && targetY <= GetTop())
        {
            if(IsInRadius())
            {
                return true;
            }
        }

        return false;
    }

    public bool IsInRadius()
    {
        var pos = MathEx.DeleteYPos(transform.position);
        var target = MathEx.DeleteYPos(GetTargetPosition());

        var dist = Vector3.Distance(pos,target);
        return dist <= radius;
    }

    public Vector3 GetTargetPosition()
    {
        return _targetTransform.position;
    }

    public float GetBottom()
    {
        if(align == CenterAlign.Bottom)
        {
            return transform.position.y;
        }
        else if(align == CenterAlign.Cetner)
        {
            return transform.position.y - height * 0.5f;
        }
        else if(align == CenterAlign.Top)
        {
            return transform.position.y - height;
        }

        return 0f;
    }

    public float GetTop()
    {
        if(align == CenterAlign.Bottom)
        {
            return transform.position.y + height;
        }
        else if(align == CenterAlign.Cetner)
        {
            return transform.position.y + height * 0.5f;
        }
        else if(align == CenterAlign.Top)
        {
            return transform.position.y;
        }

        return 0f;
    }

    public float GetCenter()
    {
        if(align == CenterAlign.Bottom)
        {
            return transform.position.y - height * 0.5f;
        }
        else if(align == CenterAlign.Cetner)
        {
            return transform.position.y;
        }
        else if(align == CenterAlign.Top)
        {
            return transform.position.y + height * 0.5f;
        }

        return 0f;
    }

#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        Handles.color = Color.green;

        var pos = transform.position;
        var top = pos;
        var bottom = pos;

        top.y = GetTop();
        bottom.y = GetBottom();

        DrawCircle(top,radius);
        DrawCircle(bottom,radius);
        Handles.DrawLine(top,bottom);

        top.x -= radius;
        bottom.x = top.x;
        Handles.DrawLine(top,bottom);
        top.x += radius*2f;
        bottom.x = top.x;
        Handles.DrawLine(top,bottom);
        top.x -= radius;
        bottom.x = top.x;

        top.z -= radius;
        bottom.z = top.z;
        Handles.DrawLine(top,bottom);
        top.z += radius*2f;
        bottom.z = top.z;
        Handles.DrawLine(top,bottom);
    }

    public void DrawCircle(Vector3 position, float radius)
    {
        for(int i = 0; i <= 36; ++i)
        {
            var angle = 10f * (float)i;
            var radian = angle * Mathf.Deg2Rad;
            var point = position + new Vector3(Mathf.Cos(radian),0f,Mathf.Sin(radian)) * radius;

            angle = 10f * (float)(i == 36 ? 0 : i + 1);
            radian = angle * Mathf.Deg2Rad;
            var point2 = position + new Vector3(Mathf.Cos(radian),0f,Mathf.Sin(radian)) * radius;

            Handles.DrawLine(point,point2);
            // item.Points.Add(point);
            // item.Points.Add(point2);
        }

    }
#endif

}
