using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPositionChanger : MonoBehaviour
{
    public Transform aClear;
    public Transform bClear;
    public Transform respawn;
    public BooleanTrigger globalTrigger;

    public void Awake()
    {
        if(globalTrigger.FindTrigger("AClear").trigger)
        {
            if (globalTrigger.FindTrigger("BClear").trigger)
            {
                respawn.position = bClear.position;
                respawn.rotation = bClear.rotation;
            }
            else
            {
                respawn.position = aClear.position;
                respawn.rotation = aClear.rotation;
            }
        }
    }
}
