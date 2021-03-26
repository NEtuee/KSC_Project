using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform prevZoneTransform;

    private LoadingZone _prevLoadingZone;

    public void RegisterLoadingZone(LoadingZone prev)
    {
        _prevLoadingZone = prev;
        _prevLoadingZone.transform.SetPositionAndRotation(prevZoneTransform.position,prevZoneTransform.rotation);
    }
}
