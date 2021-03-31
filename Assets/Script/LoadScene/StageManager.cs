using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform prevZoneTransform;
    public Transform loadedPlayerPosition;

    private LoadingZone _prevLoadingZone;



    public void RegisterLoadingZone(LoadingZone prev)
    {
        _prevLoadingZone = prev;
        _prevLoadingZone.transform.SetPositionAndRotation(prevZoneTransform.position,prevZoneTransform.rotation);
    }

    public void SetPlayerToPosition()
    {
        GameManager.Instance.player.transform.position = loadedPlayerPosition.position;
        GameManager.Instance.player.transform.rotation = loadedPlayerPosition.rotation;
    }
}
