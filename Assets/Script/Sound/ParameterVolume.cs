using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParameterVolume : MonoBehaviour
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
    public float max;

    public float radius;
    public float height;

    private float _factor;

    public void Update()
    {
        if(IsInVolume())
        {
            SetParameter();
        }
        else if(parameterClear)
        {
            float velo = 0f;
            _factor = Mathf.SmoothDamp(_factor,0f,ref velo,0.2f);
            GameManager.Instance.soundManager.SetGlobalParam(paramterCode,_factor);
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

            _factor = max * factor;

            GameManager.Instance.soundManager.SetGlobalParam(paramterCode,_factor);
        }
        else if(changeType == ParameterChangeType.HorizontalTop)
        {
            var targetY = GetTargetPosition().y;
            var factor = (targetY - GetBottom()) / height;

            _factor = max * factor;

            GameManager.Instance.soundManager.SetGlobalParam(paramterCode,_factor);
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
        return GameManager.Instance.player.transform.position;
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
