using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform prevZoneTransform;
    public Transform loadedPlayerPosition;

    public Transform prevPositionTarget;

    private LoadingZone _prevLoadingZone;
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private Vector3 _prevPos;
    private bool _progress = false;

    public void Close()
    {
        _prevLoadingZone.Close();
    }

    public void Update()
    {
        if(_progress)
        {
            float time = _timeCounter.IncreaseTimer("time",out var limit);
            if(limit)
            {
                _progress = false;
            }

            _prevLoadingZone.transform.position = Vector3.Lerp(_prevPos, prevPositionTarget.position,time);
        }
    }

    public void TranslatePrevLoadingZone()
    {
        _progress = true;
        _timeCounter.InitTimer("time");
        _prevPos = prevZoneTransform.position;
    }

    public void RegisterLoadingZone(LoadingZone prev)
    {
        _prevLoadingZone = prev;
        _prevLoadingZone.transform.SetPositionAndRotation(prevZoneTransform.position,prevZoneTransform.rotation);
        _prevLoadingZone.transform.SetParent(prevZoneTransform);
    }

    public void SetPlayerToPosition()
    {
        GameManager.Instance.player.transform.position = loadedPlayerPosition.position;
        GameManager.Instance.player.transform.rotation = loadedPlayerPosition.rotation;
    }
}
