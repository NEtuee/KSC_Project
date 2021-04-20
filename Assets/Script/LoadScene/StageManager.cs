using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public string SceneTitle = "";

    public Elevator entranceElevator;
    public Elevator exitElevator;
    public Transform loadedPlayerPosition;

    public void ObjectTeleportToLoadedPos(Transform target, Vector3 center)
    {
        var centerDir = target.position - center;
        target.position = loadedPlayerPosition.position + centerDir;
    }
}
